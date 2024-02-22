namespace Kompetenzgipfel.Controllers.DTOs;
public class TopicCreationResponse(string title, string? description, string creatorUserName)
{
    public string Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public string CreatorUserName { get; set; } = creatorUserName;
}