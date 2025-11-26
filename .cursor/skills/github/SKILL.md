---
name: github
description: Execute GitHub CLI operations including branch creation, commits, and pull requests following Spellwright project standards. Use when creating branches, making commits, creating PRs, or any GitHub-related task. Zero-input automation where the agent gathers all required information.
---

# GitHub CLI Skill

Zero-input automation for GitHub operations. The agent gathers information from Git, branch names, code analysis, and Notion - no manual input required.

## Prerequisites

- GitHub CLI installed and authenticated (`gh auth status`)
- Git repository initialized
- Notion MCP enabled (optional, enhances automation)

## Workflows

| Task          | Workflow                                                 |
| ------------- | -------------------------------------------------------- |
| Create Branch | [workflows/create-branch.md](workflows/create-branch.md) |
| Create Commit | [workflows/create-commit.md](workflows/create-commit.md) |
| Create PR     | [workflows/create-pr.md](workflows/create-pr.md)         |

## Quick Commands

```bash
# Branch
npx ts-node .cursor/skills/github/scripts/create-branch.ts \
  --type <type> --story <SPWS-X> [--task <SPWT-X>]

# Commit (footers auto-detected from branch)
npx ts-node .cursor/skills/github/scripts/create-commit.ts \
  <type> "<subject>" [--scope <scope>]

# PR (auto-pushes before creating)
npx ts-node .cursor/skills/github/scripts/create-pr.ts \
  --title "<title>" --body "<body>" --label "<label>"
```

## Utilities

```bash
# Cleanup merged branches (only deletes branches with merged PR)
npx ts-node .cursor/skills/github/scripts/cleanup-branches.ts --dry-run     # Preview
npx ts-node .cursor/skills/github/scripts/cleanup-branches.ts -y            # Delete
npx ts-node .cursor/skills/github/scripts/cleanup-branches.ts -y --skip-pr-check  # Skip PR check
```

## Analysis Commands

Run these before operations to gather context:

```bash
# Branch info (for footers)
git branch --show-current

# Staged files (for scope/type)
git diff --cached --name-only

# Staged diff (for subject/body)
git diff --cached

# PR analysis
git diff develop...HEAD --name-only
git log develop..HEAD --oneline
```

## Commit with Husky Disabled

```bash
HUSKY=0 git commit -m "type(scope): subject"
```

## References

| Reference                                   | Content                                    |
| ------------------------------------------- | ------------------------------------------ |
| [mappings.md](references/mappings.md)       | Inference tables for types, scopes, labels |
| [conventions.md](references/conventions.md) | Branch, commit, PR format standards        |
| [notion.md](references/notion.md)           | Notion MCP integration                     |

## Scripts

| Script                        | Purpose                                     |
| ----------------------------- | ------------------------------------------- |
| `scripts/create-branch.ts`    | Create branch with CLI args                 |
| `scripts/create-commit.ts`    | Create commit with auto-footers             |
| `scripts/create-pr.ts`        | Create PR with auto-push                    |
| `scripts/cleanup-branches.ts` | Delete merged branches (requires merged PR) |

## Lib Modules

| Module              | Purpose                               |
| ------------------- | ------------------------------------- |
| `lib/cli.ts`        | Argument parsing, output formatting   |
| `lib/git.ts`        | Git operations (branch, commit, push) |
| `lib/github.ts`     | GitHub CLI operations (PR queries)    |
| `lib/types.ts`      | Shared types and constants            |
| `lib/validation.ts` | ID and input validation               |
