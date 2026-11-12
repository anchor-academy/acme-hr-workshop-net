#!/usr/bin/env bash
# Commits since the last tag (or the last 30 if there is no tag).
last=$(git describe --tags --abbrev=0 2>/dev/null)
if [ -n "$last" ]; then git log --no-merges --pretty='- %s' "$last"..HEAD; else git log --no-merges --pretty='- %s' -30; fi
