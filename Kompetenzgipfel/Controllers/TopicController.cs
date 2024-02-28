using System.Security.Claims;
using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Controllers.Helpers;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kompetenzgipfel.Controllers;

public class TopicController(ITopicService service) : Controller
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddNew([FromBody] TopicCreationDto userInput)
    {
        Console.WriteLine("getting a new message\n\n\n\n\nfor adding a new topic");
        var userName = GetUserNameFromAuthorization();
        try
        {
            var result = await service.AddTopic(userInput, userName!);

            return Ok(new OwnTopicResponseModel(result.Id, result.Title, result.Description, userName, 0));
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
            return Ok(new OwnTopicResponseModel(result.Id, result.Title, result.Description, userName,
                result.Votes.Count));
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
        var response = ResponseGenerator.GenerateForeignTopicResponses(result);
        return Ok(response);
    }

    [Authorize]
    public async Task<IActionResult> AllOfLoggedIn()
    {
        var userName = GetUserNameFromAuthorization()!;
        var result = await service.FetchAllOfLoggedIn(userName);
        var response = ResponseGenerator.GenerateOwnTopicResponses(result);
        return Ok(response);
    }

    [Authorize]
    public async Task<IActionResult> AddVote([FromBody] TopicVoteDto userInput)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        var userName = GetUserNameFromAuthorization();
        await service.AddTopicVote(userInput.topicId, userName);
        return Ok();
    }


    [Authorize]
    public async Task<IActionResult> RemoveVote([FromBody] TopicVoteDto userInput)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        var userName = GetUserNameFromAuthorization();
        await service.RemoveTopicVote(userInput.topicId, userName);
        return Ok();
    }

    private string GetUserNameFromAuthorization()
    {
        return HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
    }
}