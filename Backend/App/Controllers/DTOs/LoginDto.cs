using System.ComponentModel.DataAnnotations;

namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class LoginDto(string userName, string password)
{
    [Required] public string UserName { get; } = userName;

    [Required] public string Password { get; } = password;
}