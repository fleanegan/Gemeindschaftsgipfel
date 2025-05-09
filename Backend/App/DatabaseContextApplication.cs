using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gemeinschaftsgipfel.Models;

public class DatabaseContextApplication(DbContextOptions<DatabaseContextApplication> options)
    : IdentityDbContext<User>(options)
{
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<SupportTask> SupportTasks => Set<SupportTask>();
    public DbSet<SupportPromise> SupportPromises => Set<SupportPromise>();
    public DbSet<TopicComment> TopicComments => Set<TopicComment>();
}