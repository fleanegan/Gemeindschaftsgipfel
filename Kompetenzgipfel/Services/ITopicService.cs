using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;

namespace Kompetenzgipfel.Services;

public interface ITopicService
{
    Task<Topic?> AddTopic(TopicCreationDto toBeAdded, string userName);
    Task RemoveTopic(string topicId, string loggedInUserName);
    Task<string> GetTopicsByPresenterId();
    Task<Topic> UpdateTopic(TopicUpdateDto updatedTopic, string loggedInUserName);
    Task<IEnumerable<Topic>> FetchAllExceptLoggedIn(string loggedInUserName);
    Task<IEnumerable<Topic>> FetchAllOfLoggedIn(string loggedInUserName);
    Task AddTopicVote(string topicId, string loggedInUserName);
    Task RemoveTopicVote(string topicId, string loggedInUserName);
}