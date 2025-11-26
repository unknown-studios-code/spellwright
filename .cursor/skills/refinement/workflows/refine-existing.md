# Workflow: Refine Existing Artifact

## Trigger

User requests:

- "Refine SPWT-X"
- "Update the task SPWT-X"
- "Add more details to SPWS-X"
- "Improve the epic SPWE-X"

## Step 1: Identify Artifact

Parse the ID from user request:

| Pattern | Type  |
| ------- | ----- |
| SPWE-X  | Epic  |
| SPWS-X  | Story |
| SPWT-X  | Task  |

## Step 2: Fetch Current Content

### Search for Artifact

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-search
arguments: { "query": "[ARTIFACT-ID]", "query_type": "internal" }
```

### Get Full Details

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-fetch
arguments: { "id": "[Page ID from search]" }
```

Extract:

- Current content
- Properties (Priority, Status, Type)
- Parent/child relationships
- Existing sections

## Step 3: Identify Gaps

Compare against template checklist:

### Epic Gaps

- [ ] Strategic Goal clear?
- [ ] Problem quantified?
- [ ] Value Hypothesis complete?
- [ ] Scope well-defined?
- [ ] Metrics SMART?
- [ ] Risks categorized?

### Story Gaps

- [ ] User Story format correct?
- [ ] Acceptance criteria testable?
- [ ] Components/Systems listed?
- [ ] Data flow documented?
- [ ] Architectural decisions have rationale?

### Task Gaps

- [ ] Definition of Done complete?
- [ ] File paths exact?
- [ ] Dependencies listed?
- [ ] Performance targets specified?
- [ ] Risks addressed?

## Step 4: Gather Additional Information

Ask user about gaps:

> I found the following sections that could be improved:
>
> 1. [Gap 1]
> 2. [Gap 2]
>
> Would you like me to help fill these in?

## Step 5: Analyze Codebase (for Tasks)

If refining a Task, analyze current codebase state:

```bash
# Check if files already exist
ls -la [suggested paths from task]

# Find related code changes
git log --oneline -10 -- Assets/Scripts/

# Find dependencies
rg "[ComponentName]" Assets/Scripts/
```

## Step 6: Update Content

### Update Properties

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-update-page
arguments: {
  "data": {
    "page_id": "[Page ID]",
    "command": "update_properties",
    "properties": {
      "Priority": "[Updated Priority]",
      "Status": "[Updated Status]"
    }
  }
}
```

### Replace Content Section

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-update-page
arguments: {
  "data": {
    "page_id": "[Page ID]",
    "command": "replace_content_range",
    "selection_with_ellipsis": "### ⚙️ Technical...---",
    "new_str": "[Updated Section Content]"
  }
}
```

### Insert Additional Content

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-update-page
arguments: {
  "data": {
    "page_id": "[Page ID]",
    "command": "insert_content_after",
    "selection_with_ellipsis": "### ⚠️ Potential Risks...",
    "new_str": "\n- 🔴 **New Risk:** [Description]\n    - Mitigation: [How to address]"
  }
}
```

## Step 7: Confirm Changes

Report to user:

> Updated [Artifact Type] "[Name]"
>
> Changes made:
>
> - [Change 1]
> - [Change 2]
>
> URL: [Notion URL]

## Errors

| Error                       | Resolution                               |
| --------------------------- | ---------------------------------------- |
| Artifact not found          | Verify ID format and search again        |
| Content selection not found | Use broader selection or replace_content |
| Concurrent edit conflict    | Fetch latest and retry                   |

## Tips

- Use `replace_content_range` for updating specific sections
- Use `insert_content_after` for adding new items
- Use `replace_content` only for complete rewrites
- Always fetch current content before updating
