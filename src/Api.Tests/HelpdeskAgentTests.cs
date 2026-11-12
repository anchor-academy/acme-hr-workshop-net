using Anthropic.Models.Messages;
using Api.Helpdesk;
using Api.Services;

namespace Api.Tests;

public class HelpdeskAgentTests
{
    static (HelpdeskAgent agent, FakeClaude fake) Make(params string[] replies)
    {
        var db = TestDb.Create();
        var fake = new FakeClaude(replies);
        return (new HelpdeskAgent(fake, new HelpdeskTools(db, new LeaveService(db)), new ClaudeOptions("claude-sonnet-5-5")), fake);
    }

    static readonly List<ChatTurn> Ask = [new("user", "How many days do I have?")];

    [Fact]
    public async Task Plain_answer_ends_after_one_turn()
    {
        var (agent, _) = Make(FakeClaude.Text("You have 14 days."));
        var r = await agent.RunAsync(1, Ask);
        Assert.Equal("You have 14 days.", r.Reply);
        Assert.Equal(1, r.Turns);
    }

    [Fact]
    public async Task Tool_call_then_answer()
    {
        var (agent, fake) = Make(FakeClaude.ToolUse("t1", "get_leave_balance", new { }), FakeClaude.Text("You have 14 days."));
        var r = await agent.RunAsync(1, Ask);
        Assert.Single(r.ToolCalls);
        Assert.Equal(2, r.Turns);
        // Second request resends history: user, assistant (tool_use), user (tool_result)
        Assert.Equal(3, fake.Requests[1].Messages.Count);
    }

    [Fact]
    public async Task Five_day_leave_needs_manager_approval_from_code()
    {
        var (agent, _) = Make(
            FakeClaude.ToolUse("t1", "apply_leave", new { start_date = "2026-11-16", end_date = "2026-11-20", half_day = "None" }),
            FakeClaude.Text("Sent to your manager."));
        var r = await agent.RunAsync(1, Ask);
        Assert.Contains("needs_manager_approval", r.ToolCalls[0].Output);
    }

    [Fact]
    public async Task Tool_error_is_returned_not_thrown()
    {
        var (agent, _) = Make(
            FakeClaude.ToolUse("t1", "apply_leave", new { start_date = "16/11/2026", end_date = "2026-11-20" }),
            FakeClaude.Text("Please use yyyy-MM-dd."));
        var r = await agent.RunAsync(1, Ask);
        Assert.True(r.ToolCalls[0].IsError);
    }

    [Fact]
    public async Task Stops_at_turn_limit()
    {
        var loop = Enumerable.Range(0, 20).Select(i => FakeClaude.ToolUse($"t{i}", "get_leave_balance", new { })).ToArray();
        var (agent, _) = Make(loop);
        var r = await agent.RunAsync(1, Ask);
        Assert.Equal(HelpdeskAgent.MaxTurns, r.Turns);
        Assert.Equal("turn_limit", r.StopReason);
    }
}
