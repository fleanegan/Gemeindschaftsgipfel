using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests;

internal abstract class TestHelper
{
    public static T GetDbContext<T>() where T : DbContext
    {
        var options = GetDbContextOptions<T>();

        var dbContext = (T)Activator.CreateInstance(typeof(T), options)!;
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    public static DbContextOptions GetDbContextOptions<T>() where T : DbContext
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<T>()
            .UseSqlite(connection)
            .Options;
        return options;
    }

    public static StringContent encodeBody(object value)
    {
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(value),
            Encoding.UTF8,
            "application/json"
        );
        return jsonContent;
    }
}