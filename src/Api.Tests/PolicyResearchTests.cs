using Api.Research;
using Api.Services;

namespace Api.Tests;

public class PolicyResearchTests
{
    static PolicyResearch Make(FakeClaude fake) => new(fake, TestDb.Create(), new ClaudeOptions("claude-sonnet-5-5"));

    [Fact]
    public void Validator_flags_uncited_sentence()
    {
        var bad = PolicyResearch.UncitedSentences("You can carry forward 5 days [POL-04]. Half days are fine.");
        Assert.Single(bad);
    }

    [Fact]
    public void Validator_accepts_all_cited()
    {
        Assert.Empty(PolicyResearch.UncitedSentences("You can carry forward 5 days [POL-04]. Half days deduct 0.5 [POL-03, POL-04]."));
    }

    [Fact]
    public async Task Lead_runs_a_worker_and_returns_cited_answer()
    {
        var fake = new FakeClaude(
            FakeClaude.ToolUse("l1", "research", new { subtopic = "carry forward" }),   // lead
            FakeClaude.ToolUse("w1", "search_policies", new { query = "carry" }),     // worker
            FakeClaude.Text("Up to 5 days by 31 March [POL-04]"),                      // worker done
            FakeClaude.Text("You can carry forward up to 5 days [POL-04]."));          // lead answer
        var r = await Make(fake).AskAsync("Can I carry forward leave?");
        Assert.True(r.CitationsValid);
        Assert.Contains(r.Agents, a => a.Agent == "worker-1");
        Assert.True(r.TotalTokens > 0);
    }

    [Fact]
    public async Task Worker_limit_is_enforced()
    {
        var calls = Enumerable.Range(1, 5).Select(i => FakeClaude.ToolUse($"l{i}", "research", new { subtopic = $"t{i}" })).ToList();
        var replies = new List<string>();
        foreach (var c in calls) { replies.Add(c); replies.Add(FakeClaude.Text("finding [POL-01]")); }
        var fake = new FakeClaude([.. replies, FakeClaude.Text("Done [POL-01].")]);
        var r = await Make(fake).AskAsync("Many topics");
        Assert.True(r.Agents.Count(a => a.Agent.StartsWith("worker")) <= PolicyResearch.MaxWorkers);
    }
}
