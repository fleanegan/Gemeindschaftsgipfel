using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Controllers.Helpers;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Kompetenzgipfel.Controllers;

[EnableRateLimiting("fixed")]
public class AuthController(AuthService authService) : AbstractController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await authService.Login(loginData);
        if (result == "") return new UnauthorizedResult();
        return Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignupDto signupData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var identityResult = await authService.SignUp(signupData);
        if (identityResult.Succeeded)
            return Ok(identityResult);
        return BadRequest(identityResult.Errors);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangeUserPassword([FromBody] SignupDto changeData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var loggedInUserName = GetUserNameFromAuthorization();
        var result = await authService.ChangeUserPassword(changeData, loggedInUserName);
        if (result.Succeeded)
            return Ok(result);
        return Unauthorized(result);
    }
}