using System.Security.Claims;
using Gemeinschaftsgipfel.Controllers.DTOs;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Services;

public class AuthServiceTest
{
    private readonly string _defaultPassword;
    private readonly string _defaultUserName;
    private readonly Mock<ILogger<AuthService>> _logger;
    private readonly string _theRightEntrySecret;

    public AuthServiceTest()
    {
        _theRightEntrySecret = Environment.GetEnvironmentVariable("USER_CREATION_ENTRY_SECRET") ??
                               "The alternative Passphrase which only get loaded when there is no .env present.";
        TestHelper.ReadTestEnv();
        Environment.SetEnvironmentVariable("USER_CREATION_ENTRY_SECRET", _theRightEntrySecret);
        _defaultPassword = "Complex enough!1#";
        _defaultUserName = "username";
        _logger = new Mock<ILogger<AuthService>>();
    }

    [Fact]
    public async Task Test_SignUp_GIVEN_correct_user_input_THEN_calling_UserManager_create_AND_logger()
    {
        var userManager = TestHelper.GetMockUserManager();
        var service = new AuthService(userManager.Object, new PasswordHasher<User>(),
            TestHelper.GetJwtGenerationService().Object, _logger.Object);
        var userInput = new SignupDto(_defaultUserName, _defaultPassword, _theRightEntrySecret);

        await service.SignUp(userInput);

        userManager.Verify(c => c.CreateAsync(It.IsAny<User>(), _defaultPassword), Times.Once);
    }


    [Fact]
    public async Task Test_SignUp_GIVEN_correct_user_input_THEN_creating_user()
    {
        var userManager = TestHelper.GetIntegrationInMemoryUserManager();
        var service = new AuthService(userManager, new PasswordHasher<User>(),
            TestHelper.GetJwtGenerationService().Object, _logger.Object);
        var userInput = new SignupDto(_defaultUserName, _defaultPassword, _theRightEntrySecret);


        var result = await service.SignUp(userInput);

        Assert.True(result.Succeeded);
        Assert.Equal(1, userManager.Users.Count());
    }

    [Fact]
    public async Task Test_SignUp_GIVEN_wrong_passphrase_THEN_add_to_error()
    {
        var userManager = TestHelper.GetMockUserManager();
        var service = new AuthService(userManager.Object, new PasswordHasher<User>(),
            TestHelper.GetJwtGenerationService().Object, _logger.Object);
        var userInput = new SignupDto(_defaultUserName, _defaultPassword, "wrong passphrase");

        var result = await service.SignUp(userInput);

        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
        Assert.Equal("WrongPassphrase", result.Errors.ToArray()[0].Code);
        userManager.Verify(m => m.CreateAsync(It.IsAny<User>()), Times.Never());
    }

    [Fact]
    public async Task Test_SignUp_GIVEN_empty_user_name_THEN_return_unsuccessful()
    {
        var userManager = TestHelper.GetIntegrationInMemoryUserManager();
        var service = new AuthService(userManager, new PasswordHasher<User>(),
            TestHelper.GetJwtGenerationService().Object, _logger.Object);
        var userInput = new SignupDto("", _defaultPassword,
            Environment.GetEnvironmentVariable("USER_CREATION_ENTRY_SECRET"));

        var result = await service.SignUp(userInput);

        Assert.Single(result.Errors);
        Assert.Equal("InvalidUserName", result.Errors.ToArray()[0].Code);
        Assert.Equal(0, userManager.Users.Count());
    }

    [Fact]
    public async Task Test_Login_GIVEN_non_signed_up_user_THEN_return_empty_string()
    {
        var userManager = TestHelper.GetIntegrationInMemoryUserManager();
        var jwtGenerationService = TestHelper.GetJwtGenerationService();
        var service = new AuthService(userManager, new PasswordHasher<User>(), jwtGenerationService.Object,
            _logger.Object);
        var userInput = new LoginDto
        (
            "non-existing UserName",
            "Complex enough!1#"
        );

        var result = await service.Login(userInput);

        Assert.Empty(result);
        AssertThatNoJwtGotGenerated(jwtGenerationService);
    }

    private static void AssertThatNoJwtGotGenerated(Mock<JwtGenerationService> jwtGenerationService)
    {
        jwtGenerationService.Verify(c => c.Generate(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task Test_Login_GIVEN_signed_up_user_THEN_return_jwt()
    {
        var userManager = TestHelper.GetIntegrationInMemoryUserManager();
        var passwordHasher = TestHelper.GetMockPasswordHasher();
        var expectedJwt = "expected JWT";
        var jwtGenerationService = TestHelper.GetJwtGenerationService(expectedJwt);
        var service = new AuthService(userManager, passwordHasher.Object, jwtGenerationService.Object, _logger.Object);
        await service.SignUp(new SignupDto(_defaultUserName, _defaultPassword, _theRightEntrySecret));
        var userInput = new LoginDto
        (
            _defaultUserName,
            _defaultPassword
        );

        var result = await service.Login(userInput);

        Assert.Equal(expectedJwt, result);
        AssertThatUserNameIsAddedAsClaim(jwtGenerationService);
    }

    [Fact]
    public async Task Test_changeUserPassword_GIVEN_user_not_super_admin_THEN_return_unsuccessful()
    {
        const string loggedInUserName = "is not dummyAdmin";
        var userManager = TestHelper.GetMockUserManager();
        var service = new AuthService(userManager.Object, new PasswordHasher<User>(),
            TestHelper.GetJwtGenerationService().Object, _logger.Object);
        var userInput = new SignupDto(_defaultUserName, _defaultPassword, _theRightEntrySecret);

        var result = await service.ChangeUserPassword(userInput, loggedInUserName);

        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
        Assert.Equal("Unauthorized", result.Errors.ToArray()[0].Code);
        userManager.Verify(m => m.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never());
    }

    [Fact]
    public async Task Test_changeUserPassword_GIVEN_correct_input_THEN_return_successful()
    {
        var loggedInUserName = Environment.GetEnvironmentVariable("ADMIN_USER_NAME")!;
        var userManager = TestHelper.GetIntegrationInMemoryUserManager();
        var service = new AuthService(userManager, new PasswordHasher<User>(),
            TestHelper.GetJwtGenerationService().Object, _logger.Object);
        var initialCredentials = new SignupDto(_defaultUserName, "forgotten password!3Jde$5tg", _theRightEntrySecret);
        await service.SignUp(initialCredentials);
        var userInput = new SignupDto(_defaultUserName, _defaultPassword, _theRightEntrySecret);

        var result = await service.ChangeUserPassword(userInput, loggedInUserName);

        Assert.True(result.Succeeded);
        var hasLoginResultWithNewCredentials = await service.Login(new LoginDto(_defaultUserName, userInput.Password));
        Assert.False(hasLoginResultWithNewCredentials == "");
    }

    private void AssertThatUserNameIsAddedAsClaim(Mock<JwtGenerationService> jwtGenerationService)
    {
        jwtGenerationService.Verify(
            c => c.Generate(It.Is<IEnumerable<Claim>>(claims =>
                claims.Any(x => x.Type == ClaimTypes.Name && x.Value == _defaultUserName))), Times.Once);
    }
}