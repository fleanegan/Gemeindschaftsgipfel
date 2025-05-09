using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Models;

namespace Gemeinschaftsgipfel.Services;

public interface ITopicService
{
    Task<Topic> AddTopic(TopicCreationDto toBeAdded, string userName);
    Task RemoveTopic(string topicId, string loggedInUserName);
    Task<Topic> GetTopicById(string topicId);
    Task<Topic> UpdateTopic(TopicUpdateDto updatedTopic, string loggedInUserName);
    Task<IEnumerable<Topic>> FetchAllExceptLoggedIn(string loggedInUserName);
    Task<IEnumerable<Topic>> FetchAllOfLoggedIn(string loggedInUserName);
    Task AddTopicVote(string topicId, string loggedInUserName);
    Task RemoveTopicVote(string topicId, string loggedInUserName);
    Task AddForumPostToTopic(string topicId, string content, string userName);
    Task<IEnumerable<ForumPost>> GetForumPostsForTopic(string topicId);
}