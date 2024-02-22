namespace Kompetenzgipfel.Controllers.DTOs;

public class TopicCreationResponse(string title, string? description, string presenterUserName)
{
    public string Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public string PresenterUserName { get; set; } = presenterUserName;
}