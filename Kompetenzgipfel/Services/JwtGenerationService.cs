using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Kompetenzgipfel.Services;

public class JwtGenerationService
{
    public virtual string Generate(IEnumerable<Claim> claims)
    {
        var secretKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_KEY_JWT_PRIVATE") ??
                                       string.Empty));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
            Environment.GetEnvironmentVariable("SERVER_PORT"),
            "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
            Environment.GetEnvironmentVariable("CLIENT_PORT"),
            claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: signinCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}
// this is how to generate claims:
// new List<Claim>([new Claim("username", userInput.UserName)])