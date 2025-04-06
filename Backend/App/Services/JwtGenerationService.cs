using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Gemeinschaftsgipfel.Services;

public class JwtGenerationService
{
    public virtual string Generate(IEnumerable<Claim> claims)
    {
        var secretKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_KEY_JWT_PRIVATE") ??
                                       string.Empty));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var productionAudience = "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
                                 Environment.GetEnvironmentVariable("SERVER_PORT");
        var developmentAudience = "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
                                  Environment.GetEnvironmentVariable("CLIENT_PORT");
        claims = claims.Append(new Claim("aud", developmentAudience));
        var tokenOptions = new JwtSecurityToken(
            //todo: modify to match the Part in the Program.cs JWT setup!
            "https://" + Environment.GetEnvironmentVariable("IP_ADDRESS") + ":" +
            Environment.GetEnvironmentVariable("SERVER_PORT"),
            productionAudience,
            claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: signinCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}