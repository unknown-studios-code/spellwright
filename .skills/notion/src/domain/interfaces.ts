import { IBlock, IComment, IDatabase, IPage, ISearchResult, ITeamspace, IUser } from "./entities";

export { IBlock, IComment, IDatabase, IPage, ISearchResult, ITeamspace, IUser } from "./entities";

/**
 * Interface for the Notion Repository, defining the contract for interacting with the Notion API.
 * @interface INotionRepository
 */
export interface INotionRepository {
    /**
     * Searches for pages or databases.
     * @param {string} [query] - The search query string.
     * @param {"last_edited_time" | "relevance"} [sort] - Sort criteria.
     * @returns {Promise<ISearchResult>} The search results.
     */
    search(query?: string, sort?: "last_edited_time" | "relevance"): Promise<ISearchResult>;

    /**
     * Retrieves a page by its ID.
     * @param {string} pageId - The UUID of the page.
     * @returns {Promise<IPage>} The page details.
     */
    getPage(pageId: string): Promise<IPage>;

    /**
     * Gets a page by its task ID property across all databases in the workspace.
     * @param {string} taskId - The task ID (e.g., "SPWT-1").
     * @returns {Promise<IPage | null>} The page if found, null otherwise.
     */
    getPageByTaskId(taskId: string): Promise<IPage | null>;

    /**
     * Retrieves a database by its ID.
     * @param {string} databaseId - The UUID of the database.
     * @returns {Promise<IDatabase>} The database details.
     */
    getDatabase(databaseId: string): Promise<IDatabase>;

    /**
     * Retrieves a block by its ID.
     * @param {string} blockId - The UUID of the block.
     * @returns {Promise<IBlock>} The block details.
     */
    getBlock(blockId: string): Promise<IBlock>;

    /**
     * Updates a block's content.
     * @param {string} blockId - The UUID of the block.
     * @param {string} content - The new content.
     * @returns {Promise<IBlock>} The updated block.
     */
    updateBlock(blockId: string, content: string): Promise<IBlock>;

    /**
     * Deletes a block.
     * @param {string} blockId - The UUID of the block.
     */
    deleteBlock(blockId: string): Promise<void>;

    /**
     * Appends children blocks to a parent block.
     * @param {string} blockId - The UUID of the parent block.
     * @param {Array<{ type: string; content: string }>} children - The children to append.
     * @returns {Promise<IBlock[]>} The appended blocks.
     */
    appendBlockChildren(blockId: string, children: { type: string; content: string }[]): Promise<IBlock[]>;

    /**
     * Retrieves the content (blocks) of a page or block.
     * @param {string} blockId - The UUID of the page or block.
     * @returns {Promise<IBlock[]>} A list of child blocks.
     */
    getPageContent(blockId: string): Promise<IBlock[]>;

    /**
     * Lists all users in the workspace.
     * @returns {Promise<IUser[]>} A list of users.
     */
    listUsers(): Promise<IUser[]>;

    /**
     * Retrieves a user by their ID.
     * @param {string} userId - The UUID of the user.
     * @returns {Promise<IUser>} The user details.
     */
    getUser(userId: string): Promise<IUser>;

    /**
     * Retrieves information about the bot itself.
     * @returns {Promise<{ bot: IUser; workspace_name?: string }>} The bot user and optional workspace name.
     */
    getBotInfo(): Promise<{ bot: IUser; workspace_name?: string }>;

    /**
     * Creates a comment on a page.
     * @param {string} pageId - The UUID of the page to comment on.
     * @param {string} content - The text content of the comment.
     * @returns {Promise<IComment>} The created comment.
     */
    createComment(pageId: string, content: string): Promise<IComment>;

    /**
     * Lists comments for a block or page.
     * @param {string} blockId - The UUID of the block or page.
     * @returns {Promise<IComment[]>} A list of comments.
     */
    listComments(blockId: string): Promise<IComment[]>;

    /**
     * Creates a new page.
     * @param {Object} parent - The parent location (page or database).
     * @param {string} [parent.page_id] - Parent page UUID.
     * @param {string} [parent.database_id] - Parent database UUID.
     * @param {string} title - The title of the new page.
     * @param {string} [content] - Initial text content for the page.
     * @returns {Promise<IPage>} The created page.
     */
    createPage(parent: { page_id?: string; database_id?: string }, title: string, content?: string): Promise<IPage>;

    /**
     * Updates a page's properties or archive status.
     * @param {string} pageId - The UUID of the page.
     * @param {Record<string, any>} [properties] - Properties to update.
     * @param {boolean} [archived] - Whether to archive the page.
     * @returns {Promise<IPage>} The updated page.
     */
    updatePage(pageId: string, properties?: Record<string, any>, archived?: boolean): Promise<IPage>;

    /**
     * Moves a page to a new parent. (Note: Often implemented as Copy & Delete).
     * @param {string} pageId - The UUID of the page to move.
     * @param {string} newParentId - The UUID of the new parent.
     * @returns {Promise<IPage>} The moved (new) page.
     */
    movePage(pageId: string, newParentId: string): Promise<IPage>;

    /**
     * Duplicates a page.
     * @param {string} pageId - The UUID of the page to duplicate.
     * @returns {Promise<IPage>} The duplicated page.
     */
    duplicatePage(pageId: string): Promise<IPage>;

    /**
     * Creates a new database.
     * @param {string} parentId - The UUID of the parent page.
     * @param {string} title - The title of the database.
     * @param {Record<string, any>} properties - The schema/properties of the database.
     * @returns {Promise<IDatabase>} The created database.
     */
    createDatabase(parentId: string, title: string, properties: Record<string, any>): Promise<IDatabase>;

    /**
     * Updates a database.
     * @param {string} databaseId - The UUID of the database.
     * @param {Object} updates - The updates to apply.
     * @param {string} [updates.title] - New title.
     * @param {Record<string, any>} [updates.properties] - Property updates.
     * @returns {Promise<IDatabase>} The updated database.
     */
    updateDatabase(databaseId: string, updates: { title?: string; properties?: Record<string, any> }): Promise<IDatabase>;

    /**
     * Lists teamspaces (if supported/accessible).
     * @returns {Promise<ITeamspace[]>} A list of teamspaces.
     */
    listTeamspaces(): Promise<ITeamspace[]>;
}

/**
 * Standard result wrapper for all skill operations.
 * @template T
 * @interface ISkillResult
 * @property {boolean} success - Whether the operation was successful.
 * @property {T} [data] - The result data if successful.
 * @property {string} [error] - The error message if failed.
 */
export interface ISkillResult<T> {
    success: boolean;
    data?: T;
    error?: string;
}

/**
 * Base interface for Use Cases in the Clean Architecture layers.
 * @template TInput, TOutput
 * @interface IUseCase
 */
export interface IUseCase<TInput, TOutput> {
    /**
     * Executes the use case.
     * @param {TInput} input - The input parameters.
     * @returns {Promise<ISkillResult<TOutput>>} The result of the operation.
     */
    execute(input: TInput): Promise<ISkillResult<TOutput>>;
}
