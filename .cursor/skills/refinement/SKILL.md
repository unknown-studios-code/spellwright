---
name: refinement
description: Automate agile artifact creation and refinement in Notion. Creates Epics, User Stories, and Tasks following Spellwright templates. Use when refining backlog items, creating project artifacts, breaking down stories into tasks, or updating existing Notion pages with SPWE/SPWS/SPWT IDs.
---

# Refinement Skill

Zero-input automation for creating and refining agile artifacts (Epics, Stories, Tasks) directly in Notion. The agent gathers context from user input, existing Notion pages, and codebase analysis.

## Prerequisites

- Notion MCP enabled (`project-0-spellwright-Notion`)
- Access to Spellwright Notion workspace

## Workflows

| Task               | Workflow                                                     |
| ------------------ | ------------------------------------------------------------ |
| Create Epic        | [workflows/create-epic.md](workflows/create-epic.md)         |
| Create Story       | [workflows/create-story.md](workflows/create-story.md)       |
| Create Task        | [workflows/create-task.md](workflows/create-task.md)         |
| Refine Existing    | [workflows/refine-existing.md](workflows/refine-existing.md) |
| Bulk Task Creation | [workflows/bulk-tasks.md](workflows/bulk-tasks.md)           |

## Quick Commands

```bash
# Create Epic (helper script)
npx ts-node .cursor/skills/refinement/scripts/create-epic.ts \
  --name "Epic Name" --priority High

# Create Story (helper script)
npx ts-node .cursor/skills/refinement/scripts/create-story.ts \
  --name "Story Name" --priority High --epic SPWE-1

# Create Task (helper script)
npx ts-node .cursor/skills/refinement/scripts/create-task.ts \
  --name "Task Name" --type feature --priority High --story SPWS-1
```

## Notion MCP Operations

### Search for Artifact

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-search
arguments: { "query": "SPWT-41", "query_type": "internal" }
```

### Fetch Page Content

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-fetch
arguments: { "id": "[page-id-or-url]" }
```

### Create Artifact

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-create-pages
arguments: {
  "parent": { "data_source_id": "[collection-id]" },
  "pages": [{
    "properties": { "Name": "...", "Priority": "...", ... },
    "content": "### 📝 Description\n..."
  }]
}
```

### Update Artifact

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-update-page
arguments: {
  "data": {
    "page_id": "[page-id]",
    "command": "update_properties",
    "properties": { "Status": "In progress" }
  }
}
```

## Data Source IDs

| Type    | Collection ID                          |
| ------- | -------------------------------------- |
| Epics   | `28e56d55-129b-8189-827c-000b7bda32ac` |
| Stories | `28e56d55-129b-8156-85c3-000bea866839` |
| Tasks   | `28e56d55-129b-8181-b82b-000b7a52fcfb` |

## Artifact ID Patterns

| Type  | Pattern | Example |
| ----- | ------- | ------- |
| Epic  | SPWE-X  | SPWE-1  |
| Story | SPWS-X  | SPWS-15 |
| Task  | SPWT-X  | SPWT-41 |

## References

| Reference                                       | Content                                 |
| ----------------------------------------------- | --------------------------------------- |
| [notion-schema.md](references/notion-schema.md) | Notion database schema and MCP examples |
| [priorities.md](references/priorities.md)       | MoSCoW and Bug Severity frameworks      |
| [templates.md](references/templates.md)         | Quick reference for template structures |

## Templates

| Template                                     | Use Case                                  |
| -------------------------------------------- | ----------------------------------------- |
| [epic.md](templates/epic.md)                 | Business-level project artifacts          |
| [story.md](templates/story.md)               | Functional requirements with architecture |
| [feature-task.md](templates/feature-task.md) | New gameplay/user-facing functionality    |
| [tech-task.md](templates/tech-task.md)       | Refactoring and optimizations             |
| [bug-task.md](templates/bug-task.md)         | Defect fixes                              |

## Scripts

| Script                    | Purpose                                              |
| ------------------------- | ---------------------------------------------------- |
| `scripts/create-epic.ts`  | Generate Epic with MCP parameters                    |
| `scripts/create-story.ts` | Generate Story with MCP parameters                   |
| `scripts/create-task.ts`  | Generate Task (Feature/Tech/Bug) with MCP parameters |

## Library Modules

| Module                      | Purpose                           |
| --------------------------- | --------------------------------- |
| `scripts/lib/types.ts`      | Type definitions and constants    |
| `scripts/lib/validation.ts` | ID and data validation            |
| `scripts/lib/notion.ts`     | Notion MCP helpers                |
| `scripts/lib/templates.ts`  | Content generation                |
| `scripts/lib/codebase.ts`   | Codebase analysis for suggestions |
| `scripts/lib/cli.ts`        | CLI argument parsing and output   |

## Hierarchy

```
Epic (Business Vision)
  ↓
Story (Functional Requirements + Conceptual Architecture)
  ↓
Task (Specific Implementation + Code)
```

## Priority Framework

### MoSCoW (for Epic/Story/Feature/Tech Tasks)

| Priority | Definition                                  |
| -------- | ------------------------------------------- |
| Critical | Absolutely essential, blocks all other work |
| High     | Essential for release, cannot ship without  |
| Medium   | Important but has workaround                |
| Low      | Desirable, can be deferred                  |

### Bug Severity (for Bug Tasks)

| Severity | Definition                               |
| -------- | ---------------------------------------- |
| Critical | Showstopper, data loss, security issue   |
| High     | Blocks core functionality, no workaround |
| Medium   | Degrades experience, has workaround      |
| Low      | Cosmetic, rare edge case                 |

## Risk Categories

| Emoji | Category             | Use                                     |
| ----- | -------------------- | --------------------------------------- |
| 🔴    | Technical            | Architecture, performance challenges    |
| 🟡    | Dependency           | Blocked by other work, external factors |
| 🟠    | Knowledge/Scope      | Expertise gaps, unclear boundaries      |
| 🔵    | Timeline/Integration | Schedule, coordination challenges       |
