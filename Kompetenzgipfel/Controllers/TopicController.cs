using System.Security.Claims;
using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<ActionResult> AddNew([FromBody] TopicDto toBeAdded)
    {
        try
        {
            var userName = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
            var result = await service.AddTopic(toBeAdded, userName);

            return Ok(new TopicCreationResponse(result.Title, result.Description, userName));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
}