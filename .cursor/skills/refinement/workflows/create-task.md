# Workflow: Create Task

## Trigger

User requests:

- "Create a task for X"
- "Create a feature task for [feature]"
- "Create a tech task for [refactoring]"
- "Create a bug task for [bug]"
- "Add task to SPWS-X"

## Step 1: Determine Task Type

| Type        | Use Case                                         |
| ----------- | ------------------------------------------------ |
| **Feature** | New gameplay/user-facing functionality           |
| **Tech**    | Refactoring, optimization, internal improvements |
| **Bug**     | Fix identified defects                           |

## Step 2: Gather Information

### For All Types

1. **Task Name** - Specific, implementation-focused
2. **Parent Story** - SPWS-X ID
3. **Priority** - Critical/High/Medium/Low (or Bug Severity)

### For Feature Tasks

- Description of user-facing behavior
- Expected visible outcomes

### For Tech Tasks

- Technical Objective: What + Why
- Performance improvement target (before/after)

### For Bug Tasks

- Steps to reproduce
- Expected vs Actual behavior
- Environment details

## Step 3: Fetch Parent Story

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-search
arguments: { "query": "SPWS-15", "query_type": "internal" }
```

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-fetch
arguments: { "id": "[Story Page ID]" }
```

Extract from Story:

- Acceptance criteria context
- Components/Systems involved
- Architectural decisions

## Step 4: Analyze Codebase

```bash
# Find existing file structure
ls -la Assets/Scripts/Components/
ls -la Assets/Scripts/Systems/

# Find similar implementations
rg "[SimilarPattern]" Assets/Scripts/ --type cs

# Find test patterns
ls -la Assets/Tests/
```

Suggest:

- File paths following existing structure
- Similar components/systems to reference
- Test file locations

## Step 5: Load Template

Select template based on type:

```
Read: .cursor/skills/refinement/templates/feature-task.md
Read: .cursor/skills/refinement/templates/tech-task.md
Read: .cursor/skills/refinement/templates/bug-task.md
```

## Step 6: Generate Content

### Feature Task

- **Description** - User-facing behavior
- **Definition of Done** - 10+ items with exact file paths
- **Technical Refinement** - Core Scripts, Assets, Inspector Values, Implementation Notes
- **Risks** - With mitigations

### Tech Task

- **Technical Objective** - What + Why with metrics
- **Definition of Done** - With performance targets
- **Technical Refinement** - Step-by-step plan
- **Performance Impact** - Before/After comparison

### Bug Task

- **Bug Report** - Reproduction steps, Expected/Actual, Environment
- **Root Cause Analysis** - Why it happens
- **Definition of Done** - With regression test
- **Implementation Plan** - Fix steps

## Step 7: Create in Notion

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-create-pages
arguments: {
  "parent": { "data_source_id": "28e56d55-129b-8181-b82b-000b7a52fcfb" },
  "pages": [{
    "properties": {
      "Name": "[Task Title]",
      "Priority": "[Priority]",
      "Status": "Not started",
      "Type": "[Feature/Tech/Bug]",
      "Story": "[Story Page URL]"
    },
    "content": "[Generated Markdown Content]"
  }]
}
```

## Step 8: Confirm

Report to user:

> [Type] Task "[Name]" created successfully.
> URL: [Notion URL]
> Parent Story: SPWS-X
> Priority: [Priority]
>
> Files to create:
>
> - Assets/Scripts/[path]
> - Assets/Tests/[path]
>
> Ready to start implementation!

## Errors

| Error              | Resolution                       |
| ------------------ | -------------------------------- |
| Story not found    | Ask for correct Story ID         |
| Invalid task type  | Clarify Feature/Tech/Bug         |
| Missing file paths | Analyze codebase for suggestions |

## Quality Checklist

### Feature Task

- [ ] Description is user-facing
- [ ] DoD has 10+ items with exact paths
- [ ] Implementation notes include algorithms
- [ ] Performance targets specified

### Tech Task

- [ ] Technical Objective has WHAT + WHY
- [ ] Performance metrics before/after
- [ ] Step-by-step implementation plan
- [ ] Rollback strategy if risky

### Bug Task

- [ ] Reproduction steps are exact
- [ ] Environment info complete
- [ ] Root Cause explains WHY
- [ ] Regression test required
