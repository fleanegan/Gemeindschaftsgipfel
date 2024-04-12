using Gemeinschaftsgipfel.Models;
using Microsoft.EntityFrameworkCore;

namespace Gemeinschaftsgipfel.Repositories;

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
            .ThenInclude(promise => promise.Supporter)
            .FirstOrDefaultAsync(c => c.Id == supportTaskId);
    }

    public async Task<SupportTask> Update(SupportTask updatedSupportTask)
    {
        dbContext.SupportTasks.Update(updatedSupportTask);
        await dbContext.SaveChangesAsync();
        return (await FetchBy(updatedSupportTask.Id))!;
    }
}