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
        var correctPassphrase = Environment.GetEnvironmentVariable("USER_CREATION_ENTRY_SECRET")!;
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

    public async Task<IdentityResult> ChangeUserPassword(SignupDto userInput, string loggedInUserName)
    {
        if (Environment.GetEnvironmentVariable("ADMIN_USER_NAME") != loggedInUserName)
            return IdentityResult.Failed(new IdentityError { Code = "Unauthorized" });

        var user = await userManager.FindByNameAsync(userInput.UserName);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Code = "NotFound" });

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, userInput.Password);

        return result;
    }
}