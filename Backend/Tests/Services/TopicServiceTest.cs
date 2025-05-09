using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Exceptions;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Repositories;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Services;

public class TopicServiceTest
{
    private const int AnAllowedPresentationTime = 5;
    private const int AnotherAllowedPresentationTime = 15;

    [Theory]
    [InlineData("Correct title", "")]
    [InlineData("Correct title", null)]
    [InlineData("Correct title", "Non empty but also correct title")]
    public async Task Test_add_GIVEN_correct_input_THEN_store_in_db(string title, string? description)
    {
        const string loggedInUserName = "Fake User";
        var instance = await CreateInstance([loggedInUserName]);

        await instance.Service.AddTopic(new TopicCreationDto(title, AnAllowedPresentationTime, description),
            loggedInUserName);

        var result = (await instance.Repository.GetAll()).ToArray()[0];
        Assert.NotNull(result);
        Assert.Equal(title, result.Title);
        Assert.Equal(description ?? "", result.Description);
        Assert.Equal(AnAllowedPresentationTime, result.PresentationTimeInMinutes);
        Assert.Equal(loggedInUserName, result.User.UserName);
    }

    [Fact]
    public async Task Test_add_GIVEN_duration_not_listed_THEN_throw_exception()
    {
        const int ilallowedPresentationTime = 66;
        const string loggedInUserName = "Fake User";
        var instance = await CreateInstance([loggedInUserName]);

        async Task Action()
        {
            await instance.Service.AddTopic(new TopicCreationDto("title", ilallowedPresentationTime, "description"),
                loggedInUserName);
        }

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(Action);
    }

    [Fact]
    public async Task Test_getById_GIVEN_non_existing_id_THEN_throw_exception()
    {
        const string nonExistingId = "nonExistingId";
        var instance = await CreateInstance([]);

        async Task Action()
        {
            await instance.Service.GetTopicById(nonExistingId);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
    }


    [Fact]
    public async Task Test_getById_GIVEN_existing_id_THEN_return_result()
    {
        const string loggedInUserName = "Fake User";
        var instance = await CreateInstance([loggedInUserName]);
        var topicCreationDto = new TopicCreationDto("title", AnAllowedPresentationTime, "description");
        var expectedResult = await instance.Service.AddTopic(topicCreationDto, loggedInUserName);

        var actualResult = await instance.Service.GetTopicById(expectedResult.Id);

        Assert.Equal(expectedResult.Id, actualResult.Id);
        Assert.Equal(expectedResult.Description, actualResult.Description);
        Assert.Equal(expectedResult.Title, actualResult.Title);
    }

    [Fact]
    public async Task Test_update_GIVEN_non_existing_id_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        var nonExistingId = "the original topic does not exist";
        var updatedTopic = new TopicUpdateDto(nonExistingId, "title", AnAllowedPresentationTime, "description");
        var instance = await CreateInstance([loggedInUserName]);

        async Task Action()
        {
            await instance.Service.UpdateTopic(updatedTopic, loggedInUserName);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
    }

    [Fact]
    public async Task Test_update_GIVEN_UserName_different_from_creator_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        var instance = await CreateInstance([loggedInUserName]);
        var originalTopic =
            await instance.Service.AddTopic(new TopicCreationDto("original title", AnAllowedPresentationTime, ""),
                "anotherUserName");
        var updatedTopic = new TopicUpdateDto(originalTopic.Id, "updated title", AnAllowedPresentationTime,
            "updated description");

        async Task Action()
        {
            await instance.Service.UpdateTopic(updatedTopic, loggedInUserName);
        }

        await Assert.ThrowsAsync<UnauthorizedTopicModificationException>(Action);
    }


    [Fact]
    public async Task
        Test_update_GIVEN_duration_not_listed_AND_no_allowed_durations_whatsoever_WHEN_passing_with_new_values_THEN_do_not_throw()
    {
        int newPresentationTimeInMinutes = 33;
        const string loggedInUserName = "Fake User";
        var instance = await CreateInstance([loggedInUserName], []);
        var originalTopic =
            await instance.Service.AddTopic(new TopicCreationDto("original title", 76, ""),
                loggedInUserName);
        var updatedTopic =
            new TopicUpdateDto(originalTopic.Id, "newTitle", newPresentationTimeInMinutes, "newDescription");
        
        await instance.Service.UpdateTopic(updatedTopic, loggedInUserName);
    }


