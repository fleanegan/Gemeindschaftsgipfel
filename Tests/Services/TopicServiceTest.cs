using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Services;
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

        var exception = await Assert.ThrowsAsync<Exception>(Action);
        Assert.Equal("Invalid Topic Id", exception.Message);
    }

    [Fact]
    public async void Test_update_GIVEN_UserName_different_from_creator_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        ITopicService service = await GetService(dbContext, new List<string>([loggedInUserName]), repository);
        var originalTopic = await service.AddTopic(new TopicCreationDto("original title", ""), "anotherUserName");
        var updatedTopic = new TopicUpdateDto(originalTopic!.Id, "updated title", "updated description");

        async Task Action()
        {
            await service.UpdateTopic(updatedTopic, loggedInUserName);
        }

        var exception = await Assert.ThrowsAsync<Exception>(Action);
        Assert.Equal("This Topic does not belong to you. Not allowed to update", exception.Message);
    }


    [Fact]
    public async void Test_update_GIVEN_authorized_user_and_existing_topic_WHEN_passing_with_new_values_THEN_update()
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);
        var originalTopic = await service.AddTopic(new TopicCreationDto("original title", ""), loggedInUserName);
        var updatedTopic = new TopicUpdateDto(originalTopic.Id, "updated title", "updated description");

        await service.UpdateTopic(updatedTopic, loggedInUserName);

        var result = await repository.FetchBy(originalTopic.Id)!;
        Assert.Equal(updatedTopic.Title, result.Title);
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
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [otherUserName], repository);
        var firstTopicContent = new TopicCreationDto("first title", "first description");
        await service.AddTopic(firstTopicContent, otherUserName);
        var secondTopicContent = new TopicCreationDto("second title", "second description");
        await service.AddTopic(secondTopicContent, otherUserName);

        var result = await service.FetchAllExceptLoggedIn("loggedInUserName");

        Assert.Equal(2, result.Count());
        Assert.Contains(result,
            topic => topic.Title == firstTopicContent.Title && topic.Description == firstTopicContent.Description);
        Assert.Contains(result,
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

        Assert.Equal(1, result.Count());
        Assert.Contains(result,
            topic => topic.Title == firstTopicContent.Title && topic.Description == firstTopicContent.Description);
        Assert.DoesNotContain(result,
            topic => topic.Title == secondTopicContent.Title && topic.Description == secondTopicContent.Description);
    }

    private static async Task<TopicService> GetService(DatabaseContextApplication dbContext, List<string> userNames,
        TopicRepository repository)
    {
        var userStore = new UserStore<User>(dbContext);
        var userManager = await CannotInjectUserStoreDirectlySoWrappingInUserManager(userStore, userNames);
        var service = new TopicService(repository, userManager.Object);
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