// Module 5 live demo: a small MCP server over a fake bug list, in C#.
// Read-only on purpose: there is no tool that writes, so even a fooled agent cannot change data.
// Record 117 contains a prompt injection for the "poisoned bug report" demo.
using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace); // stdout is for MCP messages only
builder.Services.AddMcpServer(o => o.ServerInfo = new() { Name = "bugs", Version = "1.0.0" }).WithStdioServerTransport().WithToolsFromAssembly();
await builder.Build().RunAsync();

public record Bug(int Id, string Title, string Status, string Area, string Description);

[McpServerToolType]
public static class BugTools
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    // Loaded once, never written.
    static readonly List<Bug> Bugs = JsonSerializer.Deserialize<List<Bug>>(
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "bugs.json")), Json)!;

    [McpServerTool(Name = "get_bug", ReadOnly = true)]
    [Description("Get one bug by its numeric id. Use search_bugs first if you do not know the id.")]
    public static string GetBug([Description("Bug id, for example 117")] int id) =>
        Bugs.FirstOrDefault(b => b.Id == id) is { } bug
            ? JsonSerializer.Serialize(bug, Json)
            : throw new McpException($"Bug {id} not found. Use search_bugs to find the id.");

    [McpServerTool(Name = "search_bugs", ReadOnly = true)]
    [Description("Search bugs by keyword in the title or description. Optional status filter. Returns id, title and status only; use get_bug for details.")]
    public static string SearchBugs(
        [Description("Keyword, for example 'leave' or 'calendar'")] string query,
        [Description("open, closed or any")] string status = "any")
    {
        if (query.Trim().Length < 2) throw new McpException("Query must be at least 2 characters.");
        if (status is not ("open" or "closed" or "any")) throw new McpException("Status must be open, closed or any.");
        var hits = Bugs
            .Where(b => status == "any" || b.Status == status)
            .Where(b => $"{b.Title} {b.Description}".Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(b => new { b.Id, b.Title, b.Status })
            .ToList();
        return hits.Count > 0 ? JsonSerializer.Serialize(hits, Json) : $"No bugs match \"{query}\". Try a shorter keyword.";
    }
}
