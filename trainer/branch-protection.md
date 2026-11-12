# Branch protection for Lab 5

GitHub Free supports required checks on **public** repos. Each team repo created from the template is public.

1. Let `ci` and `claude-review` run once (open any pull request).
2. Settings → Branches → Add branch ruleset (or classic rule) for `main`:
   - Require a pull request before merging
   - Require status checks to pass: **ci**, **claude-review**
   - Block force pushes
3. Test: open the PR from `lab5-planted-bug`. `ci` passes, `claude-review` fails, Merge is blocked.
