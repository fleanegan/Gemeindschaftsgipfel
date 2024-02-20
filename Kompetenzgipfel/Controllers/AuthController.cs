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
    public async Task<string> Login([FromBody] LoginDto loginData)
    {
        return await authService.Login(loginData);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IdentityResult> SignUp([FromBody] SignupDto signupData)
    {
        return await authService.SignUp(signupData);
    }
}