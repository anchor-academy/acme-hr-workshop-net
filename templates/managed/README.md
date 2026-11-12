# Managed settings

Managed settings are company policy for Claude Code. Nothing in user, project or local settings can override a managed key.
Order (highest first): **managed** → command line `--settings` → project local → shared project (`.claude/settings.json`) → user (`~/.claude/settings.json`).
Lists such as `permissions.deny` merge across levels, so a team can add rules but not remove company ones.

## How to deliver
- **Server-managed settings** (Team / Enterprise): set the JSON in the claude.ai admin console. Reaches every signed-in user, including cloud sessions. Easiest start.
- **File on each machine** (via MDM or your device tool): `managed-settings.json` in the system folder:
  - macOS: `/Library/Application Support/ClaudeCode/managed-settings.json`
  - Linux / WSL: `/etc/claude-code/managed-settings.json`
  - Windows: `C:\Program Files\ClaudeCode\managed-settings.json`
  Check the current paths in the docs before rollout: https://code.claude.com/docs/en/managed-settings

## What the example does
- Denies reads of `.env`, `secrets/`, SSH and cloud keys on every machine.
- Asks before `git push`.
- `disableBypassPermissionsMode: "disable"` removes the "skip all permissions" mode.
- `allowManagedPermissionRulesOnly`: set to `true` only when you want company rules to be the **only** rules (teams lose their own allow lists). Start with `false`.
- Turns on telemetry to your collector (see `../telemetry/`).

## Check it
Run `/status` in Claude Code: `Setting sources` names the managed source. `claude doctor` lists entries it rejected.
Use company Claude accounts (Team / Enterprise). Personal subscriptions do not receive server-managed settings.
