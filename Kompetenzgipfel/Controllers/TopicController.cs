using Kompetenzgipfel.Models;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kompetenzgipfel.Controllers;

public class TopicController(ILogger<TopicController> logger, ITopicService service) : Controller
{
    private readonly ILogger<TopicController> _logger = logger;

    [HttpGet]
    public string All()
    {
        return "get response";
    }

    [HttpPost]
    public string One()
    {
        return "post response";
    }

    [HttpGet]
    public async Task<string> GetById()
    {
        return await service.GetTopicsByPresenterId();
    }

    [HttpPost]
    public async Task<ActionResult> AddNew([FromBody] TopicDto toBeAdded)
    {
        try
        {
            var topic = Topic.Create(toBeAdded.Title, toBeAdded.Description);
            return Ok(await service.AddTopic(topic));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
}