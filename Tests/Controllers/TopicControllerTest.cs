using System.Net;
using System.Text.Json;
using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Properties;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.Controllers;

public class TopicControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private const string DummyId = "dummyId";
    private const string NonExistingDummyId = "nonExistingDummyId";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _mockedResult = "mocked result";
    private Mock<ITopicService>? _mockTopicService;

    public TopicControllerTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITopicService));
                if (descriptor != null) services.Remove(descriptor);

                IEnumerable<Topic> demoTopics =
                [
                    new Topic("description", "title",
                        new User { UserName = AutoAuthorizeMiddleware.UserName },
                        [new Vote(Topic.Create("asdf", "asdf", new User()), new User())])
                ];
                _mockTopicService = new Mock<ITopicService>();
                _mockTopicService.Setup(s => s.GetTopicsByPresenterId()).ReturnsAsync(_mockedResult);
                _mockTopicService.Setup(s => s.AddTopic(It.IsAny<TopicCreationDto>(), It.IsAny<string>()))
                    .ReturnsAsync((TopicCreationDto newTopic, string _) =>
                    {
                        var topic = Topic.Create(newTopic.Title, newTopic.Description ?? "",
                            new User { Id = "testId" });
                        topic!.Id = DummyId;
                        return topic;
                    });
                _mockTopicService.Setup(s =>
                        s.UpdateTopic(It.IsAny<TopicUpdateDto>(), It.IsAny<string>()))
                    .ReturnsAsync((TopicUpdateDto topicToUpdate, string _) =>
                    {
                        if (topicToUpdate.Id == NonExistingDummyId) throw new Exception("Invalid Topic Id");
                        var topic = Topic.Create(topicToUpdate.Title, topicToUpdate.Description ?? "",
                            new User { Id = "testId" });
                        topic!.Id = DummyId;
                        topic.Votes = [];
                        return topic;
                    });
                _mockTopicService.Setup(c => c.FetchAllExceptLoggedIn(It.IsAny<string>())).ReturnsAsync(() =>
                    demoTopics);
                _mockTopicService.Setup(c => c.FetchAllOfLoggedIn(It.IsAny<string>())).ReturnsAsync(() =>
                    demoTopics);
                _mockTopicService.Setup(c => c.AddTopicVote(It.IsAny<string>(), It.IsAny<string>())).Returns(
                    (string topicId, string userName) =>
                    {
                        return Task.Run(() =>
                        {
                            if (topicId == NonExistingDummyId) throw new Exception("Invalid Topic Id");
                        });
                    });
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
        _mockTopicService?.Verify(x => x.AddTopic(It.IsAny<TopicCreationDto>(), AutoAuthorizeMiddleware.UserName),
            Times.Once);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(newTopic.Title, dictionary?["title"].ToString());
        Assert.Equal(newTopic.Description, dictionary?["description"].ToString());
        Assert.Equal(DummyId, dictionary?["id"].ToString());
    }

    [Fact]
    public async Task Test_update_GIVEN_no_connected_user_WHEN_putting_THEN_return_error_response()
    {
        TestHelper.ShouldAddAuthorizedDummyUser(false);
        var client = _factory.CreateClient();
        var updatedTopic = new
        {
            Id = "Correct id",
            Title = "Correct title",
            Description = "Correct description"
        };

        var response = await client.PutAsync("/topic/Update", TestHelper.encodeBody(updatedTopic));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_update_GIVEN_wrong_data_type_WHEN_putting_THEN_return_error_response()
    {
        var client = _factory.CreateClient();
        TestHelper.ShouldAddAuthorizedDummyUser(true);
        var updatedTopic = TestHelper.encodeBody(new { });

        var response = await client.PutAsync("/topic/Update", TestHelper.encodeBody(updatedTopic));

        var errorDescription = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(Constants.EmptyTitleErrorMessage, errorDescription);
        Assert.Contains(Constants.EmptyIdErrorMessage, errorDescription);
    }

    [Fact]
    public async Task Test_update_GIVEN_non_existing_id_WHEN_putting_THEN_return_error_response()
    {
        var client = _factory.CreateClient();
        TestHelper.ShouldAddAuthorizedDummyUser(true);
        var updatedTopic = new
        {
            Id = NonExistingDummyId,
            Title = "Correct title",
            Description = "Correct description"
        };

        var response = await client.PutAsync("/topic/Update", TestHelper.encodeBody(updatedTopic));

        var errorDescription = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Id", errorDescription);
    }

    [Fact]
    public async Task Test_update_GIVEN_correct_input_WHEN_putting_THEN_return_error_response()
    {
        TestHelper.ShouldAddAuthorizedDummyUser(true);
        var client = _factory.CreateClient();
        var updatedTopic = new
        {
            Id = "Correct id",
            Title = "Correct title",
            Description = "Correct description"
        };

        var response = await client.PutAsync("/topic/Update", TestHelper.encodeBody(updatedTopic));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //todo: find out why test not passing with
        // new TopicUpdateDto(updatedTopic.Id, updatedTopic.Title, updatedTopic.Description)
        _mockTopicService!.Verify(
            c => c.UpdateTopic(It.IsAny<TopicUpdateDto>(),
                It.IsAny<string>()), Times.Once);
        var dictionary =
            JsonSerializer.Deserialize<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(dictionary);
        Assert.Equal(updatedTopic.Title, dictionary["title"].ToString());
        Assert.Equal(updatedTopic.Description, dictionary["description"].ToString());
    }

    [Fact]
    public async Task
        Test_allExceptLoggedIn_GIVEN_connected_user_and_some_posts_WHEN_getting_THEN_call_service_and_return_view_model()
    {
        TestHelper.ShouldAddAuthorizedDummyUser(true);
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/topic/allExceptLoggedIn");

        var dictionary =
            JsonSerializer.Deserialize<List<Dictionary<string, string>>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(dictionary);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockTopicService!.Verify(c => c.FetchAllExceptLoggedIn(AutoAuthorizeMiddleware.UserName), Times.Once);
        Assert.Contains("description", dictionary[0]["description"]);
        Assert.Contains("title", dictionary[0]["title"]);
        Assert.Contains(AutoAuthorizeMiddleware.UserName, dictionary[0]["presenterUserName"]);
    }

    [Fact]
    public async Task Test_allExceptLoggedIn_GIVEN_no_connected_user_WHEN_getting_THEN_return_error_response()
    {
        TestHelper.ShouldAddAuthorizedDummyUser(false);
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/topic/allExceptLoggedIn");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_allOfLoggedIn_GIVEN_no_connected_user_WHEN_getting_THEN_return_error_response()
    {
        TestHelper.ShouldAddAuthorizedDummyUser(false);
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/topic/allOfLoggedIn");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task
        Test_allOfLoggedIn_GIVEN_connected_user_and_some_posts_WHEN_getting_THEN_call_service_and_return_view_model()
    {
        TestHelper.ShouldAddAuthorizedDummyUser(true);
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/topic/allOfLoggedIn");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dictionary =
            JsonSerializer.Deserialize<List<Dictionary<string, string>>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(dictionary);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockTopicService!.Verify(c => c.FetchAllOfLoggedIn(AutoAuthorizeMiddleware.UserName), Times.Once);
        Assert.Contains("description", dictionary[0]["description"]);
        Assert.Contains("title", dictionary[0]["title"]);
        Assert.Contains("1", dictionary[0]["votes"]);
        Assert.Contains(AutoAuthorizeMiddleware.UserName, dictionary[0]["presenterUserName"]);
    }

    [Theory]
    [InlineData("add")]
    [InlineData("remove")]
    public async Task Test_addVote_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response(string urlExtension)
    {
        TestHelper.ShouldAddAuthorizedDummyUser(false);
        var client = _factory.CreateClient();
        var voteBody = new
        {
            TopicId = "some id"
        };

        var response = await client.PostAsync("/topic/" + urlExtension + "Vote", TestHelper.encodeBody(voteBody));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("add")]
    [InlineData("remove")]
    public async Task Test_addVote_GIVEN_wrong_user_input_WHEN_posting_THEN_return_error_response(string urlExtension)
    {
        TestHelper.ShouldAddAuthorizedDummyUser(true);
        var client = _factory.CreateClient();
        var voteBody = new
        {
            Manana = "dip di bidibi"
        };

        var response = await client.PostAsync("/topic/" + urlExtension + "Vote", TestHelper.encodeBody(voteBody));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _mockTopicService!.Verify(c => c.AddTopicVote(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        _mockTopicService!.Verify(c => c.RemoveTopicVote(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Theory]
    [InlineData("add")]
    [InlineData("remove")]
    public async Task Test_addVote_GIVEN_correct_input_WHEN_posting_THEN_return_ok_response(string urlExtension)
    {
        TestHelper.ShouldAddAuthorizedDummyUser(true);
        var client = _factory.CreateClient();
        var voteBody = new
        {
            TopicId = DummyId
        };

        var response = await client.PostAsync("/topic/" + urlExtension + "Vote", TestHelper.encodeBody(voteBody));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        if (urlExtension == "add")
            _mockTopicService!.Verify(c => c.AddTopicVote(DummyId, AutoAuthorizeMiddleware.UserName),
                Times.Once);
        if (urlExtension == "remove")
            _mockTopicService!.Verify(c => c.RemoveTopicVote(DummyId, AutoAuthorizeMiddleware.UserName),
                Times.Once);
    }
}