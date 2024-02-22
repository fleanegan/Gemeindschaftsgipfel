using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;

namespace Kompetenzgipfel.Services;

public interface ITopicService
{
    Task<Topic?> AddTopic(TopicDto toBeAdded, string userName);
    Task<string> GetTopicsByPresenterId();
}