using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kompetenzgipfel.Models;

public class SupportPromise
{
    internal SupportPromise()
    {
    }

    public SupportPromise(SupportTask supportTask, User supporter)
    {
        SupportTask = supportTask;
        Supporter = supporter;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    [Required] public SupportTask SupportTask { get; set; }

    [Required] public User Supporter { get; set; }
}