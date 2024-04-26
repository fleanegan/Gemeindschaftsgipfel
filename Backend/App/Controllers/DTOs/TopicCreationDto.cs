using System.ComponentModel.DataAnnotations;

namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class TopicCreationDto(string title, int presentationTimeInMinutes, string? description)
{
    public string? Description { get; } = description;

    [Required] public string Title { get; } = title;

    [Required] public int PresentationTimeInMinutes {get;} = presentationTimeInMinutes;
}
