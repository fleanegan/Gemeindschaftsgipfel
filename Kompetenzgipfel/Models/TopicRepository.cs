using Microsoft.EntityFrameworkCore;

namespace Kompetenzgipfel.Models;

public class TopicRepository(DatabaseContextApplication dbContext)
{
    public async Task<IEnumerable<Topic>> GetAll()
    {
        return await dbContext.Topics.Include(topic => topic.User).Where(topic => topic.Id != "").ToListAsync();
    }

    public async Task<Topic> Create(Topic? newTopic)
    {
        dbContext.Topics.Add(newTopic);
        await dbContext.SaveChangesAsync();
        return newTopic!;
    }

    public async Task<Topic?> FetchBy(string topicId)
    {
        return await dbContext.Topics.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == topicId);
    }

    public async Task<Topic> Update(Topic updatedTopic)
    {
        dbContext.Topics.Update(updatedTopic);
        await dbContext.SaveChangesAsync();
        return updatedTopic;
    }

    public async Task<IEnumerable<Topic>> GetAllExceptForUser(string userName)
    {
        return await dbContext.Topics.Include(c => c.User).Where(topic => topic.User.UserName != userName)
            .ToListAsync();
    }
}