using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Exceptions;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Repositories;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;

namespace Tests.Services;

public class TopicServiceTest
{
    [Theory]
    [InlineData("Correct title", "")]
    [InlineData("Correct title", null)]
    [InlineData("Correct title", "Non empty but also correct title")]
    public async void Test_add_GIVEN_correct_input_THEN_store_in_db(string title, string? description)
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        ITopicService service = await GetService(dbContext, [loggedInUserName], repository);

        await service.AddTopic(new TopicCreationDto(title, description), loggedInUserName);

        var result = (await repository.GetAll()).ToArray()[0];
        Assert.NotNull(result);
        Assert.Equal(title, result.Title);
        Assert.Equal(description ?? "", result.Description);
        Assert.Equal(loggedInUserName, result.User.UserName);
    }

    [Fact]
    public async Task Test_getById_GIVEN_non_existing_id_THEN_throw_exception()
    {
        const string nonExistingId = "nonExistingId";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [], repository);

        async Task Action()
        {
            await service.GetTopicById(nonExistingId);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
    }


    [Fact]
    public async Task Test_getById_GIVEN_existing_id_THEN_return_result()
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);
        var topicCreationDto = new TopicCreationDto("title", "description");
        var expectedResult = await service.AddTopic(topicCreationDto, loggedInUserName);

        var actualResult = await service.GetTopicById(expectedResult.Id);

        Assert.Equal(expectedResult.Id, actualResult.Id);
        Assert.Equal(expectedResult.Description, actualResult.Description);
        Assert.Equal(expectedResult.Title, actualResult.Title);
    }

    [Fact]
    public async void Test_update_GIVEN_non_existing_id_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        var nonExistingId = "the original topic does not exist";
        var updatedTopic = new TopicUpdateDto(nonExistingId, "title", "description");
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);

        async Task Action()
        {
            await service.UpdateTopic(updatedTopic, loggedInUserName);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
    }

    [Fact]
    public async void Test_update_GIVEN_UserName_different_from_creator_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        ITopicService service = await GetService(dbContext, [loggedInUserName], repository);
        var originalTopic = await service.AddTopic(new TopicCreationDto("original title", ""), "anotherUserName");
        var updatedTopic = new TopicUpdateDto(originalTopic!.Id, "updated title", "updated description");

        async Task Action()
        {
            await service.UpdateTopic(updatedTopic, loggedInUserName);
        }

        await Assert.ThrowsAsync<UnauthorizedTopicModificationException>(Action);
    }


    [Theory]
    [InlineData("Correct title", "")]
    [InlineData("Correct title", null)]
    [InlineData("Correct title", "Non empty but also correct title")]
    public async void Test_update_GIVEN_authorized_user_and_existing_topic_WHEN_passing_with_new_values_THEN_update(
        string newTitle, string newDescription)
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);
        var originalTopic = await service.AddTopic(new TopicCreationDto("original title", ""), loggedInUserName);
        var updatedTopic = new TopicUpdateDto(originalTopic.Id, newTitle, newDescription);

        await service.UpdateTopic(updatedTopic, loggedInUserName);

        var result = await repository.FetchBy(originalTopic.Id);
        Assert.Equal(updatedTopic.Title, result!.Title);
    }

    [Fact]
    public async void Test_fetchAllExceptLoggedIn_GIVEN_zero_topics_THEN_return_empty()
    {
        var loggedInUserName = "loggedInUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);

        var result = await service.FetchAllExceptLoggedIn(loggedInUserName);

        Assert.Empty(result);
    }

    [Fact]
    public async void Test_fetchAllExceptLoggedIn_GIVEN_two_topics_by_other_users_THEN_return_them_all()
    {
        var otherUserName = "otherUserName";
        var firstTopicContent = new TopicCreationDto("first title", "first description");
        var secondTopicContent = new TopicCreationDto("second title", "second description");
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [otherUserName], repository);
        await service.AddTopic(firstTopicContent, otherUserName);
        await service.AddTopic(secondTopicContent, otherUserName);

        var result = await service.FetchAllExceptLoggedIn("loggedInUserName");

        var enumerable = result as Topic[] ?? result.ToArray();
        Assert.Equal(2, enumerable.Count());
        Assert.Contains(enumerable,
            topic => topic.Title == firstTopicContent.Title && topic.Description == firstTopicContent.Description);
        Assert.Contains(enumerable,
            topic => topic.Title == secondTopicContent.Title && topic.Description == secondTopicContent.Description);
    }

    [Fact]
    public async void
        Test_fetchAllExceptLoggedIn_GIVEN_two_topics_by_two_different_users_THEN_return_only_those_by_other_user()
    {
        var loggedInUserName = "loggedInUserName";
        var otherUserName = "otherUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName, otherUserName], repository);
        var firstTopicContent = new TopicCreationDto("first title", "first description");
        await service.AddTopic(firstTopicContent, otherUserName);
        var secondTopicContent = new TopicCreationDto("second title", "second description");
        await service.AddTopic(secondTopicContent, loggedInUserName);

        var result = await service.FetchAllExceptLoggedIn(loggedInUserName);

        var collection = result as Topic[] ?? result.ToArray();
        Assert.Single(collection);
        Assert.Contains(collection,
            topic => topic.Title == firstTopicContent.Title && topic.Description == firstTopicContent.Description);
        Assert.DoesNotContain(collection,
            topic => topic.Title == secondTopicContent.Title && topic.Description == secondTopicContent.Description);
    }

    [Fact]
    public async void Test_fetchAllOfLoggedIn_GIVEN_zero_topics_THEN_return_empty()
    {
        var loggedInUserName = "loggedInUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);

        var result = await service.FetchAllOfLoggedIn(loggedInUserName);

        Assert.Empty(result);
    }

    [Fact]
    public async void Test_fetchAllOfLoggedIn_GIVEN_two_topics_by_other_users_THEN_return_empty()
    {
        var otherUserName = "otherUserName";
        var firstTopicContent = new TopicCreationDto("first title", "first description");
        var secondTopicContent = new TopicCreationDto("second title", "second description");
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [otherUserName], repository);
        await service.AddTopic(firstTopicContent, otherUserName);
        await service.AddTopic(secondTopicContent, otherUserName);

        var result = await service.FetchAllOfLoggedIn("loggedInUserName");

        var enumerable = result as Topic[] ?? result.ToArray();
        Assert.Empty(enumerable);
    }

    [Fact]
    public async void
        Test_fetchAllOfLoggedIn_GIVEN_two_topics_by_two_different_users_THEN_return_only_those_by_current_user()
    {
        var loggedInUserName = "loggedInUserName";
        var otherUserName = "otherUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName, otherUserName], repository);
        var firstTopicContent = new TopicCreationDto("first title", "first description");
        await service.AddTopic(firstTopicContent, otherUserName);
        var secondTopicContent = new TopicCreationDto("second title", "second description");
        await service.AddTopic(secondTopicContent, loggedInUserName);

        var result = await service.FetchAllOfLoggedIn(loggedInUserName);

        var collection = result as Topic[] ?? result.ToArray();
        Assert.Single(collection);
        Assert.DoesNotContain(collection,
            topic => topic.Title == firstTopicContent.Title && topic.Description == firstTopicContent.Description);
        Assert.Contains(collection,
            topic => topic.Title == secondTopicContent.Title && topic.Description == secondTopicContent.Description);
    }

    [Fact]
    public async void Test_removing_topic_GIVEN_non_existing_topic_THEN_throw_error()
    {
        const string loggedInUserName = "loggedInUserName";
        const string nonExistingId = "nonExistingId";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);

        async Task Action()
        {
            await service.RemoveTopic(nonExistingId, loggedInUserName);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
    }

    [Fact]
    public async void Test_removing_topic_GIVEN_someone_else_s_topic_THEN_throw_error()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "creatorUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName, creatorUserName], repository);
        var topicToDelete = await service.AddTopic(new TopicCreationDto("title", "description"), creatorUserName);


        async Task Action()
        {
            await service.RemoveTopic(topicToDelete.Id, loggedInUserName);
        }

        var exception = await Assert.ThrowsAsync<Exception>(Action);
        Assert.Equal("Not your Topic", exception.Message);
    }

    [Fact]
    public async void Test_removing_topic_GIVEN_one_s_own_topic_THEN_remove_topic()
    {
        const string loggedInUserName = "loggedInUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);
        var topicToDelete = await service.AddTopic(new TopicCreationDto("title", "description"), loggedInUserName);


        await service.RemoveTopic(topicToDelete.Id, loggedInUserName);

        Assert.Empty(await new TopicRepository(dbContext).GetAll());
    }

    [Fact]
    public async void Test_adding_vote_GIVEN_a_non_existing_topic_THEN_throw_error()
    {
        const string loggedInUserName = "loggedInUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);
        const string nonExistingId = "non-existing Id";

        async Task Action()
        {
            await service.AddTopicVote(nonExistingId, loggedInUserName);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
        Assert.Empty(await new VoteRepository(dbContext).FetchAll());
    }

    [Fact]
    public async void Test_adding_vote_GIVEN_an_existing_topic_and_two_users_THEN_add_voter_to_topic()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "otherUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName, creatorUserName], repository);
        var firstTopicContent = new TopicCreationDto("first title", "first description");
        var topic = await service.AddTopic(firstTopicContent, creatorUserName);

        await service.AddTopicVote(topic.Id, loggedInUserName);

        var result = await repository.FetchBy(topic.Id);
        Assert.Equal(loggedInUserName, result!.Votes.ToArray()[0].Voter.UserName);
    }

    [Fact]
    public async void Test_adding_vote_GIVEN_voting_twice_with_same_user_THEN_add_voter_to_topic_once()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "otherUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName, creatorUserName], repository);
        var firstTopicContent = new TopicCreationDto("first title", "first description");
        var topic = await service.AddTopic(firstTopicContent, creatorUserName);

        await service.AddTopicVote(topic.Id, loggedInUserName);

        async Task Action()
        {
            await service.AddTopicVote(topic.Id, loggedInUserName);
        }

        await Assert.ThrowsAsync<VoteImpossibleException>(Action);
        var result = await repository.FetchBy(topic.Id);
        Assert.Equal(1, result!.Votes.Count);
    }

    [Fact]
    public async void Test_removing_vote_GIVEN_a_non_existing_topic_THEN_throw_error()
    {
        const string loggedInUserName = "loggedInUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);
        const string nonExistingId = "non-existing Id";

        async Task Action()
        {
            await service.RemoveTopicVote(nonExistingId, loggedInUserName);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
    }

    [Fact]
    public async void Test_removing_vote_GIVEN_an_existing_vote_THEN_remove_voter_from_topic()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "otherUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName, creatorUserName], repository);
        var firstTopicContent = new TopicCreationDto("first title", "first description");
        var topic = await service.AddTopic(firstTopicContent, creatorUserName);
        await service.AddTopicVote(topic.Id, loggedInUserName);

        await service.RemoveTopicVote(topic.Id, loggedInUserName);

        var result = await repository.FetchBy(topic.Id);
        Assert.Empty(result!.Votes.ToArray());
    }

    [Fact]
    public async void Test_removing_vote_GIVEN_removing_twice_with_same_user_THEN_do_nothing()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "otherUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName, creatorUserName], repository);
        var firstTopicContent = new TopicCreationDto("first title", "first description");
        var topic = await service.AddTopic(firstTopicContent, creatorUserName);

        await service.RemoveTopicVote(topic.Id, loggedInUserName);

        var result = await repository.FetchBy(topic.Id);
        Assert.Equal(0, result!.Votes.Count);
    }

    private static async Task<TopicService> GetService(DatabaseContextApplication dbContext, List<string> userNames,
        TopicRepository repository)
    {
        var voteRepository = new VoteRepository(dbContext);
        var userStore = new UserStore<User>(dbContext);
        var userManager = await CannotInjectUserStoreDirectlySoWrappingInUserManager(userStore, userNames);
        var service = new TopicService(repository, voteRepository, userManager.Object);
        return service;
    }

    private static async Task<Mock<UserManager<User>>> CannotInjectUserStoreDirectlySoWrappingInUserManager(
        UserStore<User> userStore, List<string> userNames)
    {
        foreach (var userName in userNames)
            await userStore.CreateAsync(new User
                { AccessFailedCount = 0, UserName = userName, LockoutEnabled = false, EmailConfirmed = false });
        var userManager = TestHelper.GetMockUserManager(userStore);
        userManager.Setup(c => c.FindByNameAsync(It.IsAny<string>()))
            .Returns((string inputUserName) => Task.FromResult(new User { UserName = inputUserName })!);
        return userManager;
    }
}