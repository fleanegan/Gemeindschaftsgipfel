using System.Net;
using System.Text.Json;
using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Exceptions;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.Controllers;

public class SupportTaskControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private const string HappyPathDummyId = "happyPathDummyId";
    private const string NonExistingDummyId = "nonExistingDummyId";
    private const string ConflictingDummyId = "conflictingDummyId";
    private readonly SupportTask _demoTask;
    private readonly IEnumerable<SupportTask> _demoTasks;


    private readonly WebApplicationFactory<Program> _factoryWithAuthorization;
    private readonly WebApplicationFactory<Program> _factoryWithoutAuthorization;

    private Mock<ISupportTaskService> _mockSupportTaskService = null!;

    public SupportTaskControllerTest(WebApplicationFactory<Program> factory)
    {
        TestHelper.ReadTestEnv();
        _demoTask = new SupportTask("description", "title", "duration", 2, []);
        _demoTasks = [_demoTask, _demoTask];
        _factoryWithAuthorization = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISupportTaskService));
                if (descriptor != null) services.Remove(descriptor);
                SetupMock(services);
                //authentication: this Middleware automatically adds user "FakeAuthUser"
                services.AddSingleton<IStartupFilter>(new AutoAuthorizeStartupFilter());
            });
        });

        _factoryWithoutAuthorization = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISupportTaskService));
                if (descriptor != null) services.Remove(descriptor);
                SetupMock(services);
            });
        });
    }

    private void SetupMock(IServiceCollection services)
    {
        _mockSupportTaskService = new Mock<ISupportTaskService>();
        _mockSupportTaskService.Setup(c => c.AddTask(It.IsAny<SupportTaskCreationDto>(), It.IsAny<string>()))
            .ReturnsAsync(
                (SupportTaskCreationDto _, string _) => new SupportTask("description", "title", "duration", 5, []));
        var dummySupportTaskCommitmentAction = (string supportTaskId, string _) =>
        {
            if (supportTaskId == NonExistingDummyId)
                throw new SupportTaskNotFoundException(supportTaskId);
            if (supportTaskId == ConflictingDummyId)
                throw new SupportPromiseImpossibleException(supportTaskId);
        };
        _mockSupportTaskService.Setup(s =>
                s.CommitToSupportTask(It.IsAny<string>(), It.IsAny<string>()))
            .Callback(dummySupportTaskCommitmentAction);
        _mockSupportTaskService.Setup(s =>
                s.ResignFromSupportTask(It.IsAny<string>(), It.IsAny<string>()))
            .Callback(dummySupportTaskCommitmentAction);
        _mockSupportTaskService.Setup(s => s.GetAll()).Returns(() => Task.Run(() => _demoTasks));
        services.AddSingleton(_mockSupportTaskService.Object);
    }

    [Fact]
    public async Task Test_add_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var userInput = new SupportTaskCreationDto("title", "description", "from x to y", 53);

        var response = await client.PostAsync("/SupportTask/AddNew", TestHelper.EncodeBody(userInput));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task
        Test_add_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response_DOES_THIS_HAVE_NON_REPEATING_BUG()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var userInput = new SupportTaskCreationDto("title", "description", "from x to y", 53);

        var response = await client.PostAsync("/SupportTask/AddNew", TestHelper.EncodeBody(userInput));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_add_GIVEN_malformed_request_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var userInput = new
        {
            Title = "title", Description = "description", Duration = "from x to y",
            RequiredSupporters = "This must be an int"
        };

        var response = await client.PostAsync("/SupportTask/AddNew", TestHelper.EncodeBody(userInput));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _mockSupportTaskService.Verify(c =>
            c.AddTask(It.IsAny<SupportTaskCreationDto>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Test_add_GIVEN_connected_user_WHEN_posting_THEN_call_service_and_return_success_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var userInput = new SupportTaskCreationDto("title", "description", "from x to y", 53);

        var response = await client.PostAsync("/SupportTask/AddNew", TestHelper.EncodeBody(userInput));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockSupportTaskService.Verify(c =>
            c.AddTask(It.IsAny<SupportTaskCreationDto>(), AutoAuthorizeMiddleware.UserName));
    }

    [Fact]
    public async Task Test_commitToSupportTask_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var userInput = new SupportPromiseDto(HappyPathDummyId);

        var response = await client.PostAsync("/SupportTask/help", TestHelper.EncodeBody(userInput));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        _mockSupportTaskService.Verify(c => c.CommitToSupportTask(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task
        Test_commitToSupportTask_GIVEN_connected_user_WHEN_impossible_exception_while_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var userInput = new SupportPromiseDto(ConflictingDummyId);

        var response = await client.PostAsync("/SupportTask/help", TestHelper.EncodeBody(userInput));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        _mockSupportTaskService.Verify(
            c => c.CommitToSupportTask(userInput.SupportTaskId, AutoAuthorizeMiddleware.UserName), Times.Once);
        Assert.Contains(userInput.SupportTaskId, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task
        Test_commitToSupportTask_GIVEN_connected_user_WHEN_not_found_exception_while_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var userInput = new SupportPromiseDto(NonExistingDummyId);

        var response = await client.PostAsync("/SupportTask/help", TestHelper.EncodeBody(userInput));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _mockSupportTaskService.Verify(
            c => c.CommitToSupportTask(userInput.SupportTaskId, AutoAuthorizeMiddleware.UserName), Times.Once);
        Assert.Contains(userInput.SupportTaskId, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task
        Test_commitToSupportTask_GIVEN_malformed_input_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var userInput = new { SupportTaskId = 123 };

        var response = await client.PostAsync("/SupportTask/help", TestHelper.EncodeBody(userInput));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _mockSupportTaskService.Verify(c => c.CommitToSupportTask(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task
        Test_commitToSupportTask_GIVEN_correct_input_WHEN_posting_THEN_return_success_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var userInput = new { SupportTaskId = HappyPathDummyId };

        var response = await client.PostAsync("/SupportTask/help", TestHelper.EncodeBody(userInput));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockSupportTaskService.Verify(
            c => c.CommitToSupportTask(userInput.SupportTaskId, AutoAuthorizeMiddleware.UserName), Times.Once);
    }

    [Fact]
    public async Task Test_resignFromSupportTask_GIVEN_no_connected_user_WHEN_deleting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();

        var response = await client.DeleteAsync("/SupportTask/help/" + HappyPathDummyId);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        _mockSupportTaskService.Verify(c => c.CommitToSupportTask(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task
        Test_resignFromSupportTask_GIVEN_connected_user_WHEN_impossible_exception_while_deleting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.DeleteAsync("/SupportTask/help/" + ConflictingDummyId);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        _mockSupportTaskService.Verify(
            c => c.ResignFromSupportTask(ConflictingDummyId, AutoAuthorizeMiddleware.UserName), Times.Once);
        Assert.Contains(ConflictingDummyId, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task
        Test_resignFromSupportTask_GIVEN_connected_user_WHEN_not_found_exception_while_deleting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.DeleteAsync("/SupportTask/help/" + NonExistingDummyId);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _mockSupportTaskService.Verify(
            c => c.ResignFromSupportTask(NonExistingDummyId, AutoAuthorizeMiddleware.UserName), Times.Once);
        Assert.Contains(NonExistingDummyId, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task
        Test_resignFromSupportTask_GIVEN_correct_input_WHEN_getting_THEN_return_success_response()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.DeleteAsync("/SupportTask/help/" + HappyPathDummyId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockSupportTaskService.Verify(c => c.ResignFromSupportTask(HappyPathDummyId, AutoAuthorizeMiddleware.UserName),
            Times.Once);
    }

    [Fact]
    public async Task Test_getAll_GIVEN_no_connected_user_WHEN_getting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();

        var response = await client.GetAsync("/SupportTask/GetAll");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        _mockSupportTaskService.Verify(c => c.GetAll(), Times.Never);
    }

    [Fact]
    public async Task
        Test_getAll_GIVEN_connected_user_with_some_SupportTasks_WHEN_getting_THEN_return_success_response_AND_list_with_SupportTasks()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.GetAsync("/SupportTask/GetAll");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockSupportTaskService.Verify(c => c.GetAll(), Times.Once);
        var dictionary =
            JsonSerializer.Deserialize<List<Dictionary<string, object>>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(_demoTask.Title, dictionary![0]["title"].ToString());
        Assert.Equal(_demoTask.Description, dictionary[0]["description"].ToString());
        Assert.Equal(_demoTask.Duration, dictionary[0]["duration"].ToString());
        Assert.Equal(2, dictionary.Count);
    }
}