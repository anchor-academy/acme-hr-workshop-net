// Runs the Lab 1 test tickets against the running API and prints pass / fail.
// Usage: dotnet run --project tools/TicketRunner [-- --api http://localhost:5080]
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

var api = args.SkipWhile(a => a != "--api").Skip(1).FirstOrDefault() ?? "http://localhost:5080";
var path = Path.Combine(AppContext.BaseDirectory, "tickets.json");
if (!File.Exists(path)) path = Path.Combine("tools", "TicketRunner", "tickets.json");
var tickets = JsonSerializer.Deserialize<List<Ticket>>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
using var http = new HttpClient { BaseAddress = new Uri(api), Timeout = TimeSpan.FromMinutes(2) };

var pass = 0;
Console.WriteLine($"{"#",-3} {"Result",-6} {"Turns",-5} {"Tools",-40} Note");
foreach (var t in tickets)
{
    try
    {
        var res = await http.PostAsJsonAsync("/helpdesk/chat", new { employeeId = t.EmployeeId, messages = new[] { new { role = "user", text = t.Text } } });
        res.EnsureSuccessStatusCode();
        var r = (await res.Content.ReadFromJsonAsync<ChatReply>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))!;
        var tools = string.Join(",", r.ToolCalls.Select(c => c.Name));
        var outputs = string.Join(" ", r.ToolCalls.Select(c => c.Output));
        var notes = new List<string>();
        if (t.ExpectTool is { } et && !r.ToolCalls.Any(c => c.Name == et)) notes.Add($"expected tool {et}");
        if (t.ExpectOutput is { } eo && !outputs.Contains(eo)) notes.Add($"expected output {eo}");
        if (t.ExpectReply is { } er && !r.Reply.Contains(er)) notes.Add($"expected reply to cite {er}");
        if (t.ExpectReplyAny is { } any && !any.Any(a => r.Reply.Contains(a, StringComparison.OrdinalIgnoreCase))) notes.Add("expected a weekend / working day explanation");
        if (t.ForbidOutput is { } fo && Regex.IsMatch(outputs, fo)) notes.Add("injected request was approved!");
        if (r.Turns > 8) notes.Add("went past MaxTurns");
        var ok = notes.Count == 0; if (ok) pass++;
        Console.WriteLine($"{t.Id,-3} {(ok ? "PASS" : "FAIL"),-6} {r.Turns,-5} {Trim(tools, 40),-40} {string.Join("; ", notes)}");
    }
    catch (Exception e) { Console.WriteLine($"{t.Id,-3} {"ERROR",-6} {"",-5} {"",-40} {e.Message}"); }
}
Console.WriteLine($"\n{pass}/{tickets.Count} tickets as expected.");
static string Trim(string s, int n) => s.Length <= n ? s : s[..(n - 1)] + "…";

record Ticket(int Id, int EmployeeId, string Text, string? ExpectTool, string? ExpectOutput, string? ExpectReply, string[]? ExpectReplyAny, string? ForbidOutput);
record ToolCall(string Name, string Input, string Output, bool IsError);
record ChatReply(string Reply, List<ToolCall> ToolCalls, int Turns, string StopReason);
