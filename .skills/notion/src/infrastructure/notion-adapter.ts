import { Client } from "@notionhq/client";
import * as dotenv from "dotenv";
import { IBlock, IComment, IDatabase, IPage, ISearchResult, ITeamspace, IUser } from "../domain/entities";
import { INotionRepository } from "../domain/interfaces";
import { createParagraphBlock, createTitleProperty, extractTitle, mapBlock, mapComment, mapDatabase, mapPage, mapSearchResult, mapUser, paginate, parseTaskId } from "./notion-utils";

dotenv.config({ path: "./.env" });

/**
 * Implementation of the Notion Repository using the official Notion SDK.
 * @class NotionAdapter
 * @implements {INotionRepository}
 */
export class NotionAdapter implements INotionRepository {
    private client: Client;

    /**
     * Initializes the Notion client.
     * @constructor
     * @throws {Error} If NOTION_API_KEY environment variable is missing.
     */
    constructor() {
        const token = process.env.NOTION_API_KEY;
        if (!token) {
            throw new Error("NOTION_API_KEY environment variable is not set.");
        }
        this.client = new Client({ auth: token });
    }

    /**
     * Searches for pages or databases in Notion.
     * @param {string} [query] - The text to search for.
     * @param {"last_edited_time" | "relevance"} [sort] - Sort criteria.
     * @returns {Promise<ISearchResult>} The search results.
     */
    async search(query?: string, sort?: "last_edited_time" | "relevance"): Promise<ISearchResult> {
        const args: any = { query, page_size: 100 };
        if (sort && sort !== "relevance") {
            args.sort = { direction: "descending", timestamp: sort };
        }

        const response = await this.client.search(args);
        return {
            results: response.results.map(mapSearchResult),
            next_cursor: response.next_cursor,
            has_more: response.has_more,
        };
    }

    /**
     * Retrieves a page by its ID.
     * @param {string} pageId - The UUID of the page.
     * @returns {Promise<IPage>} The page details.
     */
    async getPage(pageId: string): Promise<IPage> {
        const response = await this.client.pages.retrieve({ page_id: pageId });
        return mapPage(response);
    }

    /**
     * Gets a page by its task ID property across all data sources in the workspace.
     * @param {string} taskId - The task ID (e.g., "SPWT-1").
     * @returns {Promise<IPage | null>} The page if found, null otherwise.
     */
    async getPageByTaskId(taskId: string): Promise<IPage | null> {
        const parsed = parseTaskId(taskId);
        if (!parsed) return null;

        const dataSources = await this.getAllDataSources();
        const results = await Promise.all(dataSources.map((id) => this.queryDataSourceForTask(id, parsed)));

        return results.find((r) => r !== null) ?? null;
    }

    /**
     * Retrieves all blocks (content) of a page or block.
     * @param {string} blockId - The UUID of the parent block/page.
     * @returns {Promise<IBlock[]>} A list of child blocks.
     */
    async getPageContent(blockId: string): Promise<IBlock[]> {
        return paginate((cursor) => this.client.blocks.children.list({ block_id: blockId, start_cursor: cursor, page_size: 100 }), mapBlock);
    }

    /**
     * Lists all users in the workspace.
     * @returns {Promise<IUser[]>} A list of users.
     */
    async listUsers(): Promise<IUser[]> {
        return paginate((cursor) => this.client.users.list({ start_cursor: cursor, page_size: 100 }), mapUser);
    }

    /**
     * Retrieves a specific user by ID.
     * @param {string} userId - The UUID of the user.
     * @returns {Promise<IUser>} The user details.
     */
    async getUser(userId: string): Promise<IUser> {
        const response = await this.client.users.retrieve({ user_id: userId });
        return mapUser(response);
    }

    /**
     * Retrieves information about the bot user.
     * @returns {Promise<{ bot: IUser; workspace_name?: string }>} Bot info.
     */
    async getBotInfo(): Promise<{ bot: IUser; workspace_name?: string }> {
        const response = await this.client.users.me({});
        return {
            bot: mapUser(response),
            workspace_name: (response as any).bot?.workspace_name,
        };
    }

    /**
     * Creates a comment on a page.
     * @param {string} pageId - The page ID.
     * @param {string} content - The comment text.
     * @returns {Promise<IComment>} The created comment.
     */
    async createComment(pageId: string, content: string): Promise<IComment> {
        const response = await this.client.comments.create({
            parent: { page_id: pageId },
            rich_text: [{ text: { content } }],
        });
        return mapComment(response);
    }

