using Kompetenzgipfel.Controllers;
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
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var userName = "Fake User";
        var repository = new TopicRepository(dbContext);
        var userStore = new UserStore<User>(dbContext);
        var userManager = await CannotInjectUserStoreDirectlySoWrappingInUserManager(userStore, userName);
        var service = new TopicService(repository, userManager.Object);

        await service.AddTopic(new TopicDto(description, title), userName);

        var result = repository.GetAll().FirstOrDefault(topic => topic.Title == title);
        Assert.NotNull(result);
        Assert.Equal(title, result.Title);
        Assert.Equal(description, result.Description);
        Assert.Equal(userName, result.Presenter.UserName);
    }

    private static async Task<Mock<UserManager<User>>> CannotInjectUserStoreDirectlySoWrappingInUserManager(
        UserStore<User> userStore, string userName)
    {
        await userStore.CreateAsync(new User
            { AccessFailedCount = 0, UserName = userName, LockoutEnabled = false, EmailConfirmed = false });
        var userManager = TestHelper.GetMockUserManager(userStore);
        userManager.Setup(c => c.FindByNameAsync(It.IsAny<string>()))
            .Returns(() => Task.FromResult(new User { UserName = userName })!);
        return userManager;
    }
}