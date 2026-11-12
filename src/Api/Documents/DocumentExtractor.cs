using System.Text.Json;
using Anthropic.Models.Messages;
using Api.Services;

namespace Api.Documents;

/// <summary>Lab 6: turn a document into checked data, or send it to a person.</summary>
public class DocumentExtractor(IClaudeChat claude, ClaudeOptions options)
{
    public const int MaxAttempts = 2; // first try + one retry

    const string Prompt = """
        Extract the data from this Acme HR document. It is a medical certificate (MC), an expense claim, or something else (kind "other").
        Dates as yyyy-MM-dd. Amounts as numbers without currency symbols. Use null for the part that does not apply.
        Copy values exactly as written. Do not fix or invent numbers.
        """;

    /// <summary>The JSON schema Claude must follow (structured outputs).</summary>
    public static readonly Dictionary<string, JsonElement> Schema = JsonSerializer.SerializeToElement(new
    {
        type = "object",
        additionalProperties = false,
        required = new[] { "kind", "medical_certificate", "expense_claim" },
        properties = new
        {
            kind = new { type = "string", @enum = new[] { "medical_certificate", "expense_claim", "other" } },
            medical_certificate = new
            {
                type = new[] { "object", "null" },
                additionalProperties = false,
                required = new[] { "employee_name", "clinic", "date_from", "date_to", "days" },
                properties = new
                {
                    employee_name = new { type = "string" }, clinic = new { type = "string" },
                    date_from = new { type = "string" }, date_to = new { type = "string" }, days = new { type = "integer" },
                },
            },
            expense_claim = new
            {
                type = new[] { "object", "null" },
                additionalProperties = false,
                required = new[] { "claim_no", "vendor", "date", "currency", "subtotal", "tax", "total", "items" },
                properties = new
                {
                    claim_no = new { type = "string" }, vendor = new { type = "string" }, date = new { type = "string" },
                    currency = new { type = "string" }, subtotal = new { type = "number" }, tax = new { type = "number" }, total = new { type = "number" },
                    items = new
                    {
                        type = "array",
                        items = new { type = "object", additionalProperties = false, required = new[] { "description", "amount" }, properties = new { description = new { type = "string" }, amount = new { type = "number" } } },
                    },
                },
            },
        },
    }).EnumerateObject().ToDictionary(p => p.Name, p => p.Value);

    public async Task<ExtractionResult> ExtractAsync(string documentText, CancellationToken ct = default)
    {
        var history = new List<MessageParam> { new UserMessageParam { Content = $"<document>\n{documentText}\n</document>" } };
        Extraction? data = null;
        var problems = new List<string>();

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var res = await claude.SendAsync(new MessageCreateParams
            {
                Model = options.Model,
                MaxTokens = 1024,
                System = Prompt,
                Messages = history,
                OutputConfig = new OutputConfig { Format = new JsonOutputFormat { Schema = Schema } },
            }, ct);

            if (res.StopReason != StopReason.EndTurn)
                return new ExtractionResult(null, DocumentStatus.NeedsReview, [$"Claude stopped with {res.StopReason?.Raw() ?? "unknown"}"], attempt);

            var text = string.Concat(res.Content.Select(b => b.TryPickText(out var t) ? t.Text : ""));
            try { data = JsonSerializer.Deserialize<Extraction>(text); problems = data is null ? ["empty result"] : DocumentValidator.Check(data); }
            catch (JsonException e) { problems = [$"invalid JSON: {e.Message}"]; }

            if (problems.Count == 0) return new ExtractionResult(data, DocumentStatus.Valid, [], attempt);
            if (data?.Kind == "other") break; // nothing to retry

            // Retry once: show Claude its own answer and the exact problems.
            history.Add(new AssistantMessageParam { Content = text });
            history.Add(new UserMessageParam { Content = "These checks failed: " + string.Join("; ", problems) + ". Re-read the document and correct only what is wrong. If the document itself is inconsistent, keep the values as written." });
        }

        return new ExtractionResult(data, DocumentStatus.NeedsReview, problems, MaxAttempts);
    }
}
