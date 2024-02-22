namespace Kompetenzgipfel.Models;

public class TopicRepository(DatabaseContextApplication dbContext)
{
    public IEnumerable<Topic> GetAll()
    {
        return dbContext.Topics.ToList();
    }

    public async Task<Topic> Create(Topic newTopic)
    {
        var result = dbContext.Topics.Add(newTopic);
        await dbContext.SaveChangesAsync();
        return result.Entity;
    }

    // Implement other CRUD methods
}