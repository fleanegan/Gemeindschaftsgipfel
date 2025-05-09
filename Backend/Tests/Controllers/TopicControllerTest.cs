using System.Net;
using System.Text.Json;
using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Exceptions;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Properties;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.Controllers;

public class TopicControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private const string HappyPathDummyId = "happyPathDummyId";
    private const string NonExistingDummyId = "nonExistingDummyId";
    private const string ConflictingDummyId = "conflictingDummyId";
    private List<int> legalPresentationDurations = [15,30,45];

    private readonly IEnumerable<Topic> _demoTopics =
    [
        new("description", "title",
            new User { UserName = AutoAuthorizeMiddleware.UserName },
            [new Vote(Topic.Create("asdf", 5, "asdf", new User()), new User { UserName = "demo voter" })])
    ];

    private readonly WebApplicationFactory<Program> _factoryWithAuthorization;
    private readonly WebApplicationFactory<Program> _factoryWithoutAuthorization;

    private Mock<ITopicService>? _mockTopicService;

    public TopicControllerTest(WebApplicationFactory<Program> factory)
    {
        TestHelper.ReadTestEnv();
        _factoryWithAuthorization = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITopicService));
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
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITopicService));
                if (descriptor != null) services.Remove(descriptor);

                SetupMock(services);
            });
        });
    }

    private void SetupMock(IServiceCollection services)
    {
        _mockTopicService = new Mock<ITopicService>();
	_mockTopicService.Setup(s => s.GetLegalPresentationDurations()).Returns(() => {return legalPresentationDurations;});
        _mockTopicService.Setup(s => s.GetTopicById(It.IsAny<string>())).Returns(async (string topicId) =>
        {
            return await Task.Run(() =>
            {
                if (topicId == NonExistingDummyId)
                    throw new TopicNotFoundException(topicId);
                var topic = _demoTopics.ToArray()[0];
                topic.Id = HappyPathDummyId;
                return topic;
            });
        });
        _mockTopicService.Setup(s => s.AddTopic(It.IsAny<TopicCreationDto>(), It.IsAny<string>()))
            .ReturnsAsync((TopicCreationDto newTopic, string _) =>
            {
                var topic = Topic.Create(newTopic.Title, newTopic.PresentationTimeInMinutes, newTopic.Description ?? "",
                    new User { Id = "testId" });
                topic.Id = HappyPathDummyId;
                return topic;
            });
        _mockTopicService.Setup(s =>
                s.UpdateTopic(It.IsAny<TopicUpdateDto>(), It.IsAny<string>()))
            .ReturnsAsync((TopicUpdateDto topicToUpdate, string _) =>
            {
                if (topicToUpdate.Id == NonExistingDummyId)
                    throw new TopicNotFoundException(topicToUpdate.Id);
                if (topicToUpdate.Id == ConflictingDummyId)
                    throw new UnauthorizedTopicModificationException(topicToUpdate.Id);
                var topic = Topic.Create(topicToUpdate.Title, topicToUpdate.PresentationTimeInMinutes,
                    topicToUpdate.Description ?? "",
                    new User { Id = "testId" });
                topic.Id = HappyPathDummyId;
                topic.Votes = [];
                return topic;
            });
        _mockTopicService.Setup(c => c.FetchAllExceptLoggedIn(It.IsAny<string>())).ReturnsAsync(() =>
            _demoTopics);
        _mockTopicService.Setup(c => c.FetchAllOfLoggedIn(It.IsAny<string>())).ReturnsAsync(() =>
            _demoTopics);
        _mockTopicService.Setup(c => c.AddTopicVote(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(async (string topicId, string _) =>
            {
                await Task.Run(() =>
                {
                    if (topicId == NonExistingDummyId)
                        throw new TopicNotFoundException(topicId);
                    if (topicId == ConflictingDummyId)
                        throw new VoteImpossibleException(topicId);
                });
            });
        _mockTopicService.Setup(c => c.RemoveTopicVote(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(async (string topicId, string _) =>
            {
                await Task.Run(() =>
                {
                    if (topicId == NonExistingDummyId)
                        throw new TopicNotFoundException(topicId);
                    if (topicId == ConflictingDummyId)
                        throw new VoteImpossibleException(topicId);
                });
            });
        _mockTopicService.Setup(c => c.RemoveTopic(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(async (string topicId, string _) =>
            {
                await Task.Run(() =>
                {
                    if (topicId == NonExistingDummyId)
                        throw new TopicNotFoundException(topicId);
                    if (topicId == ConflictingDummyId)
                        throw new UnauthorizedTopicModificationException(topicId);
                });
            });
        _mockTopicService.Setup(c => c.CommentOnTopic(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(async (string topicId, string content, string userName) =>
            {
                await Task.Run(() =>
                {
                    if (topicId == NonExistingDummyId)
                        throw new TopicNotFoundException(topicId);
                });
            });
        _mockTopicService.Setup(c => c.GetCommentsForTopic(It.IsAny<string>())).ReturnsAsync((String topicId) =>
        {
            if (topicId == NonExistingDummyId)
                throw new TopicNotFoundException(topicId);
            return [];
        });
        services.AddSingleton(_mockTopicService.Object);
    }

    [Fact]
    public async Task Test_add_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var newTopic = new
        {
            Title = "Correct title",
            PresentationTimeInMinutes = 5,
            Description = "Correct description"
        };

        var response = await client.PostAsync("/topic/AddNew", TestHelper.EncodeBody(newTopic));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_add_GIVEN_wrong_data_type_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var jsonContent = TestHelper.EncodeBody(new { });

        var response = await client.PostAsync("/topic/AddNew", jsonContent);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(Constants.EmptyTitleErrorMessage, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Test_add_GIVEN_correct_data_type_WHEN_posting_THEN_call_TopicService_and_return_Topic()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var newTopic = new
        {
            Title = "Correct title",
            PresentationTimeInMinutes = 5,
            Description = "Correct description"
        };

        var response = await client.PostAsync("/topic/AddNew", TestHelper.EncodeBody(newTopic));

        var responseContent = await response.Content.ReadAsStringAsync();
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        _mockTopicService?.Verify(x => x.AddTopic(It.IsAny<TopicCreationDto>(), AutoAuthorizeMiddleware.UserName),
            Times.Once);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(newTopic.Title, dictionary?["title"].ToString());
        Assert.Equal(newTopic.PresentationTimeInMinutes,
            Convert.ToInt32(dictionary?["presentationTimeInMinutes"].ToString()));
        Assert.Equal(newTopic.Description, dictionary?["description"].ToString());
        Assert.Equal(HappyPathDummyId, dictionary?["id"].ToString());
    }

    [Fact]
    public async Task Test_getOne_GIVEN_no_connected_user_WHEN_getting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();

        var response = await client.GetAsync("/topic/GetOne/" + HappyPathDummyId);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_GetLegalPresentationDuration_GIVEN_present_configuration_WHEN_getting_THEN_return_array()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.GetAsync("/topic/GetLegalPresentationDurations/");

	var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	Assert.Equal(responseContent, JsonSerializer.Serialize(legalPresentationDurations));
    }

    [Fact]
    public async Task Test_getOne_GIVEN_non_existing_id_WHEN_getting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.GetAsync("/topic/GetOne/" + NonExistingDummyId);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Test_getOne_GIVEN_existing_id_WHEN_getting_THEN_return_topic()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.GetAsync("/topic/GetOne/" + HappyPathDummyId);

        var responseContent = await response.Content.ReadAsStringAsync();
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        _mockTopicService?.Verify(x => x.GetTopicById(HappyPathDummyId),
            Times.Once);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(_demoTopics.ToArray()[0].Title, dictionary?["title"].ToString());
        Assert.Equal(_demoTopics.ToArray()[0].Description, dictionary?["description"].ToString());
        Assert.Equal(_demoTopics.ToArray()[0].PresentationTimeInMinutes,
            Convert.ToInt32(dictionary?["presentationTimeInMinutes"].ToString()));
        Assert.Equal(HappyPathDummyId, dictionary?["id"].ToString());
    }

    [Fact]
    public async Task Test_update_GIVEN_no_connected_user_WHEN_putting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var updatedTopic = new
        {
            Id = "Correct id",
            PresentationTimeInMinutes = 5,
            Title = "Correct title",
            Description = "Correct description"
        };

        var response = await client.PutAsync("/topic/Update", TestHelper.EncodeBody(updatedTopic));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_update_GIVEN_wrong_data_type_WHEN_putting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var updatedTopic = TestHelper.EncodeBody(new { });

        var response = await client.PutAsync("/topic/Update", TestHelper.EncodeBody(updatedTopic));

        var errorDescription = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(Constants.EmptyTitleErrorMessage, errorDescription);
        Assert.Contains(Constants.EmptyIdErrorMessage, errorDescription);
    }

    [Fact]
    public async Task Test_update_GIVEN_non_existing_id_WHEN_putting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var updatedTopic = new
        {
            Id = NonExistingDummyId,
            Title = "Correct title",
            PresentationTimeInMinutes = 5,
            Description = "Correct description"
        };

        var response = await client.PutAsync("/topic/Update", TestHelper.EncodeBody(updatedTopic));

        var errorDescription = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Id", errorDescription);
    }

    [Fact]
    public async Task Test_update_GIVEN_id_of_someone_else_s_topic_WHEN_putting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var updatedTopic = new
        {
            Id = ConflictingDummyId,
            Title = "Correct title",
            PresentationTimeInMinutes = 5,
            Description = "Correct description"
        };

        var response = await client.PutAsync("/topic/Update", TestHelper.EncodeBody(updatedTopic));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Test_update_GIVEN_correct_input_WHEN_putting_THEN_success_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var payload = new TopicUpdateDto(
            "Correct id",
            "Correct title",
            3,
            "Correct description"
        );

        var response = await client.PutAsync("/topic/Update", TestHelper.EncodeBody(payload));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockTopicService!.Verify(
            c => c.UpdateTopic(
                It.Is<TopicUpdateDto>(dto =>
                    dto.Description == payload.Description && dto.Id == payload.Id && dto.Title == payload.Title),
                It.IsAny<string>()), Times.Once);
        var dictionary =
            JsonSerializer.Deserialize<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(dictionary);
        Assert.Equal(payload.Title, dictionary["title"].ToString());
        Assert.Equal(payload.PresentationTimeInMinutes,
            Convert.ToInt32(dictionary["presentationTimeInMinutes"].ToString()));
        Assert.Equal(payload.Description, dictionary["description"].ToString());
    }

    [Fact]
    public async Task
        Test_allExceptLoggedIn_GIVEN_connected_user_and_some_posts_WHEN_getting_THEN_call_service_and_return_view_model()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.GetAsync("/topic/allExceptLoggedIn");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dictionary =
            JsonSerializer.Deserialize<List<Dictionary<string, object>>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(dictionary);
        _mockTopicService!.Verify(c => c.FetchAllExceptLoggedIn(AutoAuthorizeMiddleware.UserName), Times.Once);
        Assert.Contains("description", dictionary[0]["description"].ToString());
        Assert.Equal(_demoTopics.First().PresentationTimeInMinutes,
            Convert.ToInt32(dictionary[0]["presentationTimeInMinutes"].ToString()));
        Assert.Contains("title", dictionary[0]["title"].ToString());
        Assert.Equal("False", dictionary[0]["didIVoteForThis"].ToString());
        Assert.Contains(AutoAuthorizeMiddleware.UserName, dictionary[0]["presenterUserName"].ToString());
    }

    [Fact]
    public async Task Test_allExceptLoggedIn_GIVEN_no_connected_user_WHEN_getting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();

        var response = await client.GetAsync("/topic/allExceptLoggedIn");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_allOfLoggedIn_GIVEN_no_connected_user_WHEN_getting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();

        var response = await client.GetAsync("/topic/allOfLoggedIn");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task
        Test_allOfLoggedIn_GIVEN_connected_user_and_some_posts_WHEN_getting_THEN_call_service_and_return_view_model()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.GetAsync("/topic/allOfLoggedIn");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dictionary =
            JsonSerializer.Deserialize<List<Dictionary<string, object>>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(dictionary);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockTopicService!.Verify(c => c.FetchAllOfLoggedIn(AutoAuthorizeMiddleware.UserName), Times.Once);
        Assert.Contains("description", dictionary[0]["description"].ToString());
        Assert.Equal(_demoTopics.First().PresentationTimeInMinutes,
            Convert.ToInt32(dictionary[0]["presentationTimeInMinutes"].ToString()));
        Assert.Contains("title", dictionary[0]["title"].ToString());
        Assert.Contains("1", dictionary[0]["votes"].ToString());
        Assert.Contains(AutoAuthorizeMiddleware.UserName, dictionary[0]["presenterUserName"].ToString());
    }

    [Fact]
    public async Task Test_removeTopic_GIVEN_no_connected_user_WHEN_deleting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();

        var response = await client.DeleteAsync("/topic/Delete/" + HappyPathDummyId);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_removeTopic_GIVEN_non_existing_id_WHEN_deleting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.DeleteAsync("/topic/Delete/" + NonExistingDummyId);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Test_removeTopic_GIVEN_forbidden_topic_id_WHEN_deleting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.DeleteAsync("/topic/Delete/" + ConflictingDummyId);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Test_addVote_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var voteBody = new TopicVoteDto("some id");

        var response = await client.PostAsync("/topic/addVote", TestHelper.EncodeBody(voteBody));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_addVote_GIVEN_wrong_user_input_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var voteBody = new { Manamana = "dip di bidibi" };

        var response = await client.PostAsync("/topic/addVote", TestHelper.EncodeBody(voteBody));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _mockTopicService!.Verify(c => c.AddTopicVote(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Test_addVote_GIVEN_non_existing_topic_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var voteBody = new TopicVoteDto(NonExistingDummyId);

        var response = await client.PostAsync("/topic/addVote", TestHelper.EncodeBody(voteBody));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _mockTopicService!.Verify(c => c.AddTopicVote(NonExistingDummyId, AutoAuthorizeMiddleware.UserName),
            Times.Once);
    }

    [Fact]
    public async Task Test_addVote_GIVEN_conflicting_topic_state_WHEN_posting_THEN_return_error_response(
    )
    {
        var client = _factoryWithAuthorization.CreateClient();
        var voteBody = new TopicVoteDto(ConflictingDummyId);

        var response = await client.PostAsync("/topic/addVote", TestHelper.EncodeBody(voteBody));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        _mockTopicService!.Verify(c => c.AddTopicVote(ConflictingDummyId, AutoAuthorizeMiddleware.UserName),
            Times.Once);
    }

    [Fact]
    public async Task Test_removeVote_GIVEN_no_connected_user_WHEN_deleting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();

        var response = await client.DeleteAsync("/topic/removeVote/" + HappyPathDummyId);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_removeVote_GIVEN_non_existing_topic_WHEN_deleting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.DeleteAsync("/topic/removeVote/" + NonExistingDummyId);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _mockTopicService!.Verify(c => c.RemoveTopicVote(NonExistingDummyId, AutoAuthorizeMiddleware.UserName),
            Times.Once);
    }

    [Fact]
    public async Task Test_removeVote_GIVEN_conflicting_topic_state_WHEN_deleting_THEN_return_error_response(
    )
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.DeleteAsync("/topic/removeVote/" + ConflictingDummyId);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        _mockTopicService!.Verify(c => c.RemoveTopicVote(ConflictingDummyId, AutoAuthorizeMiddleware.UserName),
            Times.Once);
    }

    [Fact]
    public async Task Test_removeVote_GIVEN_valid_topic_id_WHEN_deleting_THEN_return_success_response(
    )
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.DeleteAsync("/topic/removeVote/" + HappyPathDummyId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockTopicService!.Verify(c => c.RemoveTopicVote(HappyPathDummyId, AutoAuthorizeMiddleware.UserName),
            Times.Once);
    }

    [Fact]
    public async Task Test_attachPost_GIVEN_no_connected_user_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();
        var newPost = new
        {
            topicId = HappyPathDummyId,
            content = "Test content"
        };

        var response = await client.PostAsync("/topic/CommentOnTopic", TestHelper.EncodeBody(newPost));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_attachPost_GIVEN_wrong_data_type_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var jsonContent = TestHelper.EncodeBody(new { });

        var response = await client.PostAsync("/topic/CommentOnTopic", jsonContent);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Test_attachPost_GIVEN_non_existing_topic_WHEN_posting_THEN_return_error_response()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var newPost = new
        {
            topicId = NonExistingDummyId,
            content = "Test content"
        };

        var response = await client.PostAsync("/topic/CommentOnTopic", TestHelper.EncodeBody(newPost));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _mockTopicService?.Verify(
            x => x.CommentOnTopic(NonExistingDummyId, "Test content", AutoAuthorizeMiddleware.UserName),
            Times.Once);
    }

    [Fact]
    public async Task Test_attachPost_GIVEN_correct_data_WHEN_posting_THEN_call_TopicService_and_return_success()
    {
        var client = _factoryWithAuthorization.CreateClient();
        var newPost = new
        {
            topicId = HappyPathDummyId,
            content = "Test content"
        };

        var response = await client.PostAsync("/topic/CommentOnTopic", TestHelper.EncodeBody(newPost));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockTopicService?.Verify(
            x => x.CommentOnTopic(HappyPathDummyId, "Test content", AutoAuthorizeMiddleware.UserName),
            Times.Once);
    }

    [Fact]
    public async Task Test_getPosts_GIVEN_no_connected_user_WHEN_getting_THEN_return_error_response()
    {
        var client = _factoryWithoutAuthorization.CreateClient();

        var response = await client.GetAsync($"/topic/Comments?topicId={HappyPathDummyId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test_getPosts_GIVEN_non_existing_topic_WHEN_getting_THEN_return_not_found()
    {
        var client = _factoryWithAuthorization.CreateClient();


        var response = await client.GetAsync($"/topic/Comments?topicId={NonExistingDummyId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _mockTopicService?.Verify(x => x.GetCommentsForTopic(NonExistingDummyId), Times.Once);
    }

    [Fact]
    public async Task Test_getPosts_GIVEN_existing_topic_WHEN_getting_THEN_return_posts()
    {
        var client = _factoryWithAuthorization.CreateClient();

        var response = await client.GetAsync($"/topic/Comments?topicId={HappyPathDummyId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockTopicService?.Verify(x => x.GetCommentsForTopic(HappyPathDummyId), Times.Once);
    }
}