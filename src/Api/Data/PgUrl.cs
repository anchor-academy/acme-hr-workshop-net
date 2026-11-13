namespace Api.Data;

/// <summary>Render gives postgres://user:pass@host:port/db. Npgsql wants Host=...;Username=... . Accept both.</summary>
public static class PgUrl
{
    public static string? ToNpgsql(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !(value.StartsWith("postgres://") || value.StartsWith("postgresql://"))) return value;
        var uri = new Uri(value);
        var userInfo = uri.UserInfo.Split(':', 2);
        var b = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
        };
        if (uri.Host.Contains('.')) b.SslMode = Npgsql.SslMode.Require; // external URL; the internal host has no dots
        return b.ConnectionString;
    }
}
