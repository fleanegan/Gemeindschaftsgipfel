namespace Kompetenzgipfel.Controllers.DTOs;

public class SupportTaskCreationDto(string title, string description, string duration, int requiredSupporters)
{
    public string Description { get; } = description;
    public string Title { get; } = title;
    public string Duration { get; } = duration;
    public int RequiredSupporters { get; } = requiredSupporters;
}