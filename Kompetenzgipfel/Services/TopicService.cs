using Kompetenzgipfel.Controllers;
using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;
using Microsoft.AspNetCore.Identity;

namespace Kompetenzgipfel.Services;

public class TopicService : ITopicService
{
    private readonly TopicRepository _repository;
    private readonly UserManager<User> _userManager;

    public TopicService(
        TopicRepository repository
        , UserManager<User> userManager
    )
    {
        _repository = repository;
        _userManager = userManager;
    }

    public async Task<string> GetTopicsByPresenterId()
    {
        return await Task.FromResult("original implementation");
    }

    public async Task<Topic> AddTopic(TopicDto toBeAdded, string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        var newTopic = Topic.Create(toBeAdded.Title, toBeAdded.Description, user);
        return await _repository.Create(newTopic);
    }
}