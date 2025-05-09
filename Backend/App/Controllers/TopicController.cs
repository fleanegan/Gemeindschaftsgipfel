using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Controllers.Helpers;
using Gemeinschaftsgipfel.Exceptions;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gemeinschaftsgipfel.Controllers;

public class TopicController(ITopicService service) : AbstractController
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddNew([FromBody] TopicCreationDto userInput)
    {
        var userName = GetUserNameFromAuthorization();
        try
        {
            var result = await service.AddTopic(userInput, userName);

            return Ok(new OwnTopicResponseModel(result.Id, result.Title, result.PresentationTimeInMinutes, result.Description, userName, 0));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetOne(string id)
    {
        try
        {
            var topicById = await service.GetTopicById(id);
            return Ok(new ForeignTopicResponseModel(topicById.Id, topicById.Title, topicById.PresentationTimeInMinutes, topicById.Description,
                topicById.User.UserName, false));
        }
        catch (TopicNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var userName = GetUserNameFromAuthorization();
        try
        {
            await service.RemoveTopic(id, userName);
        }
        catch (TopicNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (UnauthorizedTopicModificationException)
        {
            return Forbid(new AuthenticationProperties());
        }

        return Ok();
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
            return Ok(new OwnTopicResponseModel(result.Id, result.Title, result.PresentationTimeInMinutes, result.Description, userName,
                result.Votes.Count));
        }
        catch (TopicNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (UnauthorizedTopicModificationException)
        {
            return Forbid(new AuthenticationProperties());
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> AllExceptLoggedIn()
    {
        var userName = GetUserNameFromAuthorization();
        var result = await service.FetchAllExceptLoggedIn(userName);
        var response = ResponseGenerator.GenerateForeignTopicResponses(result, userName);
        return Ok(response);
    }

    [Authorize]
    public async Task<IActionResult> AllOfLoggedIn()
    {
        var userName = GetUserNameFromAuthorization();
        var result = await service.FetchAllOfLoggedIn(userName);
        var response = ResponseGenerator.GenerateOwnTopicResponses(result);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddVote([FromBody] TopicVoteDto userInput)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        var userName = GetUserNameFromAuthorization();
        try
        {
            await service.AddTopicVote(userInput.topicId, userName);
            return Ok();
        }
        catch (VoteImpossibleException e)
        {
            return Conflict(e.Message);
        }
        catch (TopicNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> RemoveVote(string id)
    {
        var userName = GetUserNameFromAuthorization();
        try
        {
            await service.RemoveTopicVote(id, userName);
            return Ok();
        }
        catch (VoteImpossibleException e)
        {
            return Conflict(e.Message);
        }
        catch (TopicNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AttachForumPost([FromBody] TopicForumPostDto userInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userName = GetUserNameFromAuthorization();
        try
        {
            await service.AddForumPostToTopic(userInput.topicId, userInput.content, userName);
            return Ok();
        }
        catch (TopicNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPosts(string topicId)
    {
        try
        {
            var posts = await service.GetForumPostsForTopic(topicId);
            var postsResponse = posts.Select(p => new ForumPostResponseModel(
                p.Id,
                p.Content,
                p.Creator.UserName,
                p.CreatedAt));
            
            return Ok(postsResponse);
        }
        catch (TopicNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
