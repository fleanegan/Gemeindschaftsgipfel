using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kompetenzgipfel.Properties;

namespace Kompetenzgipfel.Models;

public class SupportTask
{
    internal SupportTask()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    [StringLength(Constants.MaxLengthTitle)]
    [Required]
    public string Title { get; set; }

    [StringLength(Constants.MaxLengthDescription)]
    [Required]
    public string Description { get; set; }

    [StringLength(Constants.MaxLengthTitle)]
    [Required]
    public string Duration { get; set; }

    [Required] public int RequiredSupporters { get; set; }

    public ICollection<SupportPromise> SupportPromises { get; set; } = [];
}