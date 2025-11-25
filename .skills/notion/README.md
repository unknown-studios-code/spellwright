# Notion Skills

TypeScript-based Notion integration skills implementing Clean Architecture. These skills can be executed via CLI or imported as a library for agent composition.

## Setup

1. **Install Dependencies**:

    ```bash
    cd .agent/skills
    npm install
    ```

2. **Configure Environment**:
   Create/Update `.env` with your Notion Integration Token:

    ```properties
    NOTION_API_KEY=secret_your_notion_integration_token
    ```

3. **Share Pages**:
   You must invite your integration (bot) to the specific pages/databases you want it to access in Notion.

## Usage

### Programmatic API (Recommended for Agents)

Agents should import functions from `notion/index.ts` to chain operations or process data programmatically.

```typescript
import { createPage, updateDatabase } from "./.agent/skills/notion/index";

async function task() {
    const page = await createPage("parentId", "page", "New Page");
    // Use data directly
    console.log(page.data.id);
}
```

### CLI (Manual Testing)

**Search & Read**

- **Search**: `npx ts-node notion/cli/search.ts "query" [sort]`
- **Get Page**: `npx ts-node notion/cli/get_page.ts <pageId>`
- **Get Database**: `npx ts-node notion/cli/get_database.ts <databaseId>`
- **Get Block**: `npx ts-node notion/cli/get_block.ts <blockId>`
- **Get Page Content**: `npx ts-node notion/cli/get_page_content.ts <blockId>`
- **Get Bot Info**: `npx ts-node notion/cli/get_bot_info.ts`
- **List Users**: `npx ts-node notion/cli/list_users.ts`
- **Get User**: `npx ts-node notion/cli/get_user.ts <userId>`
- **List Teamspaces**: `npx ts-node notion/cli/list_teams.ts`

**Write Operations**

- **Create Page**: `npx ts-node notion/cli/create_page.ts <parentId> <page|database> <title> [content]`
- **Update Page**: `npx ts-node notion/cli/update_page.ts <pageId> [key=value] [archived=true]`
- **Move Page**: `npx ts-node notion/cli/move_page.ts <pageId> <newParentId>`
- **Duplicate Page**: `npx ts-node notion/cli/duplicate_page.ts <pageId>`
- **Create Database**: `npx ts-node notion/cli/create_database.ts <parentId> <title> <propertiesJSON>`
- **Update Database**: `npx ts-node notion/cli/update_database.ts <databaseId> [title=NewTitle] [properties=JSON]`
- **Update Block**: `npx ts-node notion/cli/update_block.ts <blockId> <content>`
- **Delete Block**: `npx ts-node notion/cli/delete_block.ts <blockId>`
- **Append Children**: `npx ts-node notion/cli/append_block_children.ts <blockId> <childrenJSON>`

**Comments**

- **Create Comment**: `npx ts-node notion/cli/create_comment.ts <pageId> "comment text"`
- **List Comments**: `npx ts-node notion/cli/list_comments.ts <blockId>`

## Architecture

- **Domain**: `src/domain` (Entities, Interfaces)
- **Infrastructure**: `src/infrastructure` (NotionClient Adapter)
- **Application**: `src/application` (Use Cases)
- **Presentation**:
    - **CLI**: `cli/*.ts` (Command Line Interface)
    - **Library**: `index.ts` (Programmatic API)
