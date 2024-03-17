using System.Net;
using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Exceptions;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Services;
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

    private readonly WebApplicationFactory<Program> _factoryWithAuthorization;
    private readonly WebApplicationFactory<Program> _factoryWithoutAuthorization;

    private Mock<ISupportTaskService> _mockSupportTaskService;

    public SupportTaskControllerTest(WebApplicationFactory<Program> factory)
    {
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
                (SupportTaskCreationDto _, string _) => new SupportTask(title: "title",
                    description: "description", supportPromises: [], requiredSupporters: 5));
        _mockSupportTaskService.Setup(s =>
                s.CommitToSupportTask(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string supportTopicId, string _) =>
            {
                if (supportTopicId == NonExistingDummyId)
                    throw new SupportTaskNotFoundException(supportTopicId);
                if (supportTopicId == ConflictingDummyId)
                    throw new SupportPromiseImpossibleException(supportTopicId);
            });
        services.AddSingleton(_mockSupportTaskService.Object);
    }

    [Fact]
    public async Task Test_add_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var userInput = new SupportTaskCreationDto("title", "description", "from x to y", 53);

        var response = await client.PostAsync("/SupportTask/AddNew", TestHelper.encodeBody(userInput));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task
        Test_add_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response_DOES_THIS_HAVE_NON_REPEATING_BUG()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var userInput = new SupportTaskCreationDto("title", "description", "from x to y", 53);

        var response = await client.PostAsync("/SupportTask/AddNew", TestHelper.encodeBody(userInput));

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

        var response = await client.PostAsync("/SupportTask/AddNew", TestHelper.encodeBody(userInput));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _mockSupportTaskService.Verify(c =>
            c.AddTask(It.IsAny<SupportTaskCreationDto>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Test_add_GIVEN_connected_user_WHEN_posting_THEN_call_service_and_return_success_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var userInput = new SupportTaskCreationDto("title", "description", "from x to y", 53);

        var response = await client.PostAsync("/SupportTask/AddNew", TestHelper.encodeBody(userInput));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockSupportTaskService.Verify(c =>
            c.AddTask(It.IsAny<SupportTaskCreationDto>(), AutoAuthorizeMiddleware.UserName));
    }

    [Fact]
    public async Task Test_commitToSupportTask_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var userInput = new SupportPromiseDto { SupportTaskId = HappyPathDummyId };

        var response = await client.PostAsync("/SupportTask/help", TestHelper.encodeBody(userInput));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task
        Test_commitToSupportTask_GIVEN_connected_user_WHEN_impossible_exception_while_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var userInput = new SupportPromiseDto { SupportTaskId = ConflictingDummyId };

        var response = await client.PostAsync("/SupportTask/help", TestHelper.encodeBody(userInput));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task
        Test_commitToSupportTask_GIVEN_connected_user_WHEN_not_found_exception_while_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var userInput = new SupportPromiseDto { SupportTaskId = NonExistingDummyId };

        var response = await client.PostAsync("/SupportTask/help", TestHelper.encodeBody(userInput));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}