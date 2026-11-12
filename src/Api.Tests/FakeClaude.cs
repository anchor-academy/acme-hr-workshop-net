using System.Text.Json;
using Anthropic.Models.Messages;
using Api.Services;

namespace Api.Tests;

/// <summary>Plays back scripted Claude replies so agent code can be tested without an API key.</summary>
public class FakeClaude(params string[] replies) : IClaudeChat
{
    readonly Queue<string> _replies = new(replies);
    public List<MessageCreateParams> Requests { get; } = [];

    public Task<Message> SendAsync(MessageCreateParams request, CancellationToken ct = default)
    {
        Requests.Add(request);
        var json = _replies.Count > 0 ? _replies.Dequeue() : Text("(no more scripted replies)");
        return Task.FromResult(JsonSerializer.Deserialize<Message>(json)!);
    }

    static string Msg(string stop, string contentJson) =>
        $$"""{"id":"msg_test","type":"message","role":"assistant","model":"claude-sonnet-5-5","stop_reason":"{{stop}}","stop_sequence":null,"usage":{"input_tokens":100,"output_tokens":20},"content":[{{contentJson}}]}""";

    public static string Text(string text) => Msg("end_turn", $$"""{"type":"text","text":{{JsonSerializer.Serialize(text)}}}""");

    public static string ToolUse(string id, string name, object input) =>
        Msg("tool_use", $$"""{"type":"tool_use","id":"{{id}}","name":"{{name}}","input":{{JsonSerializer.Serialize(input)}}}""");
}
