using System.Net;
using System.Text.Json;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Properties;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.Controllers;

public class TopicControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _mockedResult = "mocked result";
    private Mock<ITopicService>? mockTopicService;

    public TopicControllerTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITopicService));
                if (descriptor != null) services.Remove(descriptor);

                mockTopicService = new Mock<ITopicService>();
                mockTopicService.Setup(s => s.GetTopicsByPresenterId()).ReturnsAsync(_mockedResult);
                mockTopicService.Setup(s => s.AddTopic(It.IsAny<Topic>()))
                    .ReturnsAsync((Topic newTopic) => Topic.Create(newTopic.Title, newTopic.Description));
                services.AddSingleton(mockTopicService.Object);
            });
        });
    }

    [Fact]
    public async Task TestMyController()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/topic/GetById/150");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(_mockedResult, responseString);
    }

    [Fact]
    public async Task Test_add_GIVEN_wrong_data_type_WHEN_posting_THEN_return_error_response()
    {
        // Arrange
        var client = _factory.CreateClient();
        var jsonContent = TestHelper.encodeBody(new { });

        // Act
        var response = await client.PostAsync("/topic/AddNew", jsonContent);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(Constants.EmptyTitleErrorMessage, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Test_add_GIVEN_correct_data_type_WHEN_posting_THEN_call_TopicService_and_return_Topic()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newTopic = new
        {
            Title = "Correct title",
            Description = "Correct description"
        };

        // Act
        var response = await client.PostAsync("/topic/AddNew", TestHelper.encodeBody(newTopic));

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        mockTopicService?.Verify(x => x.AddTopic(It.IsAny<Topic>()), Times.Once);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(newTopic.Title, dictionary?["title"].ToString());
        Assert.Equal(newTopic.Description, dictionary?["description"].ToString());
    }


    //        public async Task Test_add_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response()
    //        public async Task Test_add_GIVEN_connected_user_WHEN_posting_THEN_add_user_id_to_service_call()
}