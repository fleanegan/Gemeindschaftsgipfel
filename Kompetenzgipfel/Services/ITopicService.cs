using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;

namespace Kompetenzgipfel.Services;

public interface ITopicService
{
    Task<Topic?> AddTopic(TopicCreationDto toBeAdded, string userName);
    Task<string> GetTopicsByPresenterId();
    Task<Topic> UpdateTopic(TopicUpdateDto updatedTopic, string loggedInUserName);
    Task<IEnumerable<Topic>> FetchAllExceptLoggedIn(string loggedInUserName);
    Task<IEnumerable<Topic>> FetchAllOfLoggedIn(string loggedInUserName);
    public Task AddTopicVote(string topicId, string loggedInUserName);
    public Task RemoveTopicVote(string topicId, string loggedInUserName);
}