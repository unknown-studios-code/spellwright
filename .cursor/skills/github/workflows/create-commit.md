# Workflow: Create Commit

## Trigger

User requests: "Commit the changes", "Create a commit"

## Step 1: Analyze State

```bash
git branch --show-current
git diff --cached --name-only
git diff --cached
```

## Step 2: Extract Footers from Branch

Parse branch name for `Implements` and `Part of`. See [mappings.md](../references/mappings.md#footers-from-branch-name).

Example: `tech/SPWS-4/SPWT-13` → Implements: SPWT-13, Part of: SPWS-4

## Step 3: Determine Type

Analyze diff. See [mappings.md](../references/mappings.md#commit-type-from-changes).

## Step 4: Determine Scope

Map file paths to scope. See [mappings.md](../references/mappings.md#scope-from-file-paths).

## Step 5: Generate Subject

- Imperative mood: "add", "fix", "update"
- Lowercase, no period
- Under 72 characters

## Step 6: Execute

```bash
npx ts-node .cursor/skills/github/scripts/create-commit.ts \
  <type> "<subject>" --scope <scope>
```

Footers auto-detected from branch. Or directly:

```bash
HUSKY=0 git commit -m "type(scope): subject" \
  -m "Implements: SPWT-X" \
  -m "Part of: SPWS-X"
```

## Step 7: Confirm

Show the created commit message.

## Errors

| Error             | Resolution       |
| ----------------- | ---------------- |
| No staged changes | Ask to stage all |
| Subject too long  | Shorten          |
| Hook fails        | Fix issue, retry |
