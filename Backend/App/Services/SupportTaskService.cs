using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Exceptions;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Gemeinschaftsgipfel.Services;

public class SupportTaskService(
    SupportTaskRepository supportTaskRepository,
    SupportPromiseRepository supportPromiseRepository,
    UserManager<User> userManager) : ISupportTaskService
{
    public async Task<SupportTask> AddTask(SupportTaskCreationDto userInput, string loggedInUserName)
    {
        if (Environment.GetEnvironmentVariable("ADMIN_USER_NAME").ToLower() != loggedInUserName.ToLower())
            throw new UnauthorizedException(loggedInUserName);
        return await supportTaskRepository.Create(new SupportTask
        {
            Title = userInput.Title, Description = userInput.Description, Duration = userInput.Duration,
            RequiredSupporters = userInput.RequiredSupporters
        });
    }

    public async Task<SupportTask> ModifyTask(SupportTaskCreationDto userInput, string supportTaskId, string loggedInUserName)
    {
        if (Environment.GetEnvironmentVariable("ADMIN_USER_NAME").ToLower() != loggedInUserName.ToLower())
            throw new UnauthorizedException(loggedInUserName);
        var existingSupportTask = await supportTaskRepository.FetchBy(supportTaskId);
        existingSupportTask!.RequiredSupporters = userInput.RequiredSupporters;
        existingSupportTask.Description = userInput.Description;
        existingSupportTask.Title = userInput.Title;
        existingSupportTask.Duration = userInput.Duration;
        return await supportTaskRepository.Update(existingSupportTask);
    }

    public async Task CommitToSupportTask(string supportTaskId, string loggedInUserName)
    {
        var supportTask = await supportTaskRepository.FetchBy(supportTaskId);
        if (supportTask == null)
            throw new SupportTaskNotFoundException(supportTaskId);
        var supporter = await userManager.FindByNameAsync(loggedInUserName);
        var supportPromise = new SupportPromise(supportTask, supporter!);
        if (supportTask.SupportPromises.Any(s => s.Supporter.UserName.ToLower() == supporter!.UserName.ToLower()))
            throw new SupportPromiseImpossibleException(supportTaskId);
        await supportPromiseRepository.Create(supportPromise);
    }

    public async Task ResignFromSupportTask(string supportTaskId, string loggedInUserName)
    {
        var supportTask = await supportTaskRepository.FetchBy(supportTaskId);
        if (supportTask == null)
            throw new SupportTaskNotFoundException(supportTaskId);
        var supportPromise = supportTask.SupportPromises.FirstOrDefault(s => s.Supporter.UserName.ToLower() == loggedInUserName.ToLower());
        if (supportPromise == null)
            throw new SupportPromiseImpossibleException(supportTaskId);
        await supportPromiseRepository.Remove(supportPromise);
    }

    public async Task<IEnumerable<SupportTask>> GetAll()
    {
        return await supportTaskRepository.FetchAll();
    }
}
