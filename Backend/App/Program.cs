using System.Text;
using System.Threading.RateLimiting;
using Gemeinschaftsgipfel;
using Gemeinschaftsgipfel.Models;
using Gemeinschaftsgipfel.Repositories;
using Gemeinschaftsgipfel.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log/log.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 500000)
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.MessageTemplate.Text.Contains("LOGIN_REGISTER"))
        .WriteTo.File("log/log_inregister.txt", rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information()
    )
    .CreateLogger();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponsePropertiesAndHeaders;

    options.CombineLogs = true;
});
// todo: make sure that .env is the only configuration source. Needs to set the configuration provider for builder but don't know how to do that right know
var parentFullName = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.FullName ?? Directory.GetCurrentDirectory();
var envFilePath = Path.Combine(
    parentFullName!, ".env");
DotEnv.Load(envFilePath);
if (!builder.Environment.IsDevelopment()) Console.WriteLine("this is in production");
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.WebHost.ConfigureKestrel(options =>
{
    var serverPort = int.Parse(Environment.GetEnvironmentVariable("SERVER_PORT")!);
    if (builder.Environment.IsDevelopment())
        options.ListenLocalhost(serverPort, listenOptions => listenOptions.UseHttps());
    else
        options.ListenAnyIP(serverPort);
});
builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var productionAudience = "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
                                 Environment.GetEnvironmentVariable("SERVER_PORT");
        var developmentAudience = "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
                                  Environment.GetEnvironmentVariable("CLIENT_PORT");
        var audiences = new List<string> { productionAudience, developmentAudience };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
                          Environment.GetEnvironmentVariable("SERVER_PORT"),
            ValidAudiences = audiences,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_KEY_JWT_PRIVATE")!))
        };
    });
builder.Services.AddControllers();
builder.Services.AddDbContext<DatabaseContextApplication>(options =>
{
    var config = builder.Configuration;
    var connectionString = "Data Source=data/database.db";
    options.UseSqlite(connectionString);
});
builder.Services.AddTransient<IEmailSender<User>, NoOpEmailSenderService>();
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<DatabaseContextApplication>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(60);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });
});
builder.Services.AddScoped<TopicRepository, TopicRepository>();
builder.Services.AddScoped<VoteRepository, VoteRepository>();
builder.Services.AddScoped<SupportPromiseRepository, SupportPromiseRepository>();
builder.Services.AddScoped<SupportTaskRepository, SupportTaskRepository>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<ISupportTaskService, SupportTaskService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<JwtGenerationService>();
var app = builder.Build();
app.UseForwardedHeaders();
app.UseHttpLogging();
if (!app.Environment.IsDevelopment())
{
    // Configure the HTTP request pipeline.
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
if (app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<User>();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");
app.UseRateLimiter();
app.Run();

public partial class Program
{
} // needed for testing
