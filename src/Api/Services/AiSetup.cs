using Anthropic;

namespace Api.Services;

public static class AiSetup
{
    /// <summary>Registers the Claude client. Reads ANTHROPIC_API_KEY from the environment.</summary>
    public static IServiceCollection AddAcmeAi(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton(_ => new AnthropicClient());
        services.AddSingleton<IClaudeChat, AnthropicClaudeChat>();
        services.AddScoped<Api.Helpdesk.HelpdeskTools>();
        services.AddScoped<Api.Helpdesk.HelpdeskAgent>();
        services.AddSingleton(new ClaudeOptions(config["Claude:Model"] ?? "claude-sonnet-5-5"));
        return services;
    }
}

public record ClaudeOptions(string Model);
