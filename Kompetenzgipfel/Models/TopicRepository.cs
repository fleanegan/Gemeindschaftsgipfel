using Microsoft.EntityFrameworkCore;

namespace Kompetenzgipfel.Models;

public class TopicRepository(DatabaseContextApplication dbContext)
{
    public IEnumerable<Topic?> GetAll()
    {
        return dbContext.Topics.Where(topic => topic.Id != "");
    }

    public async Task<Topic> Create(Topic? newTopic)
    {
        dbContext.Topics.Add(newTopic);
        await dbContext.SaveChangesAsync();
        return newTopic!;
    }

    public async Task<Topic?> FetchBy(string topicId)
    {
        var includableQueryable = dbContext.Topics.Include(c => c.User);
        return await includableQueryable.FirstOrDefaultAsync(c => c.Id == topicId);
    }

    public async Task<Topic> Update(Topic updatedTopic)
    {
        dbContext.Topics.Update(updatedTopic);
        await dbContext.SaveChangesAsync();
        return updatedTopic;
    }
}