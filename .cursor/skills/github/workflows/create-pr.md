# Workflow: Create Pull Request

## Trigger

User requests: "Create a PR", "Open a pull request"

## Step 1: Gather State

```bash
git branch --show-current
git log develop..HEAD --oneline
git diff develop...HEAD --name-only
git diff develop...HEAD
```

## Step 2: Extract Branch Info

Parse branch for Task ID, Story ID, Type. See [mappings.md](../references/mappings.md#footers-from-branch-name).

## Step 3: Fetch from Notion

Get task details for PR body. See [notion.md](../references/notion.md).

Extract: Title, Priority, Definition of Done.

## Step 4: Push if Needed

```bash
git push -u origin $(git branch --show-current)
```

## Step 5: Generate Title

From commits: use main commit subject or summarize multiple.

Format: `type(scope): subject`

## Step 6: Determine Labels

See [mappings.md](../references/mappings.md#labels-from-file-paths) and [mappings.md](../references/mappings.md#type-labels-from-branchcommit).

Need 3-5 labels: Type (1) + Priority (1) + Context (1-2).

## Step 7: Generate Body

Use template from [conventions.md](../references/conventions.md#body-template).

Fill with: Task info, changes summary, files tree, DoD from Notion.

## Step 8: Execute

```bash
gh pr create \
  --title "type(scope): subject" \
  --body "$(cat <<'EOF'
PR body here
EOF
)" \
  --base develop \
  --label "type: tech" \
  --label "priority: medium" \
  --label "dots: system"
```

## Step 9: Update Notion

Update `Pull Request` property. See [notion.md](../references/notion.md#update-task-with-pr).

## Step 10: Confirm

> PR created: https://github.com/.../pull/42
> Labels: type: tech, priority: medium, dots: system
> Notion updated with PR link.

## Errors

| Error         | Resolution                |
| ------------- | ------------------------- |
| PR exists     | Provide existing PR link  |
| No commits    | Inform nothing to create  |
| Label missing | Use existing labels, warn |
