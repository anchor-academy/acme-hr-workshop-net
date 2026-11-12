using System.Text.Json;
using System.Text.RegularExpressions;
using Anthropic.Models.Messages;
using Api.Data;
using Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Api.Research;

public record AgentUsage(string Agent, int Turns, long InputTokens, long OutputTokens);
public record ResearchResult(string Answer, bool CitationsValid, IReadOnlyList<string> UncitedSentences, IReadOnlyList<AgentUsage> Agents, long TotalTokens, string StoppedBy);

/// <summary>
/// Lab 3: a lead agent splits a policy question and hands each subtopic to a worker.
/// Each worker is a separate Claude conversation with its own context and only two tools.
/// </summary>
public class PolicyResearch(IClaudeChat claude, AcmeDb db, ClaudeOptions options)
{
    // Hard limits, in code.
    public const int MaxWorkers = 3;
    public const int MaxTurnsLead = 6;
    public const int MaxTurnsPerWorker = 6;
    public const long TokenBudget = 80_000;

    readonly List<AgentUsage> _usage = [];
    long _tokens;

    const string LeadPrompt = """
        You lead a small research team answering questions about Acme HR policies.
        1. Split the question into 2 or 3 subtopics and call research once per subtopic.
        2. When the findings are back, write one answer for the employee, under 150 words.
        3. EVERY sentence must end with the policy ids it relies on, like [POL-03]. Never state anything without a policy id.
        """;

    const string WorkerPrompt = """
        You research one subtopic in the Acme HR policies. Use search_policies, then read_policy for the relevant ones.
        Reply with short findings, each line ending with its policy id in brackets, like [POL-04]. Only report what the policies say.
        """;

    static readonly IReadOnlyList<ToolUnion> LeadTools =
    [
        Tool("research", "Give one subtopic to a research worker. Returns findings with policy ids.", new { subtopic = new { type = "string" } }, "subtopic"),
    ];

    static readonly IReadOnlyList<ToolUnion> WorkerTools =
    [
        Tool("search_policies", "Search policies by keyword. Returns ids and titles.", new { query = new { type = "string" } }, "query"),
        Tool("read_policy", "Read one policy by id, e.g. POL-04.", new { id = new { type = "string" } }, "id"),
    ];

    public async Task<ResearchResult> AskAsync(string question, CancellationToken ct = default)
    {
        var history = new List<MessageParam> { new UserMessageParam { Content = question } };
        var workers = 0;
        int turns = 0; long inTok = 0, outTok = 0;
        string answer = "", stoppedBy = "end_turn";

        for (turns = 1; turns <= MaxTurnsLead; turns++)
        {
            if (_tokens >= TokenBudget) { stoppedBy = "token_budget"; break; }
            var res = await claude.SendAsync(new MessageCreateParams { Model = options.Model, MaxTokens = 1500, System = LeadPrompt, Tools = LeadTools, Messages = history }, ct);
            inTok += res.Usage.InputTokens; outTok += res.Usage.OutputTokens; _tokens += res.Usage.InputTokens + res.Usage.OutputTokens;
            history.Add(new AssistantMessageParam { Content = res.Content.Select(b => new ContentBlockParam(b.Json)).ToList() });

            if (res.StopReason != StopReason.ToolUse) { answer = TextOf(res); break; }

            var results = new List<ContentBlockParam>();
            foreach (var block in res.Content)
            {
                if (!block.TryPickToolUse(out var call)) continue;
                string output; bool isError = false;
                if (workers >= MaxWorkers) { output = $"Worker limit reached ({MaxWorkers}). Answer with what you have."; isError = true; }
                else if (_tokens >= TokenBudget) { output = "Token budget used up. Answer with what you have."; isError = true; }
                else { workers++; output = await RunWorker(Str(call.Input, "subtopic"), $"worker-{workers}", ct); }
                results.Add(new ToolResultBlockParam(call.ID) { Content = output, IsError = isError });
            }
            history.Add(new UserMessageParam { Content = results });
        }
        if (turns > MaxTurnsLead) stoppedBy = "turn_limit";
        _usage.Insert(0, new AgentUsage("lead", Math.Min(turns, MaxTurnsLead), inTok, outTok));

        var uncited = UncitedSentences(answer);
        return new ResearchResult(answer, answer.Length > 0 && uncited.Count == 0, uncited, _usage, _tokens, stoppedBy);
    }

    /// <summary>One worker: its own conversation, only the two policy tools, its own turn limit.</summary>
    async Task<string> RunWorker(string subtopic, string name, CancellationToken ct)
    {
        // TODO (Lab 3): one worker = a separate Claude conversation. Use the "Finish the worker" prompt in PROMPTS.md.
        // - Start a NEW history with only the subtopic (the worker sees nothing else).
        // - Loop up to MaxTurnsPerWorker: send with WorkerPrompt and WorkerTools only; stop early if _tokens >= TokenBudget.
        // - Count tokens: add res.Usage.InputTokens + OutputTokens to _tokens and to this worker's totals.
        // - Run search_policies / read_policy with SearchAsync / ReadAsync and return one tool_result per tool_use.
        // - Return the worker's final text (findings with policy ids).
        // - Always record usage: _usage.Add(new AgentUsage(name, turns, inTok, outTok)) — use try/finally.
        await Task.CompletedTask;
        throw new NotImplementedException("Lab 3: implement RunWorker in PolicyResearch.cs");
    }

    async Task<string> SearchAsync(string query)
    {
        var words = query.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var all = await db.Policies.ToListAsync();
        var hits = all.Where(p => words.Any(w => (p.Title + " " + p.Body).ToLowerInvariant().Contains(w))).Select(p => new { p.Id, p.Title }).Take(5);
        return JsonSerializer.Serialize(hits);
    }

    async Task<string> ReadAsync(string id) =>
        await db.Policies.FindAsync(id.Trim().ToUpperInvariant()) is { } p ? JsonSerializer.Serialize(new { p.Id, p.Title, p.Body }) : $"No policy with id {id}.";

    /// <summary>Every sentence must cite at least one policy id like [POL-03].</summary>
    public static List<string> UncitedSentences(string answer) =>
        Regex.Split(answer.Trim(), @"(?<=[.!?])\s+")
            .Select(s => s.Trim())
            .Where(s => s.Length > 3 && !Regex.IsMatch(s, @"\[POL-\d{2}(,\s*POL-\d{2})*\]"))
            .ToList();

    static string Str(IReadOnlyDictionary<string, JsonElement> input, string key) =>
        input.TryGetValue(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString()! : "";

    static string TextOf(Message m) =>
        string.Join("\n", m.Content.Select(b => b.TryPickText(out var t) ? t.Text : null).Where(s => !string.IsNullOrWhiteSpace(s)));

    static ToolUnion Tool(string name, string description, object properties, params string[] required) => new Tool
    {
        Name = name,
        Description = description,
        InputSchema = new InputSchema
        {
            Properties = JsonSerializer.SerializeToElement(properties).EnumerateObject().ToDictionary(p => p.Name, p => p.Value),
            Required = required,
        },
    };
}
