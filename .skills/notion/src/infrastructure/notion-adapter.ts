import { Client } from "@notionhq/client";
import { BlockObjectRequest } from "@notionhq/client/build/src/api-endpoints";
import * as dotenv from "dotenv";
import { IBlock, IComment, IDatabase, IPage, ISearchResult, ITeamspace, IUser } from "../domain/entities";
import { INotionRepository } from "../domain/interfaces";

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
        const args: any = {
            query,
            page_size: 100,
        };

        if (sort && sort !== "relevance") {
            args.sort = {
                direction: "descending",
                timestamp: sort,
            };
        }

        const response = await this.client.search(args);

        return {
            results: response.results.map((item) => this.mapSearchResult(item)),
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
        return this.mapPage(response);
    }

    /**
     * Retrieves all blocks (content) of a page or block.
     * Handles pagination to ensure all children are retrieved.
     * @param {string} blockId - The UUID of the parent block/page.
     * @returns {Promise<IBlock[]>} A list of child blocks.
     */
    async getPageContent(blockId: string): Promise<IBlock[]> {
        const blocks: IBlock[] = [];
        let cursor: string | undefined = undefined;
        let hasMore = true;

        while (hasMore) {
            const response = await this.client.blocks.children.list({
                block_id: blockId,
                start_cursor: cursor,
                page_size: 100,
            });

            blocks.push(...response.results.map((b) => this.mapBlock(b)));
            cursor = response.next_cursor ?? undefined;
            hasMore = response.has_more;
        }

        return blocks;
    }

    /**
     * Lists all users in the workspace.
     * @returns {Promise<IUser[]>} A list of users.
     */
    async listUsers(): Promise<IUser[]> {
        const users: IUser[] = [];
        let cursor: string | undefined = undefined;
        let hasMore = true;

        while (hasMore) {
            const response = await this.client.users.list({
                start_cursor: cursor,
                page_size: 100,
            });

            users.push(...response.results.map((u) => this.mapUser(u)));
            cursor = response.next_cursor ?? undefined;
            hasMore = response.has_more;
        }

        return users;
    }

    /**
     * Retrieves a specific user by ID.
     * @param {string} userId - The UUID of the user.
     * @returns {Promise<IUser>} The user details.
     */
    async getUser(userId: string): Promise<IUser> {
        const response = await this.client.users.retrieve({ user_id: userId });
        return this.mapUser(response);
    }

    /**
     * Retrieves information about the bot user.
     * @returns {Promise<{ bot: IUser; workspace_name?: string }>} Bot info.
     */
    async getBotInfo(): Promise<{ bot: IUser; workspace_name?: string }> {
        const response = await this.client.users.me({});
        return {
            bot: this.mapUser(response),
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
        return this.mapComment(response);
    }

    /**
     * Lists all comments on a block/page.
     * Handles pagination.
     * @param {string} blockId - The UUID of the block/page.
     * @returns {Promise<IComment[]>} A list of comments.
     */
    async listComments(blockId: string): Promise<IComment[]> {
        const comments: IComment[] = [];
        let cursor: string | undefined = undefined;
        let hasMore = true;

        while (hasMore) {
            const response = await this.client.comments.list({
                block_id: blockId,
                start_cursor: cursor,
                page_size: 100,
            });

            comments.push(...response.results.map((c) => this.mapComment(c)));
            cursor = response.next_cursor ?? undefined;
            hasMore = response.has_more;
        }
        return comments;
    }

    /**
     * Creates a new page in a parent page or database.
     * @param {Object} parent - Parent location.
     * @param {string} title - Page title.
     * @param {string} [content] - Initial content.
     * @returns {Promise<IPage>} The created page.
     */
    async createPage(parent: { page_id?: string; database_id?: string }, title: string, content?: string): Promise<IPage> {
        const parentArg: any = parent.database_id ? { database_id: parent.database_id } : { page_id: parent.page_id };

        const children: BlockObjectRequest[] = content
            ? [
                  {
                      object: "block",
                      type: "paragraph",
                      paragraph: {
                          rich_text: [{ text: { content } }],
                      },
                  },
              ]
            : [];

        const properties: any = {};

        if (parent.page_id) {
            properties.title = [
                {
                    type: "text",
                    text: { content: title },
                },
            ];
        } else {
            properties.Name = {
                title: [
                    {
                        type: "text",
                        text: { content: title },
                    },
                ],
            };
        }

        const response = await this.client.pages.create({
            parent: parentArg,
            properties,
            children,
        });
        return this.mapPage(response);
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
        return this.mapPage(response);
    }

    /**
     * Moves a page to a new parent.
     * @param {string} pageId - The ID of the page to move.
     * @param {string} newParentId - The ID of the new parent.
     * @returns {Promise<IPage>} The new page instance.
     */
    async movePage(pageId: string, newParentId: string): Promise<IPage> {
        const original = await this.client.pages.retrieve({ page_id: pageId });
        const content = await this.getPageContent(pageId);

        let parentArg: any = { page_id: newParentId };
        try {
            await this.client.pages.retrieve({ page_id: newParentId });
        } catch (e) {
            parentArg = { database_id: newParentId };
        }

        const originalObj = original as any;
        const props = originalObj.properties;
        let title = "Moved Page";

        for (const key in props) {
            if (props[key].type === "title") {
                title = props[key].title?.[0]?.plain_text || title;
                break;
            }
        }

        const children: BlockObjectRequest[] = content
            .map((b) => {
                if (!b.content) return null;
                return {
                    object: "block",
                    type: "paragraph",
                    paragraph: {
                        rich_text: [{ text: { content: b.content } }],
                    },
                } as BlockObjectRequest;
            })
            .filter((b): b is BlockObjectRequest => b !== null);

        const newPage = await this.client.pages.create({
            parent: parentArg,
            properties: {
                title: [{ type: "text", text: { content: title } }],
            } as any,
            children,
        });

        await this.client.pages.update({ page_id: pageId, archived: true });

        return this.mapPage(newPage);
    }

    /**
     * Duplicates a page.
     * @param {string} pageId - The page to duplicate.
     * @returns {Promise<IPage>} The duplicated page.
     */
    async duplicatePage(pageId: string): Promise<IPage> {
        const original = await this.client.pages.retrieve({ page_id: pageId });
        const content = await this.getPageContent(pageId);
        const originalObj = original as any;

        let title = "Untitled (Copy)";
        const props = originalObj.properties;
        for (const key in props) {
            if (props[key].type === "title") {
                const rawTitle = props[key].title?.[0]?.plain_text;
                if (rawTitle) title = `${rawTitle} (Copy)`;
                break;
            }
        }

        const parent = originalObj.parent;

        const titleKey = originalObj.parent.type === "database_id" ? Object.keys(props).find((k) => props[k].type === "title") || "Name" : "title";

        const children: BlockObjectRequest[] = content
            .map((b) => {
                if (!b.content) return null;
                return {
                    object: "block",
                    type: "paragraph",
                    paragraph: {
                        rich_text: [{ text: { content: b.content } }],
                    },
                } as BlockObjectRequest;
            })
            .filter((b): b is BlockObjectRequest => b !== null);

        const newPage = await this.client.pages.create({
            parent: parent as any,
            properties: {
                [titleKey]: {
                    title: [{ type: "text", text: { content: title } }],
                },
            } as any,
            children,
        });

        return this.mapPage(newPage);
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
        return this.mapDatabase(response);
    }

    /**
     * Updates a database.
     * @param {string} databaseId - Database UUID.
     * @param {Object} updates - Updates.
     * @returns {Promise<IDatabase>} Updated database.
     */
    async updateDatabase(databaseId: string, updates: { title?: string; properties?: Record<string, any> }): Promise<IDatabase> {
        const args: any = { database_id: databaseId };
        if (updates.title) {
            args.title = [{ type: "text", text: { content: updates.title } }];
        }
        if (updates.properties) {
            args.properties = updates.properties;
        }

        const response = await this.client.databases.update(args);
        return this.mapDatabase(response);
    }

    /**
     * Lists teamspaces.
     * @returns {Promise<ITeamspace[]>} Empty list (not fully supported).
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
        return this.mapDatabase(response);
    }

    /**
     * Retrieves a block by its ID.
     * @param {string} blockId - The UUID of the block.
     * @returns {Promise<IBlock>} The block details.
     */
    async getBlock(blockId: string): Promise<IBlock> {
        const response = await this.client.blocks.retrieve({ block_id: blockId });
        return this.mapBlock(response);
    }

    /**
     * Updates a block's content.
     * @param {string} blockId - The UUID of the block.
     * @param {string} content - The new content.
     * @returns {Promise<IBlock>} The updated block.
     */
    async updateBlock(blockId: string, content: string): Promise<IBlock> {
        // We first need to know the type of the block to update it correctly
        const block = await this.client.blocks.retrieve({ block_id: blockId });
        const type = (block as any).type;

        const update: any = {
            block_id: blockId,
            [type]: {
                rich_text: [{ text: { content } }],
            },
        };

        const response = await this.client.blocks.update(update);
        return this.mapBlock(response);
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
        const blocks: BlockObjectRequest[] = children.map((child) => ({
            object: "block",
            type: child.type as any, // assuming type is valid
            [child.type]: {
                rich_text: [{ text: { content: child.content } }],
            },
        })) as unknown as BlockObjectRequest[];

        const response = await this.client.blocks.children.append({
            block_id: blockId,
            children: blocks,
        });

        return response.results.map((b) => this.mapBlock(b));
    }

    private mapSearchResult(item: any): IPage | IDatabase {
        if (item.object === "page") {
            return this.mapPage(item);
        } else {
            return this.mapDatabase(item);
        }
    }

    private mapPage(item: any): IPage {
        let title = "Untitled";
        if (item.properties) {
            for (const key in item.properties) {
                if (item.properties[key].type === "title") {
                    const titleObj = item.properties[key].title;
                    if (titleObj && titleObj.length > 0) {
                        title = titleObj[0].plain_text;
                    }
                    break;
                }
            }
        }

        return {
            object: "page",
            id: item.id,
            url: item.url,
            created_time: item.created_time,
            last_edited_time: item.last_edited_time,
            title,
            parent: item.parent,
            properties: item.properties,
        };
    }

    private mapDatabase(item: any): IDatabase {
        return {
            object: "database",
            id: item.id,
            url: item.url,
            title: item.title?.[0]?.plain_text || "Untitled",
            created_time: item.created_time,
            last_edited_time: item.last_edited_time,
            properties: item.properties,
        };
    }

    private mapUser(item: any): IUser {
        return {
            id: item.id,
            name: item.name,
            email: item.person?.email,
            type: item.type,
            avatar_url: item.avatar_url,
        };
    }

    private mapBlock(item: any): IBlock {
        let content = undefined;
        const type = item.type;
        if (item[type] && item[type].rich_text && Array.isArray(item[type].rich_text)) {
            content = item[type].rich_text.map((t: any) => t.plain_text).join("");
        }

        return {
            id: item.id,
            type: item.type,
            content,
            has_children: item.has_children,
            parent: item.parent,
        };
    }

    private mapComment(item: any): IComment {
        return {
            id: item.id,
            text: item.rich_text.map((t: any) => t.plain_text).join(""),
            created_by: this.mapUser(item.created_by),
            created_time: item.created_time,
            discussion_id: item.discussion_id,
        };
    }
}