    [Theory]
    [InlineData("Correct title", "")]
    [InlineData("Correct title", null)]
    [InlineData("Correct title", "Non empty but also correct title")]
    public async Task Test_update_GIVEN_authorized_user_and_existing_topic_WHEN_passing_with_new_values_THEN_update(
        string newTitle, string? newDescription)
    {
        int newPresentationTimeInMinutes = AnotherAllowedPresentationTime;
        const string loggedInUserName = "Fake User";
        var instance = await CreateInstance([loggedInUserName]);
        var originalTopic =
            await instance.Service.AddTopic(new TopicCreationDto("original title", AnAllowedPresentationTime, ""),
                loggedInUserName);
        var updatedTopic = new TopicUpdateDto(originalTopic.Id, newTitle, newPresentationTimeInMinutes, newDescription);

        await instance.Service.UpdateTopic(updatedTopic, loggedInUserName);

        var result = await instance.Repository.FetchBy(originalTopic.Id);
        Assert.Equal(updatedTopic.Title, result!.Title);
        Assert.Equal(updatedTopic.PresentationTimeInMinutes, result.PresentationTimeInMinutes);
    }

    [Fact]
    public async Task Test_fetchAllExceptLoggedIn_GIVEN_zero_topics_THEN_return_empty()
    {
        var loggedInUserName = "loggedInUserName";
        var instance = await CreateInstance([loggedInUserName]);

        var result = await instance.Service.FetchAllExceptLoggedIn(loggedInUserName);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Test_fetchAllExceptLoggedIn_GIVEN_two_topics_by_other_users_THEN_return_them_all()
    {
        var otherUserName = "otherUserName";
        var firstTopicContent = new TopicCreationDto("first title", AnAllowedPresentationTime, "first description");
        var secondTopicContent = new TopicCreationDto("second title", AnAllowedPresentationTime, "second description");
        var instance = await CreateInstance([otherUserName]);
        await instance.Service.AddTopic(firstTopicContent, otherUserName);
        await instance.Service.AddTopic(secondTopicContent, otherUserName);

        var result = await instance.Service.FetchAllExceptLoggedIn("loggedInUserName");

        var enumerable = result as Topic[] ?? result.ToArray();
        Assert.Equal(2, enumerable.Count());
        Assert.Contains(enumerable,
            topic => topic.Title == firstTopicContent.Title && topic.Description == firstTopicContent.Description);
        Assert.Contains(enumerable,
            topic => topic.Title == secondTopicContent.Title && topic.Description == secondTopicContent.Description);
    }

    [Fact]
    public async Task
        Test_fetchAllExceptLoggedIn_GIVEN_topic_logged_in_with_different_case_THEN_do_not_show_as_foreign()
    {
        var nameDuringCreation = "loggedInUserName";
        var nameDuringRetrieval = "LOGGEDINUSERNAME";
        var instance = await CreateInstance([nameDuringCreation]);
        var ownTopic = new TopicCreationDto("first title", AnAllowedPresentationTime, "first description");
        await instance.Service.AddTopic(ownTopic, nameDuringCreation);

        var result = await instance.Service.FetchAllExceptLoggedIn(nameDuringRetrieval);

        var collection = result as Topic[] ?? result.ToArray();
        Assert.Empty(collection);
    }

    [Fact]
    public async Task
        Test_fetchAllExceptLoggedIn_GIVEN_two_topics_by_two_different_users_THEN_return_only_those_by_other_user()
    {
        var loggedInUserName = "loggedInUserName";
        var otherUserName = "otherUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var instance = await CreateInstance([loggedInUserName, otherUserName]);
        var firstTopicContent = new TopicCreationDto("first title", 5, "first description");
        await instance.Service.AddTopic(firstTopicContent, otherUserName);
        var secondTopicContent = new TopicCreationDto("second title", AnAllowedPresentationTime, "second description");
        await instance.Service.AddTopic(secondTopicContent, loggedInUserName);

        var result = await instance.Service.FetchAllExceptLoggedIn(loggedInUserName);

        var collection = result as Topic[] ?? result.ToArray();
        Assert.Single(collection);
        Assert.Contains(collection,
            topic => topic.Title == firstTopicContent.Title && topic.Description == firstTopicContent.Description);
        Assert.DoesNotContain(collection,
            topic => topic.Title == secondTopicContent.Title && topic.Description == secondTopicContent.Description);
    }

    [Fact]
    public async Task Test_fetchAllOfLoggedIn_GIVEN_zero_topics_THEN_return_empty()
    {
        var loggedInUserName = "loggedInUserName";
        var instance = await CreateInstance([loggedInUserName]);

        var result = await instance.Service.FetchAllOfLoggedIn(loggedInUserName);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Test_fetchAllOfLoggedIn_GIVEN_two_topics_by_other_users_THEN_return_empty()
    {
        var otherUserName = "otherUserName";
        var firstTopicContent = new TopicCreationDto("first title", AnAllowedPresentationTime, "first description");
        var secondTopicContent = new TopicCreationDto("second title", AnAllowedPresentationTime, "second description");
        var instance = await CreateInstance([otherUserName]);
        await instance.Service.AddTopic(firstTopicContent, otherUserName);
        await instance.Service.AddTopic(secondTopicContent, otherUserName);

        var result = await instance.Service.FetchAllOfLoggedIn("loggedInUserName");

        var enumerable = result as Topic[] ?? result.ToArray();
        Assert.Empty(enumerable);
    }

    [Fact]
    public async Task
        Test_fetchAllOfLoggedIn_GIVEN_two_topics_by_two_different_users_THEN_return_only_those_by_current_user()
    {
        var loggedInUserName = "loggedInUserName";
        var otherUserName = "otherUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var instance = await CreateInstance([loggedInUserName, otherUserName]);
        var firstTopicContent = new TopicCreationDto("first title", 5, "first description");
        await instance.Service.AddTopic(firstTopicContent, otherUserName);
        var secondTopicContent = new TopicCreationDto("second title", AnAllowedPresentationTime, "second description");
        await instance.Service.AddTopic(secondTopicContent, loggedInUserName);

        var result = await instance.Service.FetchAllOfLoggedIn(loggedInUserName);

        var collection = result as Topic[] ?? result.ToArray();
        Assert.Single(collection);
        Assert.DoesNotContain(collection,
            topic => topic.Title == firstTopicContent.Title && topic.Description == firstTopicContent.Description);
        Assert.Contains(collection,
            topic => topic.Title == secondTopicContent.Title && topic.Description == secondTopicContent.Description);
    }

    [Fact]
    public async Task
        Test_fetchAllOfLoggedIn_GIVEN_case_difference_two_topics_by_two_different_users_THEN_return_only_those_by_current_user()
    {
        var loggedInUserName = "loggedInUserName";
        var otherUserName = "otherUserName";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var instance = await CreateInstance([loggedInUserName, otherUserName]);
        var firstTopicContent = new TopicCreationDto("first title", 5, "first description");
        await instance.Service.AddTopic(firstTopicContent, otherUserName);
        var secondTopicContent = new TopicCreationDto("second title", AnAllowedPresentationTime, "second description");
        await instance.Service.AddTopic(secondTopicContent, loggedInUserName);

        var result = await instance.Service.FetchAllOfLoggedIn(loggedInUserName.ToUpper());

        var collection = result as Topic[] ?? result.ToArray();
        Assert.Single(collection);
        Assert.DoesNotContain(collection,
            topic => topic.Title == firstTopicContent.Title && topic.Description == firstTopicContent.Description);
        Assert.Contains(collection,
            topic => topic.Title == secondTopicContent.Title && topic.Description == secondTopicContent.Description);
    }

    [Fact]
    public async Task Test_removing_topic_GIVEN_non_existing_topic_THEN_throw_error()
    {
        const string loggedInUserName = "loggedInUserName";
        const string nonExistingId = "nonExistingId";
        var instance = await CreateInstance([loggedInUserName]);

        async Task Action()
        {
            await instance.Service.RemoveTopic(nonExistingId, loggedInUserName);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
    }

    [Fact]
    public async Task Test_removing_topic_GIVEN_someone_else_s_topic_THEN_throw_error()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "creatorUserName";
        var instance = await CreateInstance([loggedInUserName, creatorUserName]);
        var topicToDelete =
            await instance.Service.AddTopic(new TopicCreationDto("title", AnAllowedPresentationTime, "description"),
                creatorUserName);

        async Task Action()
        {
            await instance.Service.RemoveTopic(topicToDelete.Id, loggedInUserName);
        }

        var exception = await Assert.ThrowsAsync<Exception>(Action);
        Assert.Equal("Not your Topic", exception.Message);
    }

    [Fact]
    public async Task Test_removing_topic_GIVEN_one_s_own_topic_THEN_remove_topic()
    {
        const string loggedInUserName = "loggedInUserName";
        var instance = await CreateInstance([loggedInUserName]);
        var topicToDelete =
            await instance.Service.AddTopic(new TopicCreationDto("title", AnAllowedPresentationTime, "description"),
                loggedInUserName);


        await instance.Service.RemoveTopic(topicToDelete.Id, loggedInUserName);

        Assert.Empty(await instance.Repository.GetAll());
    }

    [Fact]
    public async Task Test_adding_vote_GIVEN_a_non_existing_topic_THEN_throw_error()
    {
        const string loggedInUserName = "loggedInUserName";
        var instance = await CreateInstance([loggedInUserName]);
        const string nonExistingId = "non-existing Id";

        async Task Action()
        {
            await instance.Service.AddTopicVote(nonExistingId, loggedInUserName);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
        Assert.Empty(await instance.Repository.GetAllForUser(loggedInUserName));
    }

    [Fact]
    public async Task Test_adding_vote_GIVEN_an_existing_topic_and_two_users_THEN_add_voter_to_topic()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "otherUserName";
        var instance = await CreateInstance([loggedInUserName, creatorUserName]);
        var firstTopicContent = new TopicCreationDto("first title", AnAllowedPresentationTime, "first description");
        var topic = await instance.Service.AddTopic(firstTopicContent, creatorUserName);

        await instance.Service.AddTopicVote(topic.Id, loggedInUserName);

        var result = await instance.Repository.FetchBy(topic.Id);
        Assert.Equal(loggedInUserName, result!.Votes.ToArray()[0].Voter.UserName);
    }

    [Fact]
    public async Task Test_adding_vote_GIVEN_voting_twice_with_same_user_THEN_add_voter_to_topic_once()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "otherUserName";
        var instance = await CreateInstance([loggedInUserName, creatorUserName]);
        var firstTopicContent = new TopicCreationDto("first title", AnAllowedPresentationTime, "first description");
        var topic = await instance.Service.AddTopic(firstTopicContent, creatorUserName);

        await instance.Service.AddTopicVote(topic.Id, loggedInUserName);

        async Task Action()
        {
            await instance.Service.AddTopicVote(topic.Id, loggedInUserName);
        }

        await Assert.ThrowsAsync<VoteImpossibleException>(Action);
        var result = await instance.Repository.FetchBy(topic.Id);
        Assert.Single(result!.Votes);
    }

    [Fact]
    public async Task Test_removing_vote_GIVEN_a_non_existing_topic_THEN_throw_error()
    {
        const string loggedInUserName = "loggedInUserName";
        var instance = await CreateInstance([loggedInUserName]);
        const string nonExistingId = "non-existing Id";

        async Task Action()
        {
            await instance.Service.RemoveTopicVote(nonExistingId, loggedInUserName);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
    }

    [Fact]
    public async Task Test_removing_vote_GIVEN_an_existing_vote_THEN_remove_voter_from_topic()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "otherUserName";
        var instance = await CreateInstance([loggedInUserName, creatorUserName]);
        var firstTopicContent = new TopicCreationDto("first title", AnAllowedPresentationTime, "first description");
        var topic = await instance.Service.AddTopic(firstTopicContent, creatorUserName);
        await instance.Service.AddTopicVote(topic.Id, loggedInUserName);

        await instance.Service.RemoveTopicVote(topic.Id, loggedInUserName);

        var result = await instance.Repository.FetchBy(topic.Id);
        Assert.Empty(result!.Votes.ToArray());
    }

    [Fact]
    public async Task Test_removing_vote_GIVEN_an_existing_vote_with_another_case_THEN_remove_voter_from_topic()
    {
        const string loggedInUserNameForVoteCreation = "loggedInUserName";
        const string loggedInUserNameForVoteRemoval = "LOGGEDINUSERNAME";
        const string creatorUserName = "otherUserName";
        var instance = await CreateInstance([loggedInUserNameForVoteCreation, loggedInUserNameForVoteRemoval]);
        var firstTopicContent = new TopicCreationDto("first title", AnAllowedPresentationTime, "first description");
        var topic = await instance.Service.AddTopic(firstTopicContent, creatorUserName);
        await instance.Service.AddTopicVote(topic.Id, loggedInUserNameForVoteCreation);

        await instance.Service.RemoveTopicVote(topic.Id, loggedInUserNameForVoteRemoval);

        var result = await instance.Repository.FetchBy(topic.Id);
        Assert.Empty(result!.Votes.ToArray());
    }

    [Fact]
    public async Task Test_removing_vote_GIVEN_removing_twice_with_same_user_THEN_do_nothing()
    {
        const string loggedInUserName = "loggedInUserName";
        const string creatorUserName = "otherUserName";
        var instance = await CreateInstance([loggedInUserName, creatorUserName]);
        var firstTopicContent = new TopicCreationDto("first title", AnAllowedPresentationTime, "first description");
        var topic = await instance.Service.AddTopic(firstTopicContent, creatorUserName);

        await instance.Service.RemoveTopicVote(topic.Id, loggedInUserName);

        var result = await instance.Repository.FetchBy(topic.Id);
        Assert.Empty(result!.Votes);
    }

    [Fact]
    public async Task Test_addPostToTopic_GIVEN_non_existing_topic_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        var instance = await CreateInstance([loggedInUserName]);
        const string nonExistingId = "non-existing-id";

        async Task Action()
        {
            await instance.Service.CommentOnTopic(nonExistingId, "Test content", loggedInUserName);
        }

        await Assert.ThrowsAsync<TopicNotFoundException>(Action);
    }

