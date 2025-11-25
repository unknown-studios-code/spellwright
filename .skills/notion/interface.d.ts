/**
 * Standard result wrapper for all skill operations.
 */
export interface ISkillResult<T> {
    success: boolean;
    data?: T;
    error?: string;
}

/**
 * Notion Skills API
 * Import from: `../notion/index`
 */
export namespace Notion {
    // Entities
    export interface IPage {
        id: string;
        url: string;
        properties: Record<string, any>;
    }
    export interface IDatabase {
        id: string;
        url: string;
        title: string;
    }
    export interface IBlock {
        id: string;
        type: string;
        content?: string;
    }

    // Functions
    export function appendBlockChildren(blockId: string, children: { type: string; content: string }[]): Promise<ISkillResult<IBlock[]>>;
    export function createComment(pageId: string, content: string): Promise<ISkillResult<any>>;
    export function createDatabase(parentId: string, title: string, properties: Record<string, any>): Promise<ISkillResult<IDatabase>>;
    export function createPage(parentId: string, parentType: "page" | "database", title: string, content?: string): Promise<ISkillResult<IPage>>;
    export function deleteBlock(blockId: string): Promise<ISkillResult<void>>;
    export function duplicatePage(pageId: string): Promise<ISkillResult<IPage>>;
    export function getBlock(blockId: string): Promise<ISkillResult<IBlock>>;
    export function getBotInfo(): Promise<ISkillResult<any>>;
    export function getDatabase(databaseId: string): Promise<ISkillResult<IDatabase>>;
    export function getPage(pageId: string): Promise<ISkillResult<IPage>>;
    export function getPageContent(blockId: string): Promise<ISkillResult<IBlock[]>>;
    export function getUser(userId: string): Promise<ISkillResult<any>>;
    export function listComments(blockId: string): Promise<ISkillResult<any[]>>;
    export function listTeams(): Promise<ISkillResult<any[]>>;
    export function listUsers(): Promise<ISkillResult<any[]>>;
    export function movePage(pageId: string, newParentId: string): Promise<ISkillResult<IPage>>;
    export function search(query: string, sort?: "last_edited_time" | "relevance"): Promise<ISkillResult<any>>;
    export function updateBlock(blockId: string, content: string): Promise<ISkillResult<IBlock>>;
    export function updateDatabase(databaseId: string, updates: { title?: string; properties?: Record<string, any> }): Promise<ISkillResult<IDatabase>>;
    export function updatePage(pageId: string, properties?: Record<string, any>, archived?: boolean): Promise<ISkillResult<IPage>>;
}