    /**
     * Lists all comments on a block/page.
     * @param {string} blockId - The UUID of the block/page.
     * @returns {Promise<IComment[]>} A list of comments.
     */
    async listComments(blockId: string): Promise<IComment[]> {
        return paginate((cursor) => this.client.comments.list({ block_id: blockId, start_cursor: cursor, page_size: 100 }), mapComment);
    }

    /**
     * Creates a new page in a parent page or database.
     * @param {Object} parent - Parent location.
     * @param {string} [parent.page_id] - Parent page UUID.
     * @param {string} [parent.database_id] - Parent database UUID.
     * @param {string} title - Page title.
     * @param {string} [content] - Initial content.
     * @returns {Promise<IPage>} The created page.
     */
    async createPage(parent: { page_id?: string; database_id?: string }, title: string, content?: string): Promise<IPage> {
        const parentArg: any = parent.database_id ? { database_id: parent.database_id } : { page_id: parent.page_id };
        const children = content ? [createParagraphBlock(content)] : [];
        const properties = createTitleProperty(title, !!parent.database_id);

        const response = await this.client.pages.create({ parent: parentArg, properties, children });
        return mapPage(response);
    }

    /**
     * Updates a page's properties or archived status.
     * @param {string} pageId - The page UUID.
     * @param {Record<string, any>} [properties] - Properties to update.
     * @param {boolean} [archived] - Archive status.
     * @returns {Promise<IPage>} The updated page.
     */
    async updatePage(pageId: string, properties?: Record<string, any>, archived?: boolean): Promise<IPage> {
        const args: any = { page_id: pageId };
        if (properties) args.properties = properties;
        if (archived !== undefined) args.archived = archived;

        const response = await this.client.pages.update(args);
        return mapPage(response);
    }

    /**
     * Moves a page to a new parent.
     * @param {string} pageId - The ID of the page to move.
     * @param {string} newParentId - The ID of the new parent.
     * @returns {Promise<IPage>} The new page instance.
     */
    async movePage(pageId: string, newParentId: string): Promise<IPage> {
        const { title, children } = await this.getPageCopyData(pageId);
        const parentArg = await this.resolveParent(newParentId);

        const newPage = await this.client.pages.create({
            parent: parentArg,
            properties: { title: [{ type: "text", text: { content: title } }] } as any,
            children,
        });

        await this.client.pages.update({ page_id: pageId, archived: true });
        return mapPage(newPage);
    }

    /**
     * Duplicates a page.
     * @param {string} pageId - The page to duplicate.
     * @returns {Promise<IPage>} The duplicated page.
     */
    async duplicatePage(pageId: string): Promise<IPage> {
        const original = await this.client.pages.retrieve({ page_id: pageId });
        const { title, children } = await this.getPageCopyData(pageId);

        const originalObj = original as any;
        const titleKey = this.getTitlePropertyKey(originalObj);

        const newPage = await this.client.pages.create({
            parent: originalObj.parent as any,
            properties: { [titleKey]: { title: [{ type: "text", text: { content: `${title} (Copy)` } }] } } as any,
            children,
        });

        return mapPage(newPage);
    }

    /**
     * Creates a new database.
     * @param {string} parentId - Parent page ID.
     * @param {string} title - Database title.
     * @param {Record<string, any>} properties - Database schema.
     * @returns {Promise<IDatabase>} The created database.
     */
    async createDatabase(parentId: string, title: string, properties: Record<string, any>): Promise<IDatabase> {
        const response = await this.client.databases.create({
            parent: { type: "page_id", page_id: parentId },
            title: [{ type: "text", text: { content: title } }],
            properties: properties as any,
        } as any);
        return mapDatabase(response);
    }

    /**
     * Updates a database.
     * @param {string} databaseId - Database UUID.
     * @param {Object} updates - Updates to apply.
     * @param {string} [updates.title] - New title.
     * @param {Record<string, any>} [updates.properties] - Property updates.
     * @returns {Promise<IDatabase>} The updated database.
     */
    async updateDatabase(databaseId: string, updates: { title?: string; properties?: Record<string, any> }): Promise<IDatabase> {
        const args: any = { database_id: databaseId };
        if (updates.title) args.title = [{ type: "text", text: { content: updates.title } }];
        if (updates.properties) args.properties = updates.properties;

        const response = await this.client.databases.update(args);
        return mapDatabase(response);
    }

    /**
     * Lists teamspaces (not fully supported).
     * @returns {Promise<ITeamspace[]>} Empty list.
     */
    async listTeamspaces(): Promise<ITeamspace[]> {
        return [];
    }

