# Notion Integration

MCP integration for task management automation.

## Available Tools

| Tool                 | Use Case                         |
| -------------------- | -------------------------------- |
| `notion-search`      | Find tasks by ID (SPWT-X)        |
| `notion-fetch`       | Get task details                 |
| `notion-update-page` | Update task with branch/PR links |

## Search for Task

```
Tool: CallMcpTool
Server: user-Notion
Tool Name: notion-search
Arguments: { "query": "SPWT-13", "query_type": "internal" }
```

## Fetch Task Details

```
Tool: CallMcpTool
Server: user-Notion
Tool Name: notion-fetch
Arguments: { "id": "<page_url>" }
```

## Update Task with Branch

```
Tool: CallMcpTool
Server: user-Notion
Tool Name: notion-update-page
Arguments: {
  "data": {
    "page_id": "<task_id>",
    "command": "update_properties",
    "properties": {
      "Branch": "https://github.com/owner/repo/tree/branch-name"
    }
  }
}
```

## Update Task with PR

```
Tool: CallMcpTool
Server: user-Notion
Tool Name: notion-update-page
Arguments: {
  "data": {
    "page_id": "<task_id>",
    "command": "update_properties",
    "properties": {
      "Pull Request": "https://github.com/owner/repo/pull/42"
    }
  }
}
```

## Property Names

| Property         | Type   | Use                |
| ---------------- | ------ | ------------------ |
| `Branch`         | URL    | GitHub branch link |
| `Pull Request`   | URL    | GitHub PR link     |
| `Priority`       | Select | PR labels          |
| `userDefined:ID` | Text   | Task ID (SPWT-X)   |
| `Name`           | Title  | Task title         |

## Extractable Information

| Property | Use For                 |
| -------- | ----------------------- |
| Name     | Commit subject, PR body |
| Priority | PR labels               |
| Content  | Definition of Done      |
| URL      | PR references           |
