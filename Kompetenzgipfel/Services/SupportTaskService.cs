using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Exceptions;
using Kompetenzgipfel.Models;
using Microsoft.AspNetCore.Identity;

namespace Kompetenzgipfel.Services;

public class SupportTaskService(
    SupportTaskRepository supportTaskRepository,
    SupportPromiseRepository supportPromiseRepository,
    UserManager<User> userManager) : ISupportTaskService
{
    public async Task<SupportTask> AddTask(SupportTaskCreationDto userInput, string loggedIdUserName)
    {
        if (Environment.GetEnvironmentVariable("ADMIN_USER_NAME") != loggedIdUserName)
            throw new UnauthorizedException(loggedIdUserName);
        return await supportTaskRepository.Create(new SupportTask
        {
            Title = userInput.Title, Description = userInput.Description, Duration = userInput.Duration,
            RequiredSupporters = userInput.RequiredSupporters
        });
    }

    public async Task CommitToSupportTask(string supportTaskId, string loggedInUserName)
    {
        var supportTask = await supportTaskRepository.FetchBy(supportTaskId);
        if (supportTask == null)
            throw new SupportTaskNotFoundException(supportTaskId);
        var supporter = await userManager.FindByNameAsync(loggedInUserName);
        var supportPromise = new SupportPromise(supportTask, supporter!);
        if (supportTask.SupportPromises.Any(s => s.Supporter.UserName == supporter!.UserName))
            throw new SupportPromiseImpossibleException(supportTaskId);
        await supportPromiseRepository.Create(supportPromise);
    }

    public async Task ResignFromSupportTask(string supportTaskId, string loggedInUserName)
    {
        var supportTask = await supportTaskRepository.FetchBy(supportTaskId);
        if (supportTask == null)
            throw new SupportTaskNotFoundException(supportTaskId);
        var supportPromise = supportTask.SupportPromises.FirstOrDefault(s => s.Supporter.UserName == loggedInUserName);
        if (supportPromise == null)
            throw new SupportPromiseImpossibleException(supportTaskId);
        await supportPromiseRepository.Remove(supportPromise);
    }
}