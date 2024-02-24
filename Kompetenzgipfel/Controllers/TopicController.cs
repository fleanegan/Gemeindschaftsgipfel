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
    public async Task<IActionResult> AddNew([FromBody] TopicCreationDto userInput)
    {
        var userName = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Name)!.Value;
        try
        {
            var result = await service.AddTopic(userInput, userName!);

            return Ok(new TopicResponse(result.Id, result.Title, result.Description, userName));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] TopicUpdateDto userInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userName = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            var result = await service.UpdateTopic(userInput, userName);
            return Ok(new TopicResponse(result.Id, result.Title, result.Description, userName));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> AllExceptLoggedIn()
    {
        var userName = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
        var fetchAllExceptLoggedIn = await service.FetchAllExceptLoggedIn(userName!);
        var mappedList = fetchAllExceptLoggedIn
            .Select(topic => new TopicResponse(topic.Id, topic.Title, topic.Description, topic.User.UserName))
            .ToList();
        return Ok(mappedList);
    }
}