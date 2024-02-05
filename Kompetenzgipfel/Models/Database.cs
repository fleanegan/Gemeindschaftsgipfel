namespace Kompetenzgipfel.Models;

using Microsoft.EntityFrameworkCore;

public class Database(DbContextOptions<Database> options) : DbContext(options)
{
    public DbSet<Topic> Topics => Set<Topic>();
}