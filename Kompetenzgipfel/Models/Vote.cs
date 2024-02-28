using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kompetenzgipfel.Models;

public class Vote
{
    internal Vote()
    {
    }

    public Vote(Topic topic, User voter)
    {
        Topic = topic;
        Voter = voter;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    [Required] public Topic Topic { get; set; }

    [Required] public User Voter { get; set; }
}