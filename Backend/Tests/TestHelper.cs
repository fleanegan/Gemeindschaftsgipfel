using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Gemeinschaftsgipfel;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests;

internal abstract class TestHelper
{
    public static T GetDbContext<T>() where T : DbContext
    {
        var options = GetDbContextOptions<T>();

        var dbContext = (T)Activator.CreateInstance(typeof(T), options)!;
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    public static DbContextOptions GetDbContextOptions<T>() where T : DbContext
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<T>()
            .UseSqlite(connection)
            .Options;
        return options;
    }

    public static StringContent encodeBody(object value, string? jwtAuthToken = null)
    {
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(value),
            Encoding.UTF8,
            "application/json"
        );
        if (jwtAuthToken != null)
            jsonContent.Headers.Add("HttpRequestMessage", "Authorization: Bearer " + jwtAuthToken.Trim());
        return jsonContent;
    }

    public static UserManager<User> GetIntegrationInMemoryUserManager()
    {
        var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.RemoveAll(typeof(ITopicService)); });
        });
        var scope = application.Services.CreateScope();
        var userStore = new UserStore<User>(GetDbContext<DatabaseContextApplication>());
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

    public static Mock<UserManager<User>> GetMockUserManager(IUserStore<User>? userStore = null)
    {
        var userManager = new Mock<UserManager<User>>(
            userStore ?? Mock.Of<IUserStore<User>>(), // Pass a mock of IUserStore<User>
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
        userManager.Setup(c => c.FindByNameAsync("FakeAuthUser")).Returns(() => Task.FromResult(new User
        {
            UserName = "FakeAuthUser",
            Id = "made up id",
            AccessFailedCount = 0,
            EmailConfirmed = false,
            LockoutEnabled = false
        })!);
        userManager.Setup(c => c.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => Task.FromResult(new IdentityResult()));
        return userManager;
    }

    public static Mock<PasswordHasher<User>> GetMockPasswordHasher()
    {
        // new PasswordHasher<User>()
        var passwordHasher = new Mock<PasswordHasher<User>>(null);
        passwordHasher.Setup(c => c.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        return passwordHasher;
    }

    public static Mock<JwtGenerationService> GetJwtGenerationService(string dummyJwt = "dummy_JWT")
    {
        var result = new Mock<JwtGenerationService>();
        result.Setup(c => c.Generate(It.IsAny<IEnumerable<Claim>>())).Returns(dummyJwt);
        return result;
    }

    public static void ReadTestEnv()
    {
        var parentFullName = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent
            ?.FullName;
        var envFilePath = Path.Combine(
            parentFullName!,
            ".env");
        DotEnv.Load(envFilePath);
    }
}

internal class AutoAuthorizeStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder.UseMiddleware<AutoAuthorizeMiddleware>();
            next(builder);
        };
    }
}

internal class AutoAuthorizeMiddleware(RequestDelegate rd)
{
    public const string UserName = "FakeAuthUser";

    public async Task Invoke(HttpContext httpContext)
    {
        var identity = new ClaimsIdentity("Bearer");
        identity.AddClaim(new Claim(ClaimTypes.Name, UserName));
        httpContext.User.AddIdentity(identity);
        await rd.Invoke(httpContext);
    }
}