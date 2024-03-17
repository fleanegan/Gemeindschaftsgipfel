using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Controllers.Helpers;
using Kompetenzgipfel.Exceptions;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kompetenzgipfel.Controllers;

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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Help([FromBody] SupportPromiseDto userInput)
    {
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
}