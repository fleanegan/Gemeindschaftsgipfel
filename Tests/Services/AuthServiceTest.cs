using System.Security.Claims;
using Kompetenzgipfel.Controllers;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Tests.Services;

public class AuthServiceTest
{
    private readonly string _defaultPassword;
    private readonly string _defaultUserName;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _theRightPassphrase;

    public AuthServiceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _theRightPassphrase = "the right passphrase";
        Environment.SetEnvironmentVariable("USER_CREATION_PASSPHRASE", _theRightPassphrase);
        _defaultPassword = "C0mplex$nough";
        _defaultUserName = "username";
    }

    [Fact]
    public async Task Test_SignUp_GIVEN_correct_user_input_THEN_hashing_user_password()
    {
        var userManager = GetMockUserManager();
        var service = new AuthService(userManager.Object, new PasswordHasher<User>(), GetJwtGenerationService().Object);
        var userInput = new SignupDto
        {
            Passphrase = _theRightPassphrase,
            UserName = _defaultUserName,
            Password = _defaultPassword
        };

        await service.SignUp(userInput);

        userManager.Verify(c => c.CreateAsync(It.IsAny<User>(), _defaultPassword), Times.Once);
    }


    [Fact]
    public async Task Test_SignUp_GIVEN_correct_user_input_THEN_creating_user()
    {
        var userManager = GetIntegrationInMemoryUserManager();
        var service = new AuthService(userManager, new PasswordHasher<User>(), GetJwtGenerationService().Object);
        var userInput = new SignupDto
        {
            Passphrase = _theRightPassphrase,
            UserName = _defaultUserName,
            Password = _defaultPassword
        };


        var result = await service.SignUp(userInput);

        Assert.True(result.Succeeded);
        Assert.Equal(1, userManager.Users.Count());
    }

    [Fact]
    public async Task Test_SignUp_GIVEN_wrong_passphrase_THEN_add_to_error()
    {
        var userManager = GetMockUserManager();
        var service = new AuthService(userManager.Object, new PasswordHasher<User>(), GetJwtGenerationService().Object);
        var userInput = new SignupDto
        {
            Passphrase = "wrong passphrase",
            UserName = _defaultUserName,
            Password = _defaultPassword
        };

        var result = await service.SignUp(userInput);

        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
        Assert.Equal("WrongPassphrase", result.Errors.ToArray()[0].Code);
        userManager.Verify(m => m.CreateAsync(It.IsAny<User>()), Times.Never());
    }

    [Fact]
    public async Task Test_SignUp_GIVEN_empty_user_name_THEN_return_unsuccessful()
    {
        var userManager = GetIntegrationInMemoryUserManager();
        var service = new AuthService(userManager, new PasswordHasher<User>(), GetJwtGenerationService().Object);
        var userInput = new SignupDto
        {
            Passphrase = Environment.GetEnvironmentVariable("USER_CREATION_PASSPHRASE"),
            UserName = "",
            Password = _defaultPassword
        };

        var result = await service.SignUp(userInput);

        Assert.Single(result.Errors);
        Assert.Equal("InvalidUserName", result.Errors.ToArray()[0].Code);
        Assert.Equal(0, userManager.Users.Count());
    }

    [Fact]
    public async Task Test_SignUp_GIVEN_malformed_user_input_THEN_return_unsuccessful()
    {
        var userManager = GetIntegrationInMemoryUserManager();
        var service = new AuthService(userManager, new PasswordHasher<User>(), GetJwtGenerationService().Object);
        var userInput = new SignupDto
        {
            Passphrase = Environment.GetEnvironmentVariable("USER_CREATION_PASSPHRASE"),
            Password = _defaultPassword
        };

        var result = await service.SignUp(userInput);

        Assert.Single(result.Errors);
        Assert.Equal("InvalidUserName", result.Errors.ToArray()[0].Code);
        Assert.Equal(0, userManager.Users.Count());
    }

    [Fact]
    public async Task Test_Login_GIVEN_non_signed_up_user_THEN_return_empty_string()
    {
        var userManager = GetIntegrationInMemoryUserManager();
        var jwtGenerationService = GetJwtGenerationService();
        var service = new AuthService(userManager, new PasswordHasher<User>(), jwtGenerationService.Object);
        var userInput = new LoginDto
        {
            UserName = "non-existing UserName",
            Password = "Cmplex$nough"
        };

        var result = await service.Login(userInput);

        Assert.Empty(result);
        jwtGenerationService.Verify(c => c.Generate(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task Test_Login_GIVEN_signed_up_user_THEN_return_jwt()
    {
        var userManager = GetIntegrationInMemoryUserManager();
        var passwordHasher = GetMockPasswordHasher();
        var expectedJwt = "expected JWT";
        var jwtGenerationService = GetJwtGenerationService(expectedJwt);
        var service = new AuthService(userManager, passwordHasher.Object, jwtGenerationService.Object);
        await service.SignUp(new SignupDto
            { Passphrase = _theRightPassphrase, UserName = _defaultUserName, Password = _defaultPassword });
        var userInput = new LoginDto
        {
            UserName = _defaultUserName,
            Password = _defaultPassword
        };

        var result = await service.Login(userInput);

        Assert.Equal(expectedJwt, result);
        jwtGenerationService.Verify(c => c.Generate(It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    private UserManager<User> GetIntegrationInMemoryUserManager()
    {
        var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
        var scope = application.Services.CreateScope();
        var userStore = new UserStore<User>(TestHelper.GetDbContext<DatabaseContextIdentityFramework>());
        var userManager = new UserManager<User>(
            userStore, // Pass a mock of IUserStore<User>
            scope.ServiceProvider.GetRequiredService<IOptions<IdentityOptions>>(), // Pass a null IConfiguration
            scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>(), // Pass IPasswordHasher<User>
            scope.ServiceProvider
                .GetRequiredService<IEnumerable<IUserValidator<User>>>(), // Pass IEnumerable<IUserValidator<User>>
            scope.ServiceProvider
                .GetRequiredService<
                    IEnumerable<IPasswordValidator<User>>>(), // Pass IEnumerable<IPasswordValidator<User>>
            scope.ServiceProvider.GetRequiredService<ILookupNormalizer>(), // Pass ILookupNormalizer
            scope.ServiceProvider.GetRequiredService<IdentityErrorDescriber>(), // Pass IdentityErrorDescriber
            scope.ServiceProvider.GetRequiredService<IServiceProvider>(), // Pass IServiceProvider
            scope.ServiceProvider.GetRequiredService<ILogger<UserManager<User>>>() // Pass ILogger<UserManager<User>>
        );
        return userManager;
    }

    private Mock<UserManager<User>> GetMockUserManager()
    {
        var userManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), // Pass a mock of IUserStore<User>
            null, // Pass a null IConfiguration
            null, // Pass a null IPasswordHasher<User>
            null, // Pass a null IEnumerable<IUserValidator<User>>
            null, // Pass a null IEnumerable<IPasswordValidator<User>>
            null, // Pass a null ILookupNormalizer
            null, // Pass a null IdentityErrorDescriber
            null, // Pass a null IServiceProvider
            null // Pass a null ILogger<UserManager<User>>
        );
        userManager.Setup(c => c.CreateAsync(It.IsAny<User>())).ReturnsAsync(() => IdentityResult.Success);
        return userManager;
    }

    private Mock<PasswordHasher<User>> GetMockPasswordHasher()
    {
        // new PasswordHasher<User>()
        var passwordHasher = new Mock<PasswordHasher<User>>(null);
        passwordHasher.Setup(c => c.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        return passwordHasher;
    }

    private Mock<JwtGenerationService> GetJwtGenerationService(string dummyJwt = "dummy_JWT")
    {
        var result = new Mock<JwtGenerationService>();
        result.Setup(c => c.Generate(It.IsAny<IEnumerable<Claim>>())).Returns(dummyJwt);
        return result;
    }
}