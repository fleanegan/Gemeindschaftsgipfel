using System.ComponentModel.DataAnnotations;

namespace Kompetenzgipfel.Controllers;

public class SignupDto(string userName, string password, string passphrase)
{
    [Required] public string UserName { get; } = userName;

    [Required] public string Password { get; } = password;

    [Required] public string Passphrase { get; } = passphrase;
}