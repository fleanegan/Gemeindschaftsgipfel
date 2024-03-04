using System.Text;
using Kompetenzgipfel;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
// read variables from .env file
// todo: make sure that this is the only configuration source. Needs to set the configuration provider for builder but don't know how to do that right know
DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
// Add jwt support
builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
                          Environment.GetEnvironmentVariable("SERVER_PORT"),
            ValidAudience = "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
                            Environment.GetEnvironmentVariable("CLIENT_PORT"),
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_KEY_JWT_PRIVATE")))
        };
    });
builder.Services.AddControllers();
builder.Services.AddDbContext<DatabaseContextApplication>(options =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString(Environment.GetEnvironmentVariable("DB_NAME")!);
    options.UseSqlite(connectionString);
});
//todo: find a way to replace by these lines without breaking the IEmailSender dependency
// builder.Services.AddAuthorization();
// builder.Services.AddIdentity<User, IdentityRole>()
// .AddEntityFrameworkStores<DatabaseContextApplication>();
builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<DatabaseContextApplication>();
builder.Services.AddCors();
builder.Services.AddScoped<TopicRepository, TopicRepository>();
builder.Services.AddScoped<VoteRepository, VoteRepository>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<JwtGenerationService>();
var app = builder.Build();
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
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<User>();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");
app.MapGet("/protected", (HttpContext httpContext) => "J'ai le meilleur Sab")
    .WithName("GetWeatherForecast")
    .RequireAuthorization();
app.Run();

public partial class Program
{
} // needed for testing