    /**
     * Retrieves a database by its ID.
     * @param {string} databaseId - The UUID of the database.
     * @returns {Promise<IDatabase>} The database details.
     */
    async getDatabase(databaseId: string): Promise<IDatabase> {
        const response = await this.client.databases.retrieve({ database_id: databaseId });
        return mapDatabase(response);
    }

    /**
     * Retrieves a block by its ID.
     * @param {string} blockId - The UUID of the block.
     * @returns {Promise<IBlock>} The block details.
     */
    async getBlock(blockId: string): Promise<IBlock> {
        const response = await this.client.blocks.retrieve({ block_id: blockId });
        return mapBlock(response);
    }

    /**
     * Updates a block's content.
     * @param {string} blockId - The UUID of the block.
     * @param {string} content - The new content.
     * @returns {Promise<IBlock>} The updated block.
     */
    async updateBlock(blockId: string, content: string): Promise<IBlock> {
        const block = await this.client.blocks.retrieve({ block_id: blockId });
        const type = (block as any).type;

        const response = await this.client.blocks.update({
            block_id: blockId,
            [type]: { rich_text: [{ text: { content } }] },
        });
        return mapBlock(response);
    }

    /**
     * Deletes a block.
     * @param {string} blockId - The UUID of the block.
     * @returns {Promise<void>}
     */
    async deleteBlock(blockId: string): Promise<void> {
        await this.client.blocks.delete({ block_id: blockId });
    }

    /**
     * Appends children blocks to a parent block.
     * @param {string} blockId - The UUID of the parent block.
     * @param {Array<{ type: string; content: string }>} children - The children to append.
     * @returns {Promise<IBlock[]>} The appended blocks.
     */
    async appendBlockChildren(blockId: string, children: { type: string; content: string }[]): Promise<IBlock[]> {
        const blocks = children.map((child) => ({
            object: "block" as const,
            type: child.type as any,
            [child.type]: { rich_text: [{ text: { content: child.content } }] },
        }));

        const response = await this.client.blocks.children.append({ block_id: blockId, children: blocks as any });
        return response.results.map(mapBlock);
    }

    /**
     * Retrieves all data source IDs from the workspace.
     * @returns {Promise<string[]>} List of data source IDs.
     */
    private async getAllDataSources(): Promise<string[]> {
        const result = await this.client.search({ filter: { property: "object", value: "data_source" } });
        return result.results.map((item: any) => item.id).filter(Boolean);
    }

    /**
     * Queries a data source for a task by its ID property.
     * @param {string} dataSourceId - The data source UUID.
     * @param {{ prefix: string; number: number }} task - The parsed task ID.
     * @returns {Promise<IPage | null>} The page if found, null otherwise.
     */
    private async queryDataSourceForTask(dataSourceId: string, task: { prefix: string; number: number }): Promise<IPage | null> {
        try {
            const response = await this.client.dataSources.query({
                data_source_id: dataSourceId,
                filter: { property: "ID", unique_id: { equals: task.number } } as any,
                page_size: 1,
            });

            const page = response.results[0] as any;
            const idProp = page?.properties?.ID?.unique_id;

            if (idProp?.prefix === task.prefix && idProp?.number === task.number) {
                return mapPage(page);
            }
        } catch {
            // Data source doesn't have ID property
        }
        return null;
    }

    /**
     * Retrieves page data needed for copying (title and content blocks).
     * @param {string} pageId - The page UUID.
     * @returns {Promise<{ title: string; children: any[] }>} The page copy data.
     */
    private async getPageCopyData(pageId: string): Promise<{ title: string; children: any[] }> {
        const original = await this.client.pages.retrieve({ page_id: pageId });
        const content = await this.getPageContent(pageId);

        const title = extractTitle((original as any).properties);
        const children = content.filter((b) => b.content).map((b) => createParagraphBlock(b.content!));

        return { title, children };
    }

    /**
     * Resolves whether a parent ID is a page or database.
     * @param {string} parentId - The parent UUID.
     * @returns {Promise<{ page_id: string } | { database_id: string }>} The parent reference.
     */
    private async resolveParent(parentId: string): Promise<{ page_id: string } | { database_id: string }> {
        try {
            await this.client.pages.retrieve({ page_id: parentId });
            return { page_id: parentId };
        } catch {
            return { database_id: parentId };
        }
    }

    /**
     * Gets the title property key from a page object.
     * @param {any} pageObj - The raw page object.
     * @returns {string} The title property key.
     */
    private getTitlePropertyKey(pageObj: any): string {
        if (pageObj.parent?.type !== "database_id") return "title";

        const props = pageObj.properties;
        for (const key in props) {
            if (props[key].type === "title") return key;
        }
        return "Name";
    }
}
