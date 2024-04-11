using System.ComponentModel.DataAnnotations;

namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class SupportTaskCreationDto(string title, string description, string duration, int requiredSupporters)
{
    [Required] public string Description { get; } = description;

    [Required] public string Title { get; } = title;

    [Required] public string Duration { get; } = duration;

    [Required] public int RequiredSupporters { get; } = requiredSupporters;
}