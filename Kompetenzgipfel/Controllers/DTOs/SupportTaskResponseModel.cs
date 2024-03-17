namespace Kompetenzgipfel.Controllers.DTOs;

public class SupportTaskResponseModel(
    string id,
    string title,
    string description,
    int requiredSupporters,
    List<string> supporterUserNames)
{
    public string Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public int RequiredSupporters { get; set; } = requiredSupporters;
    public List<string> SupporterUserNames { get; set; } = supporterUserNames;
}