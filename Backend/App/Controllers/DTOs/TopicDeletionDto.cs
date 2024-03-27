using System.ComponentModel.DataAnnotations;

namespace Kompetenzgipfel.Controllers.DTOs;

public class TopicDeletionDto(string topicId)
{
    [Required] public string TopicId { get; } = topicId;
}