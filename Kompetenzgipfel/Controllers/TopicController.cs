using System.Security.Claims;
using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;
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
        Console.WriteLine("getting a new message\n\n\n\n\nfor adding a new topic");
        var userName = GetUserNameFromAuthorization();
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
        var userName = GetUserNameFromAuthorization();
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
        var userName = GetUserNameFromAuthorization();
        var result = await service.FetchAllExceptLoggedIn(userName!);
        var response = GenerateTopicResponses(result);
        return Ok(response);
    }

    [Authorize]
    public async Task<IActionResult> AllOfLoggedIn()
    {
        var userName = GetUserNameFromAuthorization()!;
        var result = await service.FetchAllOfLoggedIn(userName);
        var response = GenerateTopicResponses(result);
        return Ok(response);
    }

    private static List<TopicResponse> GenerateTopicResponses(IEnumerable<Topic> fetchAllExceptLoggedIn)
    {
        return fetchAllExceptLoggedIn
            .Select(topic => new TopicResponse(topic.Id, topic.Title, topic.Description, topic.User.UserName))
            .ToList();
    }

    private string GetUserNameFromAuthorization()
    {
        return HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
    }
}