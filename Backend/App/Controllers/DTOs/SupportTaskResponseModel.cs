namespace Gemeinschaftsgipfel.Controllers.DTOs;

public class SupportTaskResponseModel(
    string id,
    string title,
    string description,
    string duration,
    int requiredSupporters,
    List<string> supporterUserNames)
{
    public string Id { get; } = id;
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string Duration { get; } = duration;
    public int RequiredSupporters { get; } = requiredSupporters;
    public List<string> SupporterUserNames { get; } = supporterUserNames;
}