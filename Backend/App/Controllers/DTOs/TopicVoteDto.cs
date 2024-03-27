using System.ComponentModel.DataAnnotations;

namespace Kompetenzgipfel.Controllers.DTOs;

public class TopicVoteDto(string topicId)
{
    [Required] public string topicId { get; init; } = topicId;
}