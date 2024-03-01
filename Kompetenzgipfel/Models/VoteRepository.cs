namespace Kompetenzgipfel.Models;

public class VoteRepository(DatabaseContextApplication dbContext)
{
    public async Task Create(Vote newVote)
    {
        dbContext.Add(newVote);
        await dbContext.SaveChangesAsync();
    }

    public async Task Remove(Vote vote)
    {
        dbContext.Votes.Remove(vote);
        await dbContext.SaveChangesAsync();
    }
}