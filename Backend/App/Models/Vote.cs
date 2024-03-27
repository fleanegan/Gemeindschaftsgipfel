using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kompetenzgipfel.Models;

public class Vote(Topic topic, User voter)
{
    internal Vote() : this(Topic.Create("title", "description", new User()), new User())
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; init; } = null!;

    [Required] public Topic Topic { get; init; } = topic;

    [Required] public User Voter { get; init; } = voter;
}