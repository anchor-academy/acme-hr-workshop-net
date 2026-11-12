#!/usr/bin/env bash
# PostToolUse hook: run tests after every edit. Exit 2 sends the failure back to Claude.
# Change TEST_CMD to your project's test command.
TEST_CMD=${TEST_CMD:-"dotnet test --nologo -v q"}
out=$($TEST_CMD 2>&1)
if [ $? -ne 0 ]; then
  echo "Tests failing after this edit:" >&2
  echo "$out" | grep -E "\[FAIL\]|error CS|Assert\.|Expected|Actual|Failed!|FAIL" | head -20 >&2
  exit 2
fi
exit 0
