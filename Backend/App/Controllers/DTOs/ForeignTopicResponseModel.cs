namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class ForeignTopicResponseModel(
    string id,
    string title,
    int presentationTimeInMinutes,
    string? description,
    string presenterUserName,
    bool didIVoteForThis)
{
    public string Id { get; } = id;
    public string Title { get; } = title;
    public int PresentationTimeInMinutes {get; } = presentationTimeInMinutes; 
    public string? Description { get; } = description;
    public string PresenterUserName { get; } = presenterUserName;
    public bool DidIVoteForThis { get; } = didIVoteForThis;
}
