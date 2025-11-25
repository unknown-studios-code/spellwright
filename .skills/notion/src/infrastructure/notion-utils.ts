import { BlockObjectRequest } from "@notionhq/client/build/src/api-endpoints";
import { IBlock, IComment, IDatabase, IPage, IUser } from "../domain/entities";

/**
 * Maps a raw Notion page response to the IPage interface.
 * @param {any} item - The raw page object from Notion API.
 * @returns {IPage} The mapped page.
 */
export function mapPage(item: any): IPage {
    return {
        object: "page",
        id: item.id,
        url: item.url,
        created_time: item.created_time,
        last_edited_time: item.last_edited_time,
        title: extractTitle(item.properties),
        parent: item.parent,
        properties: item.properties,
    };
}

/**
 * Maps a raw Notion database response to the IDatabase interface.
 * @param {any} item - The raw database object from Notion API.
 * @returns {IDatabase} The mapped database.
 */
export function mapDatabase(item: any): IDatabase {
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

/**
 * Maps a raw Notion user response to the IUser interface.
 * @param {any} item - The raw user object from Notion API.
 * @returns {IUser} The mapped user.
 */
export function mapUser(item: any): IUser {
    return {
        id: item.id,
        name: item.name,
        email: item.person?.email,
        type: item.type,
        avatar_url: item.avatar_url,
    };
}

/**
 * Maps a raw Notion block response to the IBlock interface.
 * @param {any} item - The raw block object from Notion API.
 * @returns {IBlock} The mapped block.
 */
export function mapBlock(item: any): IBlock {
    const type = item.type;
    const richText = item[type]?.rich_text;
    const content = Array.isArray(richText) ? richText.map((t: any) => t.plain_text).join("") : undefined;

    return {
        id: item.id,
        type,
        content,
        has_children: item.has_children,
        parent: item.parent,
    };
}

/**
 * Maps a raw Notion comment response to the IComment interface.
 * @param {any} item - The raw comment object from Notion API.
 * @returns {IComment} The mapped comment.
 */
export function mapComment(item: any): IComment {
    return {
        id: item.id,
        text: item.rich_text.map((t: any) => t.plain_text).join(""),
        created_by: mapUser(item.created_by),
        created_time: item.created_time,
        discussion_id: item.discussion_id,
    };
}

/**
 * Maps a raw Notion search result to either IPage or IDatabase.
 * @param {any} item - The raw search result from Notion API.
 * @returns {IPage | IDatabase} The mapped page or database.
 */
export function mapSearchResult(item: any): IPage | IDatabase {
    return item.object === "page" ? mapPage(item) : mapDatabase(item);
}

/**
 * Extracts the title from a Notion page's properties.
 * @param {any} properties - The page properties object.
 * @returns {string} The extracted title or "Untitled".
 */
export function extractTitle(properties: any): string {
    if (!properties) return "Untitled";

    for (const key in properties) {
        if (properties[key].type === "title") {
            const titleArr = properties[key].title;
            if (titleArr?.length > 0) {
                return titleArr[0].plain_text;
            }
        }
    }
    return "Untitled";
}

/**
 * Creates a paragraph block request object.
 * @param {string} content - The text content of the paragraph.
 * @returns {BlockObjectRequest} The block request object.
 */
export function createParagraphBlock(content: string): BlockObjectRequest {
    return {
        object: "block",
        type: "paragraph",
        paragraph: { rich_text: [{ text: { content } }] },
    } as BlockObjectRequest;
}

/**
 * Creates a title property object for page creation.
 * @param {string} title - The title text.
 * @param {boolean} isDatabase - Whether the parent is a database.
 * @returns {Record<string, any>} The properties object with title.
 */
export function createTitleProperty(title: string, isDatabase: boolean): Record<string, any> {
    const titleValue = [{ type: "text", text: { content: title } }];
    return isDatabase ? { Name: { title: titleValue } } : { title: titleValue };
}

/**
 * Parses a task ID string into prefix and number components.
 * @param {string} taskId - The task ID (e.g., "SPWT-11").
 * @returns {{ prefix: string; number: number } | null} The parsed components or null if invalid.
 */
export function parseTaskId(taskId: string): { prefix: string; number: number } | null {
    const match = taskId.match(/^([A-Z]+)-(\d+)$/);
    return match ? { prefix: match[1], number: parseInt(match[2], 10) } : null;
}

/**
 * Generic pagination helper for Notion API endpoints.
 * @template T - The type of items returned by the API.
 * @template R - The type of mapped results.
 * @param {Function} fetchFn - Function that fetches a page of results.
 * @param {Function} mapFn - Function that maps each item.
 * @returns {Promise<R[]>} All mapped results across all pages.
 */
export async function paginate<T, R>(fetchFn: (cursor?: string) => Promise<{ results: T[]; next_cursor: string | null; has_more: boolean }>, mapFn: (item: T) => R): Promise<R[]> {
    const items: R[] = [];
    let cursor: string | undefined = undefined;
    let hasMore = true;

    while (hasMore) {
        const response = await fetchFn(cursor);
        items.push(...response.results.map(mapFn));
        cursor = response.next_cursor ?? undefined;
        hasMore = response.has_more;
    }

    return items;
}
