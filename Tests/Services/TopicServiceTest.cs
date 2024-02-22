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
    [InlineData("Correct title", "Non empty but also correct title")]
    public async void Test_add_GIVEN_correct_input_THEN_store_in_db(string title, string description)
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);

        await service.AddTopic(new TopicDto(title, description), loggedInUserName);

        var result = repository.GetAll().FirstOrDefault(topic => topic.Title == title);
        Assert.NotNull(result);
        Assert.Equal(title, result.Title);
        Assert.Equal(description, result.Description);
        Assert.Equal(loggedInUserName, result.Presenter.UserName);
    }

    [Fact]
    public async void Test_update_GIVEN_non_existing_id_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        var updatedTopic = new TopicDto("title", "description");
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);
        var nonExistingId = "the original topic does not exist";

        async Task Action()
        {
            await service.UpdateTopic(nonExistingId, updatedTopic, loggedInUserName);
        }

        var exception = await Assert.ThrowsAsync<Exception>(Action);
        Assert.Equal("Invalid Topic Id", exception.Message);
    }

    [Fact]
    public async void Test_update_GIVEN_UserName_different_from_creator_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        var updatedTopic = new TopicDto("updated title", "updated description");
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, new List<string>([loggedInUserName]), repository);
        var originalTopic = await service.AddTopic(new TopicDto("original title", ""), "anotherUserName");
        var existingId = originalTopic.Id;

        async Task Action()
        {
            await service.UpdateTopic(existingId, updatedTopic, loggedInUserName);
        }

        var exception = await Assert.ThrowsAsync<Exception>(Action);
        Assert.Equal("This Topic does not belong to you. Not allowed to update", exception.Message);
    }


    [Fact]
    public async void Test_update_GIVEN_authorized_user_and_existing_topic_WHEN_passing_with_new_values_THEN_update()
    {
        const string loggedInUserName = "Fake User";
        var updatedTopic = new TopicDto("updated title", "updated description");
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new TopicRepository(dbContext);
        var service = await GetService(dbContext, [loggedInUserName], repository);
        var originalTopic = await service.AddTopic(new TopicDto("original title", ""), loggedInUserName);
        var existingId = originalTopic.Id;

        await service.UpdateTopic(existingId, updatedTopic, loggedInUserName);

        var result = await repository.FetchBy(existingId)!;
        Assert.Equal(updatedTopic.Title, result.Title);
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