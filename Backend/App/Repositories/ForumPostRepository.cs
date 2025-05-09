using Gemeinschaftsgipfel.Models;
using Microsoft.EntityFrameworkCore;

namespace Gemeinschaftsgipfel.Repositories;

public class ForumPostRepository(DatabaseContextApplication dbContext)
{
    public async Task<ForumPost> Create(ForumPost newForumPost)
    {
        dbContext.Posts.Add(newForumPost);
        await dbContext.SaveChangesAsync();
        return newForumPost;
    }

    public async Task<IEnumerable<ForumPost>> GetPostsForTopic(string topicId)
    {
        return await dbContext.Posts
            .Include(post => post.Creator)
            .Include(post => post.Topic)
            .Where(post => post.Topic.Id == topicId)
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync();
    }
}
