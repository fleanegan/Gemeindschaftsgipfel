namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class ForeignTopicResponseModel(
    string id,
    string title,
    int presentationTimeInMinutes,
    string? description,
    string presenterUserName,
    bool didIVoteForThis)
{
    public string Id { get; set; } = id;
    public string Title { get; set; } = title;
    public int PresentationTimeInMinutes {get; set; } = presentationTimeInMinutes; 
    public string? Description { get; set; } = description;
    public string PresenterUserName { get; set; } = presenterUserName;
    public bool DidIVoteForThis { get; set; } = didIVoteForThis;
}
