# Workflow: Create Epic

## Trigger

User requests: "Create an epic for X", "Write an epic", "Generate an epic for [feature]"

## Step 1: Gather Information

Ask the user for:

1. **Epic Name** - Clear, concise title
2. **Priority** - Critical, High, Medium, or Low
3. **Strategic Goal** - 2-3 sentence elevator pitch
4. **Problem Statement** - What problem and for whom
5. **Expected Outcomes** - What success looks like

If the user provides a description, extract information from it.

## Step 2: Analyze Context

### From Codebase (optional)

```bash
# Find related systems/components
rg -l "interface.*System" Assets/Scripts/
rg -l "IComponentData" Assets/Scripts/
```

### From Existing Documentation

Read any referenced documents to understand scope.

## Step 3: Load Template

Read the template file:

```
Read: .cursor/skills/refinement/templates/epic.md
```

## Step 4: Generate Content

Fill the template with:

- **Strategic Goal** - From user input
- **Problem Statement** - What + Who
- **Value Hypothesis** - Action → Outcome → Measurement
- **Scope** - In Scope (numbered) + Out of Scope (bullets)
- **Success Criteria** - Quantitative metrics + Qualitative criteria
- **Risks** - Categorized with 🔴🟡🟠🔵 emojis
- **References** - Design files, docs, external resources

## Step 5: Create in Notion

### Search for Existing

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-search
arguments: { "query": "[Epic Name]", "query_type": "internal" }
```

### Create Epic Page

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-create-pages
arguments: {
  "parent": { "data_source_id": "28e56d55-129b-8189-827c-000b7bda32ac" },
  "pages": [{
    "properties": {
      "Name": "[Epic Title]",
      "Priority": "[Critical/High/Medium/Low]",
      "Status": "Not started"
    },
    "content": "[Generated Markdown Content]"
  }]
}
```

## Step 6: Confirm

Report to user:

> Epic "[Name]" created successfully.
> URL: [Notion URL]
> Priority: [Priority]
>
> Next steps:
>
> - Create User Stories for this Epic
> - Add design documentation links

## Errors

| Error                 | Resolution                           |
| --------------------- | ------------------------------------ |
| Epic already exists   | Ask if user wants to update existing |
| Missing required info | Ask user for missing information     |
| Notion API error      | Check MCP connection, retry          |

## Quality Checklist

Before creating, verify:

- [ ] Strategic Goal is clear elevator pitch (2-3 sentences)
- [ ] Problem includes quantifiable impact
- [ ] Value Hypothesis has Action → Outcome → Measurement
- [ ] Scope separates IN vs OUT clearly
- [ ] Metrics are SMART
- [ ] Risks categorized with emojis
- [ ] Written in English
