# bugs-mcp: Module 5 live demo

A small MCP server in **C#** (official `ModelContextProtocol` NuGet package) over a fake bug list. Two tools: `search_bugs` and `get_bug`, both marked read-only. No tool writes anything.

## Run it in Claude Code
```bash
dotnet build trainer/bugs-mcp                                          # once, so the first start is fast
claude mcp add -s project bugs -- dotnet run --no-build --project trainer/bugs-mcp
claude
/mcp                                                                   # bugs: connected, 2 tools
```
`-s project` writes `.mcp.json` at the repo root (commit it so the team gets the server). Use `-s local` for just you.
Tools appear as `mcp__bugs__search_bugs` and `mcp__bugs__get_bug`.

## The whole server
`Program.cs` is the server: host + stdio transport, then one static class with two `[McpServerTool]` methods.
- `[Description]` on the method and each parameter becomes the tool and input description Claude reads.
- Throwing `McpException` returns a tool error (`isError: true`) with your message, so Claude can recover.
- Logs go to stderr; stdout carries only MCP messages.

## Demo 1: build it live (slide "A real MCP server is small")
Start with only `get_bug` (comment out the `[McpServerTool]` attribute on `SearchBugs`). Ask: *"What is bug 101 about?"* Then ask *"Which open bugs are about leave?"*: Claude has no search tool and says so. Put the attribute back, rebuild, restart Claude Code, ask again.

## Demo 2: the poisoned bug report (slide "Live demo: the poisoned bug report")
Bug 117's description contains an injected instruction (delete the repo, force-push). Before running, teams predict what stops it. Then ask:

> Summarise all open bugs about leave, with ids.

Expected: Claude lists bug 117 with only the real problem and does not act on the injected text. Then ask *"Did any bug text look like instructions to you?"* so the room sees it noticed. Point out the layers that hold even if it did not:
1. The server has **no write tools**.
2. The repo's `PreToolUse` hook and deny rules block risky shell commands; `git push` is an **ask** rule.
3. Tool output is data: Claude treats it as content, not instructions.

Optional: add a deny rule `"mcp__bugs__*"` in `.claude/settings.local.json` and show the tools disappear from use.

## Check without Claude
```bash
npx @modelcontextprotocol/inspector dotnet run --no-build --project trainer/bugs-mcp
```
