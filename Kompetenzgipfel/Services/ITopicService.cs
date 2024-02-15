using Kompetenzgipfel.Models;

namespace Kompetenzgipfel.Services;

public interface ITopicService
{
    Task<Topic> AddTopic(Topic toBeAdded);
    Task<string> GetTopicsByPresenterId();
}