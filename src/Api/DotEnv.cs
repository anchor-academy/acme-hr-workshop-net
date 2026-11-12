namespace Api;

/// <summary>Loads KEY=VALUE lines from the nearest .env file (repo root) into environment variables.</summary>
public static class DotEnv
{
    public static void Load()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, ".env"))) dir = dir.Parent;
        if (dir is null) return;
        foreach (var line in File.ReadAllLines(Path.Combine(dir.FullName, ".env")))
        {
            var t = line.Trim();
            if (t.Length == 0 || t.StartsWith('#') || !t.Contains('=')) continue;
            var i = t.IndexOf('=');
            var key = t[..i].Trim();
            var value = t[(i + 1)..].Trim().Trim('"');
            if (Environment.GetEnvironmentVariable(key) is null) Environment.SetEnvironmentVariable(key, value);
        }
    }
}
