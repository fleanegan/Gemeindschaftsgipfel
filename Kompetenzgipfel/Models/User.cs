using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Kompetenzgipfel.Models;

public class User : IdentityUser
{
    [Required] public override string UserName { get; set; }
    public ICollection<Topic> Topics { get; set; }
    public ICollection<Vote> Votes { get; set; }
    public ICollection<SupportPromise> SupportPromises { get; set; }
}