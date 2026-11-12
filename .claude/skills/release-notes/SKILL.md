---
name: release-notes
description: Write Acme HR release notes from git history. Use when asked for release notes, a changelog, or what shipped since the last release.
---

# Release notes

1. Run `bash .claude/skills/release-notes/scripts/commits.sh` to list commits since the last tag.
2. Group them under **Added**, **Fixed**, **Changed**. Skip merge commits and chores.
3. One line per change, plain English for HR users, ending with the area in brackets, e.g. `(Leave)`, `(Helpdesk)`, `(API)`.
4. Start with a title: `Acme HR release <date>`.
5. Keep it under 15 lines. Do not invent changes that are not in the commits.
