# Workflow: Create Branch

## Trigger

User requests: "Create a branch for SPWT-X", "Start working on SPWT-X"

## Step 1: Get Task Info

### From Notion (preferred)

Search and fetch task details. See [notion.md](../references/notion.md).

Extract: Task ID, Story ID, Task Type, Priority.

### From User

If Notion unavailable, get from user: Story ID, Task ID (optional), Branch Type.

## Step 2: Verify Clean State

```bash
git status --porcelain
# Must be empty
```

If not clean: inform user to commit or stash first.

## Step 3: Determine Branch Name

See [mappings.md](../references/mappings.md) for type mapping.

**Story branch:** `feature/develop/SPWS-X`

**Task branch:** `<type>/SPWS-X/SPWT-X`

## Step 4: Execute

```bash
npx ts-node .cursor/skills/github/scripts/create-branch.ts \
  --type <type> --story SPWS-X --task SPWT-X
```

Or directly:

```bash
git fetch origin develop
git checkout develop && git pull origin develop
git checkout -b <branch-name>
git push -u origin <branch-name>
```

## Step 5: Update Notion

Update `Branch` property with GitHub URL. See [notion.md](../references/notion.md).

## Step 6: Confirm

> Branch `tech/SPWS-4/SPWT-13` created and pushed.
> Notion updated with branch link.

## Errors

| Error               | Resolution                 |
| ------------------- | -------------------------- |
| Branch exists       | Offer to checkout existing |
| Directory not clean | Ask to commit/stash        |
| Invalid ID format   | Ask for correct format     |
