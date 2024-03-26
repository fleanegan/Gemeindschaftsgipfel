using System.Security.Claims;
using Kompetenzgipfel.Controllers.DTOs;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Properties;
using Microsoft.AspNetCore.Identity;

namespace Kompetenzgipfel.Services;

public class AuthService(
    UserManager<User> userManager,
    IPasswordHasher<User> passwordHasher,
    JwtGenerationService jwtGenerationService,
    ILogger<AuthService> logger
)
{
    public async Task<IdentityResult> SignUp(SignupDto userInput)
    {
        var correctPassphrase = Environment.GetEnvironmentVariable("USER_CREATION_ENTRY_SECRET")!;
        if (!userInput.EntrySecret.Equals(correctPassphrase))
        {
            logger.LogWarning("LOGIN_REGISTER: [WRONG_ENTRY_SECRET] someone failed to create a new user");
            return IdentityResult.Failed(new IdentityError
                { Code = "WrongPassphrase", Description = Constants.WrongPassphraseErrorMessage });
        }

        logger.LogInformation("LOGIN_REGISTER: [USER_CREATED] with username " + userInput.UserName);
        return await userManager.CreateAsync(new User { UserName = userInput.UserName }, userInput.Password);
    }

    public async Task<string> Login(LoginDto userInput)
    {
        var foundUser = await userManager.FindByNameAsync(userInput.UserName);

        if (foundUser != null &&
            passwordHasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash!, userInput.Password) ==
            PasswordVerificationResult.Success)
        {
            logger.LogInformation("LOGIN_REGISTER: " + userInput.UserName + " successfully logged in");
            return jwtGenerationService.Generate(new List<Claim>([new Claim(ClaimTypes.Name, userInput.UserName)]));
        }

        logger.LogWarning("LOGIN_REGISTER: [LOGIN_FAILURE] username: " + userInput.UserName +
                          " failed to log in, password length: " + userInput.Password.Length);

        return "";
    }

    public async Task<IdentityResult> ChangeUserPassword(SignupDto userInput, string loggedInUserName)
    {
        logger.LogInformation("LOGIN_REGISTER: user " + loggedInUserName + " requested a password change for user " +
                              userInput.UserName);
        if (Environment.GetEnvironmentVariable("ADMIN_USER_NAME") != loggedInUserName)
        {
            logger.LogWarning("LOGIN_REGISTER: user " + loggedInUserName +
                              " was not allowed to change the password of user " + userInput.UserName);
            return IdentityResult.Failed(new IdentityError { Code = "Unauthorized" });
        }

        var user = await userManager.FindByNameAsync(userInput.UserName);
        if (user == null) return IdentityResult.Failed(new IdentityError { Code = "NotFound" });

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, userInput.Password);
        logger.LogInformation("LOGIN_REGISTER: user " + loggedInUserName + " changed the password for user " +
                              userInput.UserName);

        return result;
    }
}