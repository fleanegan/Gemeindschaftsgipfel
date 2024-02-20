using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kompetenzgipfel.Controllers;

public class AuthController(AuthService authService) : Controller
{
    private readonly AuthService _authService = authService;

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginData)
    {
        var result = await authService.Login(loginData);
        if (result == "") return new UnauthorizedResult();
        return Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IdentityResult> SignUp([FromBody] SignupDto signupData)
    {
        return await authService.SignUp(signupData);
    }
}