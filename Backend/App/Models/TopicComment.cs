using System.ComponentModel.DataAnnotations;
using static Gemeinschaftsgipfel.Properties.Constants;

namespace Gemeinschaftsgipfel.Models;

public class TopicComment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public User Creator { get; set; }
    public Topic Topic { get; set; }

    private TopicComment() { }

    public static TopicComment Create(string content, User creator, Topic topic)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ValidationException("Content cannot be empty");
        
        if (content.Length > MaxLengthTopicCommentContent)
            throw new ValidationException("Content is too long (max 5000 characters)");

        return new TopicComment
        {
            Content = content,
            CreatedAt = DateTime.Now,
            Creator = creator,
            Topic = topic
        };
    }
}
