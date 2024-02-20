using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kompetenzgipfel.Models;

public class DatabaseContextIdentityFramework : IdentityDbContext<User>
{
    public DatabaseContextIdentityFramework(DbContextOptions<DatabaseContextIdentityFramework> options) :
        base(options)
    {
    }
}