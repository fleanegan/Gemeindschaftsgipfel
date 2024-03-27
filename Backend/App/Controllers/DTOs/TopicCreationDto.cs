using System.ComponentModel.DataAnnotations;

namespace Kompetenzgipfel.Controllers.DTOs;

public class TopicCreationDto(string title, string? description)
{
    public string? Description { get; } = description;

    [Required] public string Title { get; } = title;
}