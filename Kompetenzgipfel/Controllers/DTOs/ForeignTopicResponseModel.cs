namespace Kompetenzgipfel.Controllers.DTOs;

public class ForeignTopicResponseModel(string id, string title, string? description, string presenterUserName)
{
    public string Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public string PresenterUserName { get; set; } = presenterUserName;
}