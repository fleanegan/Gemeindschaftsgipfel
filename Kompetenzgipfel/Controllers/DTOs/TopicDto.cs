using System.ComponentModel.DataAnnotations;

namespace Kompetenzgipfel.Controllers.DTOs;

public class TopicDto(string description, string title)
{
    [Required] public string Description { get; } = description;

    [Required] public string Title { get; } = title;
}