    [Fact]
    public async Task Test_addPostToTopic_GIVEN_existing_topic_THEN_create_post()
    {
        const string loggedInUserName = "Fake User";
        var instance = await CreateInstance([loggedInUserName]);
        var topic = await instance.Service.AddTopic(
            new TopicCreationDto("Test Topic", AnAllowedPresentationTime, "Test Description"), 
            loggedInUserName);
        
        await instance.Service.CommentOnTopic(topic.Id, "Test post content", loggedInUserName);

        var postsAsArray = await FetchAllPostsFromRepositoryForTopic(instance, topic);
        Assert.Single(postsAsArray);
        Assert.Equal("Test post content", postsAsArray[0].Content);
        Assert.Equal(loggedInUserName, postsAsArray[0].Creator.UserName);
        Assert.Equal(topic.Id, postsAsArray[0].Topic.Id);
    }

    private static async Task<InstanceWrapper> CreateInstance(List<String> availableUserNames)
    {
        return await CreateInstance(availableUserNames, [AnAllowedPresentationTime, AnotherAllowedPresentationTime]);
    }

    private static async Task<InstanceWrapper> CreateInstance(List<String> availableUserNames,
        List<int> allowedPresentationDurationsInMin)
    {
        var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var topicRepository = new TopicRepository(dbContext);
        var postRepository = new TopicCommentRepository(dbContext);
        var topicService = await GetService(dbContext, availableUserNames, topicRepository, postRepository,
            allowedPresentationDurationsInMin);
        return new InstanceWrapper(topicRepository, postRepository, topicService);
    }

    private static async Task<TopicService> GetService(DatabaseContextApplication dbContext, List<string> userNames,
        TopicRepository repository, TopicCommentRepository topicCommentRepository, List<int> allowedPresentationDurationsInMin)
    {
        var voteRepository = new VoteRepository(dbContext);
        var userStore = new UserStore<User>(dbContext);
        var userManager = await CannotInjectUserStoreDirectlySoWrappingInUserManager(userStore, userNames);
        var service = new TopicService(repository, voteRepository, topicCommentRepository, userManager.Object,
            allowedPresentationDurationsInMin);
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

    private class InstanceWrapper(
        TopicRepository topicRepository,
        TopicCommentRepository topicCommentRepository,
        TopicService topicService)
    {
        public readonly TopicRepository Repository = topicRepository;
        public readonly TopicCommentRepository TopicCommentRepository = topicCommentRepository;
        public readonly TopicService Service = topicService;
    }

    private static async Task<TopicComment[]> FetchAllPostsFromRepositoryForTopic(InstanceWrapper instance, Topic topic)
    {
        var posts = await instance.TopicCommentRepository.GetCommentsForTopic(topic.Id);
        var postsAsArray = posts as TopicComment[] ?? posts.ToArray();
        return postsAsArray;
    }
}
