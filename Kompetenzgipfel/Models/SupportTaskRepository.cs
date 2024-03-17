using Microsoft.EntityFrameworkCore;

namespace Kompetenzgipfel.Models;

public class SupportTaskRepository(DatabaseContextApplication dbContext)
{
    public async Task<IEnumerable<SupportTask>> FetchAll()
    {
        return await dbContext.SupportTasks
            .Include(supportTask => supportTask.SupportPromises)
            .ThenInclude(promise => promise.Supporter)
            .Select(task => task)
            .ToListAsync();
    }

    public async Task<SupportTask> Create(SupportTask newSupportTask)
    {
        dbContext.SupportTasks.Add(newSupportTask!);
        await dbContext.SaveChangesAsync();
        return newSupportTask;
    }

    public async Task<SupportTask?> FetchBy(string supportTaskId)
    {
        return await dbContext.SupportTasks
            .Select(supportTask => supportTask)
            .Include(supportTask => supportTask.SupportPromises)
            .FirstOrDefaultAsync(c => c.Id == supportTaskId);
    }
}