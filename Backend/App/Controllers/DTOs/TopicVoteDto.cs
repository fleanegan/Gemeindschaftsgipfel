using System.ComponentModel.DataAnnotations;

namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class TopicVoteDto(string topicId)
{
    [Required] public string topicId { get; } = topicId;
}