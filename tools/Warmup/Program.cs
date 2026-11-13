// Module 2 warm-up: the smallest complete tool-use loop, with one tool.
// Run:  dotnet run --project tools/Warmup -- "How many leave days do I have?"
// Needs ANTHROPIC_API_KEY in this terminal. Model from CLAUDE_MODEL, default claude-sonnet-5-5.
using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;

var question = args.Length > 0 ? string.Join(' ', args) : "How many leave days do I have left?";
var model = Environment.GetEnvironmentVariable("CLAUDE_MODEL") ?? "claude-sonnet-5-5";
const int MaxTurns = 5;

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")))
{
    Console.WriteLine("Set ANTHROPIC_API_KEY in this terminal first (never in code).");
    return;
}
AnthropicClient client = new(); // reads ANTHROPIC_API_KEY

List<ToolUnion> tools =
[
    Def("get_leave_balance", "Get the annual leave balance of the current employee, in working days.", new { }),
    // TODO (warm-up): add get_public_holidays(state) here and in RunTool below.
];

var history = new List<MessageParam> { new UserMessageParam { Content = question } };

for (var turn = 1; turn <= MaxTurns; turn++)
{
    var res = await client.Messages.Create(new MessageCreateParams
    {
        Model = model,
        MaxTokens = 1024,
        System = "You are the Acme HR helpdesk. Use the tools; never guess numbers. Today is " + DateTime.Today.ToString("yyyy-MM-dd") + ".",
        Tools = tools,
        Messages = history,
    });
    Console.WriteLine($"[turn {turn}] stop_reason={res.StopReason?.Raw()} in={res.Usage.InputTokens} out={res.Usage.OutputTokens}");

    // The API is stateless: keep Claude's turn in the history and resend it.
    history.Add(new AssistantMessageParam { Content = res.Content.Select(b => new ContentBlockParam(b.Json)).ToList() });

    if (res.StopReason != StopReason.ToolUse)
    {
        foreach (var b in res.Content) if (b.TryPickText(out var t)) Console.WriteLine("\n" + t.Text);
        return;
    }

    // Claude asked for tools. Our code runs them and answers every tool_use id.
    var results = new List<ContentBlockParam>();
    foreach (var b in res.Content)
    {
        if (!b.TryPickToolUse(out var call)) continue;
        var (output, isError) = RunTool(call.Name, call.Input);
        Console.WriteLine($"  tool_use {call.Name}({JsonSerializer.Serialize(call.Input)}) -> {output}");
        results.Add(new ToolResultBlockParam(call.ID) { Content = output, IsError = isError });
    }
    history.Add(new UserMessageParam { Content = results });
}
Console.WriteLine("Stopped: turn limit reached.");

static (string Output, bool IsError) RunTool(string name, IReadOnlyDictionary<string, JsonElement> input) => name switch
{
    "get_leave_balance" => ("""{"entitlement":16,"used":5.5,"pending":2,"available":8.5}""", false),
    _ => ($"Unknown tool {name}.", true),
};

static ToolUnion Def(string name, string description, object properties, params string[] required) => new Tool
{
    Name = name,
    Description = description,
    InputSchema = new InputSchema
    {
        Properties = JsonSerializer.SerializeToElement(properties).EnumerateObject().ToDictionary(p => p.Name, p => p.Value),
        Required = required,
    },
};
