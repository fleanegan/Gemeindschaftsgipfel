using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;
using Microsoft.AspNetCore.Identity;

namespace Kompetenzgipfel.Services;

public class TopicService(TopicRepository repository, UserManager<User> userManager)
    : ITopicService
{
    public async Task<string> GetTopicsByPresenterId()
    {
        return await Task.FromResult("original implementation");
    }

    public async Task<Topic> AddTopic(TopicCreationDto toBeAdded, string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
        }

        var newTopic = Topic.Create(toBeAdded.Title, toBeAdded.Description ?? "", user);
        return await repository.Create(newTopic);
    }

    public async Task<Topic> UpdateTopic(TopicUpdateDto updatedTopicCreationContent,
        string userName)
    {
        var topicToChange = await repository.FetchBy(updatedTopicCreationContent.Id);
        if (topicToChange == null)
            throw new Exception("Invalid Topic Id");
        if (userName != topicToChange.User.UserName)
            throw new Exception("This Topic does not belong to you. Not allowed to update");
        topicToChange.Title = updatedTopicCreationContent.Title;
        topicToChange.Description = updatedTopicCreationContent.Description;
        return await repository.Update(topicToChange);
    }

    public async Task<IEnumerable<Topic>> FetchAllExceptLoggedIn(string loggedInUserName)
    {
        return await repository.GetAllExceptForUser(loggedInUserName);
    }

    public Task<IEnumerable<Topic>> FetchAllOfLoggedIn(string loggedInUserName)
    {
        return repository.GetAllForUser(loggedInUserName);
    }
}