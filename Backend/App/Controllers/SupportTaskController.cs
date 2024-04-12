using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Controllers.Helpers;
using Gemeinschaftsgipfel.Exceptions;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gemeinschaftsgipfel.Controllers;

public class SupportTaskController(ISupportTaskService service) : AbstractController
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddNew([FromBody] SupportTaskCreationDto userInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userName = GetUserNameFromAuthorization();
        await service.AddTask(userInput, userName);
        return Ok();
    }
    
    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Modify([FromBody] SupportTaskCreationDto userInput, string id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userName = GetUserNameFromAuthorization();
        await service.ModifyTask(userInput, id, userName);
        return Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Help([FromBody] SupportPromiseDto userInput)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userName = GetUserNameFromAuthorization();
        try
        {
            await service.CommitToSupportTask(userInput.SupportTaskId, userName);
        }
        catch (SupportPromiseImpossibleException e)
        {
            return Conflict(e.Message);
        }
        catch (SupportTaskNotFoundException e)
        {
            return NotFound(e.Message);
        }

        return Ok();
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Help(string id)
    {
        var userName = GetUserNameFromAuthorization();
        try
        {
            await service.ResignFromSupportTask(id, userName);
        }
        catch (SupportPromiseImpossibleException e)
        {
            return Conflict(e.Message);
        }
        catch (SupportTaskNotFoundException e)
        {
            return NotFound(e.Message);
        }

        return Ok();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await service.GetAll();
        return Ok(ResponseGenerator.GenerateSupportTaskResponses(result));
    }
}