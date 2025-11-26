# Notion Database Schema

Reference for Spellwright Notion workspace databases.

## MCP Server

**Server Identifier:** `project-0-spellwright-Notion`

## Data Sources (Collections)

| Type    | Collection ID                          | Database Name |
| ------- | -------------------------------------- | ------------- |
| Epics   | `28e56d55-129b-8189-827c-000b7bda32ac` | Epics         |
| Stories | `28e56d55-129b-8156-85c3-000bea866839` | Stories       |
| Tasks   | `28e56d55-129b-8181-b82b-000b7a52fcfb` | Tasks         |

## Epic Properties

| Property         | Type     | Description          | Values                         |
| ---------------- | -------- | -------------------- | ------------------------------ |
| `Name`           | Title    | Epic title           | String                         |
| `userDefined:ID` | Text     | Auto-increment ID    | SPWE-X                         |
| `Priority`       | Select   | MoSCoW priority      | Critical, High, Medium, Low    |
| `Status`         | Status   | Current status       | Not started, In progress, Done |
| `Assignee`       | Person   | Assigned team member | User reference                 |
| `Stories`        | Relation | Child stories        | Links to Stories database      |
| `Documentation`  | Relation | Related docs         | Links to Documentation         |

## Story Properties

| Property         | Type     | Description          | Values                         |
| ---------------- | -------- | -------------------- | ------------------------------ |
| `Name`           | Title    | Story title          | String                         |
| `userDefined:ID` | Text     | Auto-increment ID    | SPWS-X                         |
| `Priority`       | Select   | MoSCoW priority      | Critical, High, Medium, Low    |
| `Status`         | Status   | Current status       | Not started, In progress, Done |
| `Assignee`       | Person   | Assigned team member | User reference                 |
| `Epic`           | Relation | Parent epic          | Link to Epics database         |
| `Tasks`          | Relation | Child tasks          | Links to Tasks database        |
| `Branch`         | URL      | GitHub branch URL    | URL string                     |
| `Pull Request`   | URL      | GitHub PR URL        | URL string                     |

## Task Properties

| Property         | Type     | Description        | Values                         |
| ---------------- | -------- | ------------------ | ------------------------------ |
| `Name`           | Title    | Task title         | String                         |
| `userDefined:ID` | Text     | Auto-increment ID  | SPWT-X                         |
| `Priority`       | Select   | Priority/Severity  | Critical, High, Medium, Low    |
| `Status`         | Status   | Current status     | Not started, In progress, Done |
| `Type`           | Select   | Task type          | Feature, Tech, Bug             |
| `Assignee`       | Person   | Assigned developer | User reference                 |
| `Story`          | Relation | Parent story       | Link to Stories database       |
| `Branch`         | URL      | GitHub branch URL  | URL string                     |
| `Pull Request`   | URL      | GitHub PR URL      | URL string                     |

## MCP Tool Reference

### Search

```json
{
    "query": "SPWT-41",
    "query_type": "internal"
}
```

### Fetch

```json
{
    "id": "28f56d55-129b-8081-83f7-c09246a7e8ce"
}
```

### Create Page

```json
{
    "parent": { "data_source_id": "28e56d55-129b-8181-b82b-000b7a52fcfb" },
    "pages": [
        {
            "properties": {
                "Name": "Task Title",
                "Priority": "High",
                "Type": "Feature",
                "Status": "Not started",
                "Story": "{{https://www.notion.so/story-url}}"
            },
            "content": "### 📝 Description\n..."
        }
    ]
}
```

### Update Properties

```json
{
    "data": {
        "page_id": "28f56d55-129b-8081-83f7-c09246a7e8ce",
        "command": "update_properties",
        "properties": {
            "Status": "In progress",
            "Branch": "https://github.com/owner/repo/tree/branch"
        }
    }
}
```

### Replace Content

```json
{
    "data": {
        "page_id": "28f56d55-129b-8081-83f7-c09246a7e8ce",
        "command": "replace_content",
        "new_str": "# New Content\n..."
    }
}
```

### Replace Content Range

```json
{
    "data": {
        "page_id": "28f56d55-129b-8081-83f7-c09246a7e8ce",
        "command": "replace_content_range",
        "selection_with_ellipsis": "### Old Section...end of section",
        "new_str": "### New Section\nUpdated content"
    }
}
```

## Property Naming Rules

- Properties named "id" or "url" must be prefixed with `userDefined:`
- Example: `userDefined:ID` for the artifact ID
- Relation properties use Notion page URLs wrapped in `{{...}}`
- Checkbox properties use `__YES__` or `__NO__`

## URL Formats

- Page URL: `https://www.notion.so/[page-id]`
- Relation value: `{{https://www.notion.so/[page-id]}}`
- Collection URL: `collection://[collection-id]`
