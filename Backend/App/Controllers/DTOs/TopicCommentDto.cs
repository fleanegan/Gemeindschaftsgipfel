using System.ComponentModel.DataAnnotations;

namespace Gemeinschaftsgipfel.Controllers.DTOs;

public record TopicCommentDto(
    [Required] string topicId,
    [Required] string content
);
