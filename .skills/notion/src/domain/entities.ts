/**
 * Represents a Notion User (Person or Bot).
 * @interface IUser
 * @property {string} id - The unique identifier for the user (UUID).
 * @property {string} [name] - The display name of the user.
 * @property {string} [email] - The email address of the user (if available).
 * @property {"person" | "bot"} type - The type of user: "person" or "bot".
 * @property {string} [avatar_url] - The URL of the user's avatar image.
 */
export interface IUser {
    id: string;
    name?: string;
    email?: string;
    type: "person" | "bot";
    avatar_url?: string;
}

/**
 * Represents a Notion Page.
 * @interface IPage
 * @property {string} object - Discriminator field, always "page".
 * @property {string} id - The unique identifier for the page (UUID).
 * @property {string} url - The permalink URL of the page.
 * @property {string} created_time - The ISO 8601 creation timestamp.
 * @property {string} last_edited_time - The ISO 8601 last edited timestamp.
 * @property {string} title - The title of the page.
 * @property {Object} [parent] - Information about the parent object.
 * @property {Record<string, any>} properties - The properties of the page.
 */
export interface IPage {
    object: "page";
    id: string;
    url: string;
    created_time: string;
    last_edited_time: string;
    title: string;
    parent?: {
        type: "database_id" | "page_id" | "workspace" | "block_id";
        database_id?: string;
        page_id?: string;
        block_id?: string;
        workspace?: boolean;
    };
    properties: Record<string, any>;
}

/**
 * Represents a Notion Database.
 * @interface IDatabase
 * @property {string} object - Discriminator field, always "database".
 * @property {string} id - The unique identifier for the database (UUID).
 * @property {string} url - The permalink URL of the database.
 * @property {string} title - The title of the database.
 * @property {string} created_time - The ISO 8601 creation timestamp.
 * @property {string} last_edited_time - The ISO 8601 last edited timestamp.
 * @property {Record<string, any>} properties - The properties/schema of the database.
 */
export interface IDatabase {
    object: "database";
    id: string;
    url: string;
    title: string;
    created_time: string;
    last_edited_time: string;
    properties: Record<string, any>;
}

/**
 * Represents a block within a Notion page.
 * @interface IBlock
 * @property {string} id - The unique identifier for the block (UUID).
 * @property {string} type - The type of the block (e.g., "paragraph", "heading_1").
 * @property {string} [content] - The plain text content of the block, if applicable.
 * @property {boolean} has_children - Indicates if the block has nested child blocks.
 * @property {any} [parent] - Information about the parent block or page.
 */
export interface IBlock {
    id: string;
    type: string;
    content?: string;
    has_children: boolean;
    parent?: any;
}

/**
 * Represents a comment on a Notion page or block.
 * @interface IComment
 * @property {string} id - The unique identifier for the comment (UUID).
 * @property {string} text - The plain text content of the comment.
 * @property {IUser} created_by - The user who created the comment.
 * @property {string} created_time - The ISO 8601 creation timestamp.
 * @property {string} [discussion_id] - The ID of the discussion thread.
 */
export interface IComment {
    id: string;
    text: string;
    created_by: IUser;
    created_time: string;
    discussion_id?: string;
}

/**
 * Represents the result of a search operation.
 * @interface ISearchResult
 * @property {Array<IPage | IDatabase>} results - The list of pages or databases found.
 * @property {string | null} next_cursor - The cursor for the next page of results, or null if none.
 * @property {boolean} has_more - Indicates if there are more results available.
 */
export interface ISearchResult {
    results: (IPage | IDatabase)[];
    next_cursor: string | null;
    has_more: boolean;
}

/**
 * Represents a Notion Teamspace.
 * @interface ITeamspace
 * @property {string} id - The unique identifier for the teamspace (UUID).
 * @property {string} name - The name of the teamspace.
 */
export interface ITeamspace {
    id: string;
    name: string;
}
