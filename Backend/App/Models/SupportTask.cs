using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemeinschaftsgipfel.Properties;

namespace Gemeinschaftsgipfel.Models;

public class SupportTask
{
    internal SupportTask()
    {
        Title = "";
        Description = "";
        Duration = "";
    }

    public SupportTask(string description, string title, string duration,
        int requiredSupporters, ICollection<SupportPromise> supportPromises)
    {
        Description = description;
        Title = title;
        Duration = duration;
        SupportPromises = supportPromises;
        RequiredSupporters = requiredSupporters;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = null!;

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

    public ICollection<SupportPromise> SupportPromises { get; init; } = [];
}