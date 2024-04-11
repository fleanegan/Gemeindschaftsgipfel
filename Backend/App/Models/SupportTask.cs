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
    public string Id { get; init; } = null!;

    [StringLength(Constants.MaxLengthTitle)]
    [Required]
    public string Title { get; init; }

    [StringLength(Constants.MaxLengthDescription)]
    [Required]
    public string Description { get; init; }

    [StringLength(Constants.MaxLengthTitle)]
    [Required]
    public string Duration { get; init; }

    [Required] public int RequiredSupporters { get; init; }

    public ICollection<SupportPromise> SupportPromises { get; init; } = [];
}