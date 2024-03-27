using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kompetenzgipfel.Models;

public class SupportPromise(SupportTask supportTask, User supporter)
{
    internal SupportPromise() : this(new SupportTask(), new User())
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; init; } = null!;

    [Required] public SupportTask SupportTask { get; init; } = supportTask;

    [Required] public User Supporter { get; init; } = supporter;
}