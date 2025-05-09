namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class TopicCommentResponseModel(
    string id,
    string content,
    string creatorUserName,
    DateTime createdAt)
{
    public string Id { get; } = id;
    public string Content { get; } = content;
    public string CreatorUserName { get; } = creatorUserName;
    public DateTime CreatedAt { get; } = createdAt;
}