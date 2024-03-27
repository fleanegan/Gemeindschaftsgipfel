using Kompetenzgipfel.Models;
using Microsoft.EntityFrameworkCore;

namespace Kompetenzgipfel.Repositories;

public class TopicRepository(DatabaseContextApplication dbContext)
{
    public async Task<IEnumerable<Topic>> GetAll()
    {
        return await dbContext
            .Topics
            .Include(topic => topic.User)
            .Where(topic => topic.Id != "")
            .ToListAsync();
    }

    public async Task<Topic> Create(Topic newTopic)
    {
        dbContext.Topics.Add(newTopic);
        await dbContext.SaveChangesAsync();
        return newTopic;
    }

    public async Task<Topic?> FetchBy(string topicId)
    {
        return await dbContext.Topics
            .Include(c => c.User)
            .Include(c => c.Votes)
            .ThenInclude(c => c.Voter)
            .FirstOrDefaultAsync(c => c.Id == topicId);
    }

    public async Task<Topic> Update(Topic updatedTopic)
    {
        dbContext.Topics.Update(updatedTopic);
        await dbContext.SaveChangesAsync();
        return (await FetchBy(updatedTopic.Id))!;
    }

    public async Task<IEnumerable<Topic>> GetAllExceptForUser(string userName)
    {
        return await dbContext.Topics
            .Include(c => c.User)
            .Include(c => c.Votes)
            .ThenInclude(c => c.Voter)
            .Where(topic => topic.User.UserName != userName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Topic>> GetAllForUser(string userName)
    {
        return await dbContext.Topics
            .Include(c => c.User)
            .Include(c => c.Votes)
            .ThenInclude(c => c.Voter)
            .Where(topic => topic.User.UserName == userName)
            .ToListAsync();
    }

    public async Task Remove(Topic topicToDelete)
    {
        dbContext.Remove(topicToDelete);
        await dbContext.SaveChangesAsync();
    }
}