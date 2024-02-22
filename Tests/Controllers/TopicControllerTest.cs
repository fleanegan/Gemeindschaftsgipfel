using System.Net;
using System.Text.Json;
using Kompetenzgipfel.Controllers;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Properties;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Abstractions;

namespace Tests.Controllers;

public class TopicControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _mockedResult = "mocked result";
    private readonly ITestOutputHelper _testOutputHelper;
    private Mock<ITopicService>? _mockTopicService;

    public TopicControllerTest(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITopicService));
                if (descriptor != null) services.Remove(descriptor);

                _mockTopicService = new Mock<ITopicService>();
                _mockTopicService.Setup(s => s.GetTopicsByPresenterId()).ReturnsAsync(_mockedResult);
                _mockTopicService.Setup(s => s.AddTopic(It.IsAny<TopicDto>(), It.IsAny<string>()))
                    .ReturnsAsync((TopicDto newTopic, string userName) =>
                        Topic.Create(newTopic.Title, newTopic.Description, new User { Id = "testId" }));
                services.AddSingleton(_mockTopicService.Object);

                //authentication: this Middleware automatically adds user 
                services.AddSingleton<IStartupFilter>(new AutoAuthorizeStartupFilter());
            });
        });
    }

    [Fact]
    public async Task Test_add_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response()
    {
        TestHelper.ShouldAddAuthorizedDummyUser(false);
        var client = _factory.CreateClient();
        var newTopic = new
        {
            Title = "Correct title",
            Description = "Correct description"
        };

        var response = await client.PostAsync("/topic/AddNew", TestHelper.encodeBody(newTopic));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_add_GIVEN_wrong_data_type_WHEN_posting_THEN_return_error_response()
    {
        var client = _factory.CreateClient();
        TestHelper.ShouldAddAuthorizedDummyUser(true);
        var jsonContent = TestHelper.encodeBody(new { });

        var response = await client.PostAsync("/topic/AddNew", jsonContent);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(Constants.EmptyTitleErrorMessage, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Test_add_GIVEN_correct_data_type_WHEN_posting_THEN_call_TopicService_and_return_Topic()
    {
        var client = _factory.CreateClient();
        TestHelper.ShouldAddAuthorizedDummyUser(true);
        var newTopic = new
        {
            Title = "Correct title",
            Description = "Correct description"
        };

        var response = await client.PostAsync("/topic/AddNew", TestHelper.encodeBody(newTopic));

        var responseContent = await response.Content.ReadAsStringAsync();
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        _mockTopicService?.Verify(x => x.AddTopic(It.IsAny<TopicDto>(), AutoAuthorizeMiddleware.UserName), Times.Once);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(newTopic.Title, dictionary?["title"].ToString());
        Assert.Equal(newTopic.Description, dictionary?["description"].ToString());
    }
}