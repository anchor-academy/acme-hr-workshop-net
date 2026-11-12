using Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

public static class TestDb
{
    /// <summary>A fresh in-memory SQLite database with the synthetic seed data.</summary>
    public static AcmeDb Create(bool seed = true)
    {
        var conn = new SqliteConnection("Data Source=:memory:");
        conn.Open();
        var db = new AcmeDb(new DbContextOptionsBuilder<AcmeDb>().UseSqlite(conn).Options);
        db.Database.EnsureCreated();
        if (seed) Seed.Run(db);
        return db;
    }
}
