using System.ComponentModel.DataAnnotations;
using static Gemeinschaftsgipfel.Properties.Constants;

namespace Gemeinschaftsgipfel.Models;

public class ForumPost
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public User Creator { get; set; }
    public Topic Topic { get; set; }

    private ForumPost() { }

    public static ForumPost Create(string content, User creator, Topic topic)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ValidationException("Content cannot be empty");
        
        if (content.Length > MaxLengthForumPostContent)
            throw new ValidationException("Content is too long (max 5000 characters)");

        return new ForumPost
        {
            Content = content,
            CreatedAt = DateTime.UtcNow,
            Creator = creator,
            Topic = topic
        };
    }
}
