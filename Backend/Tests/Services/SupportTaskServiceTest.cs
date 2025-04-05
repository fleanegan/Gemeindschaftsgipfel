using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Exceptions;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Repositories;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;

namespace Tests.Services;

public class SupportTaskServiceTest
{
    [Fact]
    public async Task Test_add_GIVEN_user_which_is_not_admin_THEN_throw_exception()
    {
        const string loggedInUserName = "is not dummyAdmin";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);
        var userInput = new SupportTaskCreationDto("title", "description", "from day x to day y", 4);

        async Task Action()
        {
            await service.AddTask(userInput, loggedInUserName);
        }

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Action);
        Assert.Contains(loggedInUserName, exception.Message);
    }
    
    [Fact]
    public async Task Test_add_GIVEN_correct_input_THEN_store_in_db()
    {
        var loggedInUserName = Environment.GetEnvironmentVariable("ADMIN_USER_NAME")!;
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);
        var userInput = new SupportTaskCreationDto("title", "description", "from day x to day y", 4);

        await service.AddTask(userInput, loggedInUserName);

        var result = (await repository.FetchAll()).ToArray()[0];
        Assert.NotNull(result);
        Assert.Equal(userInput.Title, result.Title);
        Assert.Equal(userInput.Description, result.Description);
        Assert.Equal(userInput.Duration, result.Duration);
    }

    [Fact]
    public async Task Test_modify_GIVEN_user_which_is_not_admin_THEN_throw_exception()
    {
        const string loggedInUserName = "is not dummyAdmin";
        const string existingSupportTaskId = "existing Id";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);
        var userInput = new SupportTaskCreationDto("title", "description", "from day x to day y", 4);

        async Task Action()
        {
            await service.ModifyTask(userInput, existingSupportTaskId, loggedInUserName);
        }

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Action);
        Assert.Contains(loggedInUserName, exception.Message);
    }

    [Fact]
    public async Task Test_modify_GIVEN_correct_input_THEN_store_in_db()
    {
        var loggedInUserName = Environment.GetEnvironmentVariable("ADMIN_USER_NAME")!;
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);
        var old = await service.AddTask(new SupportTaskCreationDto("title", "description", "from day x to day y", 4), loggedInUserName);
        var modified = new SupportTaskCreationDto("new title", "new description", "new duration", 3);


        await service.ModifyTask(modified, old.Id, loggedInUserName);
        
        var result = (await repository.FetchAll()).ToArray()[0];
        Assert.NotNull(result);
        Assert.Equal(modified.Title, result.Title);
        Assert.Equal(modified.Description, result.Description);
        Assert.Equal(modified.RequiredSupporters, result.RequiredSupporters);
        Assert.Equal(modified.Duration, result.Duration);
    }

    [Fact]
    public async Task Test_commitToSupportTask_GIVEN_non_existing_SupportTask_id_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        const string supportTaskId = "there is no such SupportTask";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);

        async Task Action()
        {
            await service.CommitToSupportTask(supportTaskId, loggedInUserName);
        }

        var exception = await Assert.ThrowsAsync<SupportTaskNotFoundException>(Action);
        Assert.Contains(supportTaskId, exception.Message);
    }

    [Fact]
    public async Task Test_commitToSupportTask_GIVEN_existing_SupportTask_id_THEN_add_SupportPromise()
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);
        var supportTaskId = await GivenAddedSupportTask(service);

        await service.CommitToSupportTask(supportTaskId, loggedInUserName);

        var supportTasks = (await repository.FetchAll()).ToArray();
        Assert.Single(supportTasks.First().SupportPromises);
        var supportPromises = supportTasks.First().SupportPromises;
        Assert.Single(supportPromises);
        Assert.Equal(loggedInUserName, supportPromises.First().Supporter.UserName);
        Assert.Equal(supportPromises.First().SupportTask.Id, supportTaskId);
    }

    [Fact]
    public async Task Test_commitToSupportTask_GIVEN_multiple_submits_THEN_add_only_once()
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);
        var supportTaskId = await GivenAddedSupportTask(service);

        await service.CommitToSupportTask(supportTaskId, loggedInUserName);

        async Task Action()
        {
            await service.CommitToSupportTask(supportTaskId, loggedInUserName);
        }

        await Assert.ThrowsAsync<SupportPromiseImpossibleException>(Action);
        var supportTasks = (await repository.FetchAll()).ToArray();
        Assert.Single(supportTasks.First().SupportPromises);
    }

    [Fact]
    public async Task Test_resignFromSupportTask_GIVEN_non_existing_SupportTaskId_THEN_throw_error()
    {
        const string loggedInUserName = "Fake User";
        const string supportTaskId = "there is no such SupportTask";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);

        async Task Action()
        {
            await service.ResignFromSupportTask(supportTaskId, loggedInUserName);
        }

        var exception = await Assert.ThrowsAsync<SupportTaskNotFoundException>(Action);
        Assert.Contains(supportTaskId, exception.Message);
    }

    [Fact]
    public async Task Test_resignFromSupportTask_GIVEN_existing_SupportTask_id_AND_different_UserName_casing_THEN_remove_SupportPromise()
    {
        const string loggedInUserNameDuringCommitment = "Fake User";
        const string loggedInUserNameDuringResignment = "fAKE User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserNameDuringCommitment], repository);
        var supportTaskId = await GivenAddedSupportTask(service);
        await service.CommitToSupportTask(supportTaskId, loggedInUserNameDuringCommitment);

        await service.ResignFromSupportTask(supportTaskId, loggedInUserNameDuringResignment);

        var supportTasks = (await repository.FetchAll()).ToArray();
        Assert.Empty(supportTasks.First().SupportPromises);
    }

    [Fact]
    public async Task Test_resignFromSupportTask_GIVEN_existing_SupportTask_id_THEN_remove_SupportPromise()
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);
        var supportTaskId = await GivenAddedSupportTask(service);
        await service.CommitToSupportTask(supportTaskId, loggedInUserName);

        await service.ResignFromSupportTask(supportTaskId, loggedInUserName);

        var supportTasks = (await repository.FetchAll()).ToArray();
        Assert.Empty(supportTasks.First().SupportPromises);
    }

    [Fact]
    public async Task
        Test_resignFromSupportTask_GIVEN_no_SupportPromise_for_given_SupportTask_THEN_throw_exception()
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);
        var supportTaskId = await GivenAddedSupportTask(service);

        async Task Action()
        {
            await service.ResignFromSupportTask(supportTaskId, loggedInUserName);
        }

        await Assert.ThrowsAsync<SupportPromiseImpossibleException>(Action);
        var supportTasks = (await repository.FetchAll()).ToArray();
        Assert.Empty(supportTasks.First().SupportPromises);
    }

    [Fact]
    private async Task Test_getAll_GIVEN_no_SupportTasks_THEN_return_empty_Collection()
    {
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [], repository);

        var result = await service.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    private async Task Test_getAll_GIVEN_two_SupportTasks_THEN_return_Collection_with_two_entries()
    {
        const string loggedInUserName = "Fake User";
        await using var dbContext = TestHelper.GetDbContext<DatabaseContextApplication>();
        var repository = new SupportTaskRepository(dbContext);
        ISupportTaskService service = await GetService(dbContext, [loggedInUserName], repository);
        await GivenAddedSupportTask(service);
        await GivenAddedSupportTask(service);

        var result = (await service.GetAll()).ToArray();

        Assert.Equal(2, result.Length);
    }

    private static async Task<string> GivenAddedSupportTask(ISupportTaskService service)
    {
        var dummyadmin = Environment.GetEnvironmentVariable("ADMIN_USER_NAME");
        var createdSupportTask =
            await service.AddTask(new SupportTaskCreationDto("title", "description", "duration", 55), dummyadmin!);
        return createdSupportTask.Id;
    }
    
    private static async Task<SupportTaskService> GetService(DatabaseContextApplication dbContext,
        List<string> userNames,
        SupportTaskRepository repository)
    {
        var userStore = new UserStore<User>(dbContext);
        var supportPromiseRepository = new SupportPromiseRepository(dbContext);
        var userManager = await CannotInjectUserStoreDirectlySoWrappingInUserManager(userStore, userNames);
        var service = new SupportTaskService(repository, supportPromiseRepository, userManager.Object);
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
