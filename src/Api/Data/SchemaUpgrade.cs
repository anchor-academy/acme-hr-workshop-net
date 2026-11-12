using Microsoft.EntityFrameworkCore;

namespace Api.Data;

/// <summary>
/// EnsureCreated does nothing if the database already has tables, so a table added in a later lab
/// (for example Documents in Lab 6) would be missing. This adds any missing tables and indexes.
/// Training shortcut: a real system would use EF Core migrations.
/// </summary>
public static class SchemaUpgrade
{
    public static void AddMissingTables(AcmeDb db)
    {
        var script = db.Database.GenerateCreateScript();
        foreach (var raw in script.Split(';'))
        {
            var sql = raw.Trim();
            if (sql.StartsWith("CREATE TABLE ", StringComparison.OrdinalIgnoreCase))
                db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS " + sql["CREATE TABLE ".Length..]);
            else if (sql.StartsWith("CREATE INDEX ", StringComparison.OrdinalIgnoreCase))
                db.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS " + sql["CREATE INDEX ".Length..]);
        }
    }
}
