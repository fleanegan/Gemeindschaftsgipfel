namespace Kompetenzgipfel.Controllers.DTOs;

public class OwnTopicResponseModel(
    string id,
    string title,
    string? description,
    string presenterUserName,
    int voteCount)
{
    public string Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public string PresenterUserName { get; set; } = presenterUserName;
    public string Votes { get; set; } = voteCount.ToString();
}