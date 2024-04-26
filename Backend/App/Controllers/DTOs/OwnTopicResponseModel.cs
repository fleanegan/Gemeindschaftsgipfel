namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class OwnTopicResponseModel(
    string id,
    string title,
    int presentationTimeInMinutes,
    string? description,
    string presenterUserName,
    int voteCount)
{
    public string Id { get; set; } = id;
    public string Title { get; set; } = title;
    public int PresentationTimeInMinutes {get; set; } = presentationTimeInMinutes; 
    public string? Description { get; set; } = description;
    public string PresenterUserName { get; set; } = presenterUserName;
    public int Votes { get; set; } = voteCount;
}
