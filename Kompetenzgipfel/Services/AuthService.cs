using System.Security.Claims;
using Kompetenzgipfel.Controllers;
using Kompetenzgipfel.Models;
using Microsoft.AspNetCore.Identity;

namespace Kompetenzgipfel.Services;

public class AuthService
{
    private readonly JwtGenerationService _jwtGenerationService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly UserManager<User> _userManager;

    public AuthService(UserManager<User> userManager, IPasswordHasher<User> passwordHasher,
        JwtGenerationService jwtGenerationService)
    {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _jwtGenerationService = jwtGenerationService;
    }

    public async Task<IdentityResult> SignUp(SignupDto userInput)
    {
        var correctPassphrase = Environment.GetEnvironmentVariable("USER_CREATION_PASSPHRASE")!;
        if (userInput.Passphrase == null || !userInput.Passphrase.Equals(correctPassphrase))
            return IdentityResult.Failed(new IdentityError
            {
                Code = "WrongPassphrase",
                Description =
                    "Falsches Eintrittsgeheimnis. Stell sicher, dass du den richtigen Satz aus der Einladungsnachricht kopierst"
            });
        return await _userManager.CreateAsync(new User { UserName = userInput.UserName }, userInput.Password);
    }

    public async Task<string> Login(LoginDto userInput)
    {
        var foundUser = await _userManager.FindByNameAsync(userInput.UserName);

        if (foundUser != null &&
            _passwordHasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash, userInput.Password) ==
            PasswordVerificationResult.Success)
            return _jwtGenerationService.Generate(new List<Claim>([new Claim(ClaimTypes.Name, userInput.UserName)]));

        return "";
    }
}