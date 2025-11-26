# Workflow: Create Story

## Trigger

User requests: "Create a story for X", "Write a user story", "Add story to SPWE-X"

## Step 1: Gather Information

Ask the user for:

1. **Story Name** - Clear, action-oriented title
2. **Parent Epic** - SPWE-X ID or search term
3. **Priority** - Critical, High, Medium, or Low
4. **User Story** - As a [role], I want [action], So that [benefit]

## Step 2: Fetch Parent Epic

### Search for Epic

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-search
arguments: { "query": "SPWE-1", "query_type": "internal" }
```

### Get Epic Details

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-fetch
arguments: { "id": "[Epic Page ID]" }
```

Extract from Epic:

- Strategic context
- Scope boundaries
- Related components/systems

## Step 3: Analyze Codebase

```bash
# Find existing components
rg -l "IComponentData" Assets/Scripts/Components/

# Find existing systems
rg -l "ISystem|SystemBase" Assets/Scripts/Systems/

# Find related code
rg "[FeatureName]" Assets/Scripts/ --type cs
```

## Step 4: Load Template

```
Read: .cursor/skills/refinement/templates/story.md
```

## Step 5: Generate Content

Fill the template with:

- **User Story** - As a / I want to / So that
- **Acceptance Criteria** - Observable, testable behaviors (numbered)
- **Technical Refinement**:
    - Components (name + purpose)
    - Systems (name + responsibility)
    - Data Flow diagram
    - Architectural Decisions (with rationale)
    - Integration Points
- **Risks** - Categorized with emojis
- **References** - Including parent Epic link

**Important:** NO implementation details (no code, no file paths)

## Step 6: Create in Notion

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-create-pages
arguments: {
  "parent": { "data_source_id": "28e56d55-129b-8156-85c3-000bea866839" },
  "pages": [{
    "properties": {
      "Name": "[Story Title]",
      "Priority": "[Priority]",
      "Status": "Not started",
      "Epic": "[Epic Page URL]"
    },
    "content": "[Generated Markdown Content]"
  }]
}
```

## Step 7: Confirm

Report to user:

> Story "[Name]" created successfully.
> URL: [Notion URL]
> Parent Epic: SPWE-X
> Priority: [Priority]
>
> Next steps:
>
> - Create Tasks for this Story
> - Review acceptance criteria with team

## Errors

| Error                     | Resolution                              |
| ------------------------- | --------------------------------------- |
| Epic not found            | Ask user for correct Epic ID            |
| Story already exists      | Ask if user wants to update             |
| Missing user story format | Help user structure As a/I want/So that |

## Quality Checklist

Before creating, verify:

- [ ] User story follows "As a... I want... So that..." format
- [ ] Acceptance criteria are testable (not implementation details)
- [ ] Components list WHAT data, not HOW
- [ ] Systems list WHAT they do, not HOW
- [ ] Data flow shows transformations, not code
- [ ] Architectural decisions include WHY
- [ ] NO code snippets or file paths
- [ ] Linked to parent Epic
