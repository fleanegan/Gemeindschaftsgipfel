using System.ComponentModel.DataAnnotations;

namespace Gemeinschaftsgipfel.Controllers.DTOs;

public record TopicForumPostDto(
    [Required] string topicId,
    [Required] string content
);
