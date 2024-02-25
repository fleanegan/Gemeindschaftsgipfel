using System.Security.Claims;
using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Properties;
using Microsoft.AspNetCore.Identity;

namespace Kompetenzgipfel.Services;

public class AuthService(
    UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher,
    JwtGenerationService jwtGenerationService)
{
    public async Task<IdentityResult> SignUp(SignupDto userInput)
    {
        var correctPassphrase = Environment.GetEnvironmentVariable("USER_CREATION_PASSPHRASE")!;
        if (!userInput.EntrySecret.Equals(correctPassphrase))
            return IdentityResult.Failed(new IdentityError
                { Code = "WrongPassphrase", Description = Constants.WrongPassphraseErrorMessage });
        return await userManager.CreateAsync(new User { UserName = userInput.UserName }, userInput.Password);
    }

    public async Task<string> Login(LoginDto userInput)
    {
        var foundUser = await userManager.FindByNameAsync(userInput.UserName);

        if (foundUser != null &&
            passwordHasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash!, userInput.Password) ==
            PasswordVerificationResult.Success)
            return jwtGenerationService.Generate(new List<Claim>([new Claim(ClaimTypes.Name, userInput.UserName)]));

        return "";
    }
}