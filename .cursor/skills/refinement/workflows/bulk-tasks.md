# Workflow: Bulk Task Creation

## Trigger

User requests:

- "Create tasks for SPWS-X"
- "Break down story SPWS-X into tasks"
- "Generate all tasks for this story"
- "Create multiple tasks for [story]"

## Step 1: Fetch Parent Story

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-search
arguments: { "query": "SPWS-X", "query_type": "internal" }
```

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-fetch
arguments: { "id": "[Story Page ID]" }
```

Extract from Story:

- Acceptance criteria (each may become a task)
- Components needed (each may need implementation)
- Systems needed (each may need implementation)
- Integration points (may need tasks)

## Step 2: Analyze Story Breakdown

### Identify Potential Tasks

From Acceptance Criteria:

1. Each complex criterion → Feature Task
2. Each performance requirement → Tech Task (with metrics)

From Technical Refinement:

1. Each new Component → Feature Task
2. Each new System → Feature Task
3. Each Architectural Decision → Tech Task (if requires refactoring)

### Group by Dependencies

```
Critical Tasks (blockers)
  ↓
High Priority Tasks (core features)
  ↓
Medium Priority Tasks (enhancements)
  ↓
Low Priority Tasks (polish)
```

## Step 3: Analyze Codebase

```bash
# Check existing structure
ls -la Assets/Scripts/Components/
ls -la Assets/Scripts/Systems/

# Find patterns to follow
rg "public struct.*IComponentData" Assets/Scripts/ -A 5
rg "public partial struct.*ISystem" Assets/Scripts/ -A 5
```

## Step 4: Generate Task List

Present to user for approval:

> Based on Story SPWS-X, I suggest creating the following tasks:
>
> **Critical:**
>
> 1. SPWT-XX: [Task Name] - [Brief description]
>
> **High:** 2. SPWT-XX: [Task Name] - [Brief description] 3. SPWT-XX: [Task Name] - [Brief description]
>
> **Medium:** 4. SPWT-XX: [Task Name] - [Brief description]
>
> Would you like me to create all of these, or modify the list?

## Step 5: Create Tasks

For each approved task, use the Task Creation workflow:

```
CallMcpTool
server: project-0-spellwright-Notion
toolName: notion-create-pages
arguments: {
  "parent": { "data_source_id": "28e56d55-129b-8181-b82b-000b7a52fcfb" },
  "pages": [
    {
      "properties": {
        "Name": "[Task 1 Title]",
        "Priority": "Critical",
        "Status": "Not started",
        "Type": "Feature",
        "Story": "[Story URL]"
      },
      "content": "[Task 1 Content]"
    },
    {
      "properties": {
        "Name": "[Task 2 Title]",
        "Priority": "High",
        "Status": "Not started",
        "Type": "Feature",
        "Story": "[Story URL]"
      },
      "content": "[Task 2 Content]"
    }
  ]
}
```

**Note:** notion-create-pages supports creating multiple pages at once (up to 100).

## Step 6: Confirm Results

Report to user:

> Created X tasks for Story SPWS-X:
>
> | ID      | Name   | Type    | Priority |
> | ------- | ------ | ------- | -------- |
> | SPWT-XX | [Name] | Feature | Critical |
> | SPWT-XX | [Name] | Feature | High     |
> | SPWT-XX | [Name] | Tech    | Medium   |
>
> Recommended order of implementation:
>
> 1. SPWT-XX (Critical - blocks others)
> 2. SPWT-XX (High - core feature)
> 3. SPWT-XX (High - depends on #2)
> 4. SPWT-XX (Medium - enhancement)
>
> All tasks linked to parent Story.

## Task Breakdown Patterns

### Component Implementation

```
Task: Implement [Component]Data Component

Definition of Done:
1. Create [Component]Data.cs in Components/[Category]/
2. Define fields: [field1], [field2]
3. Implement IComponentData interface
4. Add Baker if needed
5. Create unit tests
```

### System Implementation

```
Task: Implement [System]System

Definition of Done:
1. Create [System]System.cs in Systems/[Category]/
2. Implement ISystem interface
3. Define query with required components
4. Implement OnUpdate logic
5. Apply [BurstCompile] attribute
6. Create unit tests
7. Profile performance
```

### Integration Task

```
Task: Integrate [Feature] with [OtherSystem]

Definition of Done:
1. Add integration point in [File]
2. Handle [specific scenarios]
3. Test with [integration scenarios]
4. Document interaction
```

## Errors

| Error                             | Resolution                             |
| --------------------------------- | -------------------------------------- |
| Story has no technical refinement | Ask user for breakdown guidance        |
| Too many tasks generated          | Group related work, ask for priorities |
| Duplicate tasks                   | Check existing tasks before creating   |

## Tips

- Start with Critical tasks that unblock others
- Group related components/systems together
- Include a "Documentation" task if extensive docs needed
- Consider adding a "Integration Testing" task for complex stories
