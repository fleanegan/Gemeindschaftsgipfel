using Kompetenzgipfel.Models;

namespace Kompetenzgipfel.Services;

public class TopicService : ITopicService
{
    private readonly TopicRepository _repository;

    public TopicService(TopicRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> GetTopicsByPresenterId()
    {
        return await Task.FromResult("original implementation");
    }

    public async Task<Topic> AddTopic(Topic toBeAdded)
    {
        return await _repository.Create(toBeAdded);
    }
}