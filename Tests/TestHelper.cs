using System.Text;
using System.Text.Json;
using Kompetenzgipfel.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests;

internal abstract class TestHelper
{
    public static Database GetDbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<Database>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new Database(options);
        dbContext.Database.EnsureCreated();

        return dbContext;
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