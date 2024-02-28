using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kompetenzgipfel.Models;

public class DatabaseContextApplication(DbContextOptions<DatabaseContextApplication> options)
    : IdentityDbContext<User>(options)
{
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Vote> Votes => Set<Vote>();
}