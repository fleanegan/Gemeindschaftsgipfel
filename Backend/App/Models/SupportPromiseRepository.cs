using Microsoft.EntityFrameworkCore;

namespace Kompetenzgipfel.Models;

public class SupportPromiseRepository(DatabaseContextApplication dbContext)
{
    public async Task Create(SupportPromise supportPromise)
    {
        dbContext.Add(supportPromise);
        await dbContext.SaveChangesAsync();
    }

    public async Task Remove(SupportPromise supportPromise)
    {
        dbContext.SupportPromises.Remove(supportPromise);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<SupportPromise>> FetchAll()
    {
        return await dbContext.SupportPromises.ToListAsync();
    }
}