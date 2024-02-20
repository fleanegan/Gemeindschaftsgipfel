using Microsoft.EntityFrameworkCore;

namespace Kompetenzgipfel.Models;

public class DatabaseContextDomain(DbContextOptions<DatabaseContextDomain> options) : DbContext(options)
{
    public DbSet<Topic> Topics => Set<Topic>();
}