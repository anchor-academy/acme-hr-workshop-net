#!/usr/bin/env bash
# Lab 4: two features in parallel, one folder and one branch each.
set -e
base=$(git rev-parse --abbrev-ref HEAD)
git worktree add ../acme-wt-calendar -b feature/leave-calendar "$base"
git worktree add ../acme-wt-holidays -b feature/public-holidays "$base"
echo "Open two terminals:"
echo "  cd ../acme-wt-calendar && claude     # Feature A: leave calendar page"
echo "  cd ../acme-wt-holidays && claude     # Feature B: public holidays endpoint"
echo "When both are done: git merge feature/leave-calendar feature/public-holidays"
