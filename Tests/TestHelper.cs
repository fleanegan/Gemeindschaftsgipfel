using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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

    public static void ShouldAddAuthorizedDummyUser(bool shouldAdd)
    {
        Environment.SetEnvironmentVariable("FakeAuth", shouldAdd ? "true" : "false");
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
    public async Task Invoke(HttpContext httpContext)
    {
        var shouldAuthorize = Environment.GetEnvironmentVariable("FakeAuth");
        if (shouldAuthorize == "true")
        {
            var identity = new ClaimsIdentity("Bearer");
            identity.AddClaim(new Claim(ClaimTypes.Name, "TestUser"));
            httpContext.User.AddIdentity(identity);
        }
        else if (shouldAuthorize == "false")
        {
        }
        else
        {
            throw new Exception(
                "When using the AutoAuthorizeMiddleware, specify whether to authorize each request with TestHelper.ShouldAddAuthorizedDummyUser(bool )");
        }

        Environment.SetEnvironmentVariable("FakeAuth", "");
        await rd.Invoke(httpContext);
    }
}