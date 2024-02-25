using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kompetenzgipfel.Controllers;

public class AuthController(AuthService authService) : Controller
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
        return Ok(await authService.SignUp(signupData));
    }
}