# Team visibility of Claude Code usage

Claude Code can export usage to any OpenTelemetry collector (Grafana, Datadog, Honeycomb, Azure Monitor, etc.).

## Metrics you get (names may change: check https://code.claude.com/docs/en/monitoring-usage)
- Sessions started, active time
- Tokens used and cost (by model)
- Lines of code added and removed
- Commits and pull requests created
- Accept / reject decisions on edits

## Other views
- **Claude Console** → Usage and Cost: API spend per workspace and key. Set a monthly **spend limit** per workspace (e.g. one for CI).
- **Team / Enterprise analytics** in the claude.ai admin console: seats and activity.

## Monthly review (suggested)
Cost per team and per CI job · runs that hit `--max-turns` · PRs blocked by the Claude review · top repos by usage.

## PDPA note
Prompt text is **not** exported unless `OTEL_LOG_USER_PROMPTS=1`. Keep it off: prompts can contain customer personal data.
Telemetry carries user identifiers (account, email): tell staff it is collected, keep retention short, and restrict who can view it.
