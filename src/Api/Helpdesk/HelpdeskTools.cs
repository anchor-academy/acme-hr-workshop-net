using System.Text.Json;
using Anthropic.Models.Messages;
using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Api.Helpdesk;

public record ToolOutcome(string Json, bool IsError);

/// <summary>The four tools the HR helpdesk agent may use. Claude only asks; this code decides.</summary>
public class HelpdeskTools(AcmeDb db, LeaveService leave)
{
    public static readonly IReadOnlyList<ToolUnion> Definitions =
    [
        Def("get_leave_balance", "Get the leave balance (entitlement, used, pending, available days) of the current employee.", new { }),
        Def("apply_leave", "Apply for annual leave for the current employee. Dates are yyyy-MM-dd. half_day is None, AM or PM. Use only after the employee has clearly asked for specific dates.",
            new { start_date = Str("First day, yyyy-MM-dd"), end_date = Str("Last day, yyyy-MM-dd"), half_day = new { type = "string", @enum = new[] { "None", "AM", "PM" } }, reason = Str("Short reason") },
            "start_date", "end_date"),
        Def("check_policy", "Search Acme HR policies by keyword and return matching policies with their ids. Use it before answering any policy question.",
            new { topic = Str("Keyword, for example 'carry forward' or 'medical'") }, "topic"),
        Def("escalate_to_hr", "Hand the conversation to a human HR officer. Use when the request is unclear, sensitive, outside policy, or you are not sure.",
            new { reason = Str("Why a person is needed") }, "reason"),
    ];

    public async Task<ToolOutcome> RunAsync(string name, IReadOnlyDictionary<string, JsonElement> input, int employeeId)
    {
        try
        {
            return name switch
            {
                "get_leave_balance" => Ok(await leave.GetBalanceAsync(employeeId)),
                "apply_leave" => await ApplyLeaveAsync(input, employeeId),
                "check_policy" => await CheckPolicyAsync(Get(input, "topic")),
                "escalate_to_hr" => Ok(new { status = "escalated", message = "An HR officer will reply within 1 working day.", reason = Get(input, "reason") }),
                _ => Error($"Unknown tool {name}."),
            };
        }
        catch (LeaveException e) { return Error(e.Message); }
        catch (FormatException) { return Error("Dates must be in yyyy-MM-dd format."); }
    }

    async Task<ToolOutcome> ApplyLeaveAsync(IReadOnlyDictionary<string, JsonElement> input, int employeeId)
    {
        var start = DateOnly.ParseExact(Get(input, "start_date"), "yyyy-MM-dd");
        var end = DateOnly.ParseExact(Get(input, "end_date"), "yyyy-MM-dd");
        var half = Enum.TryParse<HalfDay>(Get(input, "half_day"), true, out var h) ? h : HalfDay.None;
        var request = await leave.CreateAsync(new CreateLeave(employeeId, start, end, half, Get(input, "reason")));

        // TODO (Lab 1): the approval rule belongs here, in code, not in the prompt (POL-02, POL-14).
        // More than 3 working days must return status "needs_manager_approval" (the request stays Pending for the manager).
        return Ok(new { status = request.Status.ToString().ToLowerInvariant(), request_id = request.Id, days = LeaveService.DaysOf(request) });
    }

    async Task<ToolOutcome> CheckPolicyAsync(string topic)
    {
        var words = topic.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var all = await db.Policies.ToListAsync();
        var hits = all.Where(p => words.Any(w => (p.Title + " " + p.Body).ToLowerInvariant().Contains(w)))
            .Take(3).Select(p => new { p.Id, p.Title, p.Body }).ToList();
        return Ok(hits.Count > 0 ? hits : new object[] { new { message = "No matching policy. Escalate to HR." } });
    }

    static ToolOutcome Ok(object? value) => new(JsonSerializer.Serialize(value), false);
    static ToolOutcome Error(string message) => new(JsonSerializer.Serialize(new { error = message }), true);
    static string Get(IReadOnlyDictionary<string, JsonElement> input, string key) =>
        input.TryGetValue(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString()! : "";
    static object Str(string description) => new { type = "string", description };

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
}
