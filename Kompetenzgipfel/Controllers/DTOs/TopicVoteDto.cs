using System.ComponentModel.DataAnnotations;

namespace Kompetenzgipfel.Controllers.DTOs;

public class TopicVoteDto
{
    [Required] public string topicId { get; set; }
}