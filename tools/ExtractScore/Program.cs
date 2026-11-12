// Lab 6 scorer: sends the 20 sample documents to the API and compares with seed/documents/expected.json.
// Usage: dotnet run --project tools/ExtractScore -- --team "Team Name" [--api http://localhost:5080]
// Optional: set LEADERBOARD_URL to post { team, score } to the trainer's board.
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

string Arg(string name, string def) { var i = Array.IndexOf(args, name); return i >= 0 && i + 1 < args.Length ? args[i + 1] : def; }
var team = Arg("--team", "unnamed");
var api = Arg("--api", Environment.GetEnvironmentVariable("ACME_API_URL") ?? "http://localhost:5080");
var dir = Path.Combine(AppContext.BaseDirectory, "documents");
var expected = JsonDocument.Parse(File.ReadAllText(Path.Combine(dir, "expected.json"))).RootElement;
using var http = new HttpClient { BaseAddress = new Uri(api), Timeout = TimeSpan.FromMinutes(2) };

// The API needs a signed-in user. Defaults to a training account (see README).
var email = Environment.GetEnvironmentVariable("ACME_EMAIL") ?? "dina.ismail4@acme-hr.example";
var password = Environment.GetEnvironmentVariable("SEED_USER_PASSWORD") ?? "Acme@2026";
var login = await http.PostAsJsonAsync("/auth/login", new { email, password });
if (!login.IsSuccessStatusCode) { Console.WriteLine($"Sign-in failed for {email}: HTTP {(int)login.StatusCode}"); Environment.Exit(1); }
http.DefaultRequestHeaders.Authorization = new("Bearer", (await login.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("accessToken").GetString());

int fieldsRight = 0, fieldsTotal = 0, statusRight = 0, docs = 0, handled = 0;
Console.WriteLine($"{"File",-14} {"Status (exp)",-26} Fields  Problems");
foreach (var e in expected.EnumerateObject())
{
    docs++;
    var text = File.ReadAllText(Path.Combine(dir, e.Name));
    JsonElement res;
    try
    {
        var r = await http.PostAsJsonAsync("/documents/extract", new { text });
        res = await r.Content.ReadFromJsonAsync<JsonElement>();
        if (!r.IsSuccessStatusCode) { Console.WriteLine($"{e.Name,-14} HTTP {(int)r.StatusCode}"); continue; }
    }
    catch (Exception ex) { Console.WriteLine($"{e.Name,-14} ERROR {ex.Message}"); continue; }

    var status = res.GetProperty("status").ToString();
    var expStatus = e.Value.GetProperty("status").GetString();
    if (status is "Valid" or "NeedsReview" or "0" or "1") handled++;
    var statusName = status == "0" ? "Valid" : status == "1" ? "NeedsReview" : status;
    if (statusName == expStatus) statusRight++;

    int right = 0, total = 0;
    var kind = e.Value.GetProperty("kind").GetString()!;
    JsonElement data = res.TryGetProperty("data", out var d) && d.ValueKind == JsonValueKind.Object && d.TryGetProperty(kind, out var k) ? k : default;
    foreach (var f in e.Value.GetProperty("fields").EnumerateObject())
    {
        total++;
        if (data.ValueKind == JsonValueKind.Object && data.TryGetProperty(f.Name, out var got) && Same(f.Value, got)) right++;
    }
    fieldsRight += right; fieldsTotal += total;
    var problems = res.TryGetProperty("problems", out var p) ? string.Join("; ", p.EnumerateArray().Select(x => x.GetString())) : "";
    Console.WriteLine($"{e.Name,-14} {statusName + " (" + expStatus + ")",-26} {right}/{total}    {problems}");
}

var accuracy = fieldsTotal == 0 ? 0 : 100.0 * fieldsRight / fieldsTotal;
var score = Math.Round(0.7 * accuracy + 30.0 * statusRight / docs, 1);
Console.WriteLine();
Console.WriteLine($"Handled (Valid or NeedsReview): {handled}/{docs}");
Console.WriteLine($"Status as expected:             {statusRight}/{docs}");
Console.WriteLine($"Field accuracy:                 {accuracy:F1}%");
Console.WriteLine($"SCORE for {team}: {score.ToString(CultureInfo.InvariantCulture)}");

if (Environment.GetEnvironmentVariable("LEADERBOARD_URL") is { Length: > 0 } board)
{
    try { await http.PostAsJsonAsync(board, new { team, score }); Console.WriteLine("Posted to leaderboard."); }
    catch (Exception ex) { Console.WriteLine($"Leaderboard post failed: {ex.Message}"); }
}

static bool Same(JsonElement exp, JsonElement got) =>
    exp.ValueKind == JsonValueKind.Number && got.ValueKind == JsonValueKind.Number
        ? exp.GetDecimal() == got.GetDecimal()
        : string.Equals(exp.ToString().Trim(), got.ToString().Trim(), StringComparison.OrdinalIgnoreCase);
