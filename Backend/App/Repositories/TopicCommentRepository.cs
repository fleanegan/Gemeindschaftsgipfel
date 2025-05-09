using Gemeinschaftsgipfel.Models;
using Microsoft.EntityFrameworkCore;

namespace Gemeinschaftsgipfel.Repositories;

public class TopicCommentRepository(DatabaseContextApplication dbContext)
{
    public async Task<TopicComment> Save(TopicComment newTopicComment)
    {
        dbContext.TopicComments.Add(newTopicComment);
        await dbContext.SaveChangesAsync();
        return newTopicComment;
    }

    public async Task<IEnumerable<TopicComment>> GetCommentsForTopic(string topicId)
    {
        return await dbContext.TopicComments
            .Include(topicComment => topicComment.Creator)
            .Include(topicComment => topicComment.Topic)
            .Where(topicComment => topicComment.Topic.Id == topicId)
            .OrderByDescending(topicComment => topicComment.CreatedAt)
            .ToListAsync();
    }
}
