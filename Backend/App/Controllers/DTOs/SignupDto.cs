using System.ComponentModel.DataAnnotations;

namespace Kompetenzgipfel.Controllers.DTOs;

public class SignupDto(string userName, string password, string entrySecret)
{
    [Required] public string UserName { get; } = userName;

    [Required] public string Password { get; } = password;

    [Required] public string EntrySecret { get; } = entrySecret;
}