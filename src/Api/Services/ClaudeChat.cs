using Anthropic;
using Anthropic.Models.Messages;

namespace Api.Services;

/// <summary>One call to the Claude Messages API. An interface so tests can use a fake.</summary>
public interface IClaudeChat
{
    Task<Message> SendAsync(MessageCreateParams request, CancellationToken ct = default);
}

public class AnthropicClaudeChat(AnthropicClient client) : IClaudeChat
{
    public Task<Message> SendAsync(MessageCreateParams request, CancellationToken ct = default) =>
        client.Messages.Create(request, ct);
}
