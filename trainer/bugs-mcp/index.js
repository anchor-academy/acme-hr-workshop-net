// Module 5 live demo: a small MCP server over a fake bug list.
// Read-only on purpose: there is no tool that writes, so even a fooled agent cannot change data.
// Record 117 contains a prompt injection for the "poisoned bug report" demo.
import { readFileSync } from "node:fs";
import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";

const bugs = JSON.parse(readFileSync(new URL("./bugs.json", import.meta.url), "utf8")); // loaded once, never written

const server = new McpServer({ name: "bugs", version: "1.0.0" });
const text = (value) => ({ content: [{ type: "text", text: typeof value === "string" ? value : JSON.stringify(value, null, 2) }] });

server.registerTool(
  "get_bug",
  {
    description: "Get one bug by its numeric id. Use search_bugs first if you do not know the id.",
    inputSchema: { id: z.number().int().describe("Bug id, for example 117") },
  },
  async ({ id }) => {
    const bug = bugs.find((b) => b.id === id);
    if (!bug) return { isError: true, ...text(`Bug ${id} not found. Use search_bugs to find the id.`) };
    return text(bug);
  },
);

server.registerTool(
  "search_bugs",
  {
    description: "Search bugs by keyword in the title or description. Optional status filter. Returns id, title and status only; use get_bug for details.",
    inputSchema: {
      query: z.string().min(2).describe("Keyword, for example 'leave' or 'calendar'"),
      status: z.enum(["open", "closed", "any"]).default("any"),
    },
  },
  async ({ query, status }) => {
    const q = query.toLowerCase();
    const hits = bugs
      .filter((b) => status === "any" || b.status === status)
      .filter((b) => `${b.title} ${b.description}`.toLowerCase().includes(q))
      .map(({ id, title, status }) => ({ id, title, status }));
    return text(hits.length ? hits : `No bugs match "${query}". Try a shorter keyword.`);
  },
);

await server.connect(new StdioServerTransport());
