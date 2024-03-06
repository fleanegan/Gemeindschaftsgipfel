using System.Text;
using Kompetenzgipfel;
using Kompetenzgipfel.Models;
using Kompetenzgipfel.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
// todo: make sure that .env is the only configuration source. Needs to set the configuration provider for builder but don't know how to do that right know
DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
builder.WebHost.ConfigureKestrel(options =>
{
    var serverPort = int.Parse(Environment.GetEnvironmentVariable("SERVER_PORT")??"8080");
    if (builder.Environment.IsDevelopment())
        options.ListenLocalhost(serverPort, listenOptions => listenOptions.UseHttps());
    else
        options.ListenAnyIP(serverPort);
});
Console.WriteLine("this is in production");
builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var httpPrefix = builder.Environment.IsDevelopment() ? "https://" : "http//";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = httpPrefix + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
                          Environment.GetEnvironmentVariable("SERVER_PORT"),
            ValidAudience = httpPrefix + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
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
if (app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<User>();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");
app.Run();

public partial class Program
{
} // needed for testing