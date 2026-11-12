# Lab 4: two features in parallel, one folder and one branch each.
$base = git rev-parse --abbrev-ref HEAD
git worktree add ../acme-wt-calendar -b feature/leave-calendar $base
git worktree add ../acme-wt-holidays -b feature/public-holidays $base
Write-Host "Each worktree is a fresh folder: run 'cd web; npm install' in ../acme-wt-calendar before building the page."
Write-Host "Open two terminals:"
Write-Host "  cd ../acme-wt-calendar; claude     # Feature A: leave calendar page"
Write-Host "  cd ../acme-wt-holidays; claude     # Feature B: public holidays endpoint"
Write-Host "When both are done: git merge feature/leave-calendar feature/public-holidays"
