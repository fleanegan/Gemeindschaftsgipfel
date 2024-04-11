using System.ComponentModel.DataAnnotations;

namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class TopicDeletionDto(string topicId)
{
    [Required] public string TopicId { get; } = topicId;
}