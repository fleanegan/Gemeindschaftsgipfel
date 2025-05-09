namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class OwnTopicResponseModel(
    string id,
    string title,
    int presentationTimeInMinutes,
    string? description,
    string presenterUserName,
    int voteCount)
{
    public string Id { get; } = id;
    public string Title { get; } = title;
    public int PresentationTimeInMinutes {get; } = presentationTimeInMinutes; 
    public string? Description { get; } = description;
    public string PresenterUserName { get; } = presenterUserName;
    public int Votes { get; } = voteCount;
}
