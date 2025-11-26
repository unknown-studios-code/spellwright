/**
 * Notion MCP Integration Module
 *
 * This module provides type definitions and documentation for interacting with
 * the Notion MCP server. Since MCP calls are made by the agent, this module
 * serves as a reference for the expected parameters and return types.
 *
 * MCP Server: project-0-spellwright-Notion
 */

import { DATA_SOURCE_IDS, DataSourceType, NotionPageProperties } from "./types";

// MCP Tool Names
export const NOTION_TOOLS = {
    search: "notion-search",
    fetch: "notion-fetch",
    createPages: "notion-create-pages",
    updatePage: "notion-update-page",
} as const;

// MCP Server Identifier
export const NOTION_SERVER = "project-0-spellwright-Notion";

/**
 * Get the data source ID for a given artifact type
 */
export function getDataSourceId(type: DataSourceType): string {
    return DATA_SOURCE_IDS[type];
}

/**
 * Build the collection URL for a data source
 */
export function getCollectionUrl(type: DataSourceType): string {
    return `collection://${DATA_SOURCE_IDS[type]}`;
}

/**
 * Search Parameters for notion-search
 *
 * @example
 * {
 *   query: "SPWT-41",
 *   query_type: "internal"
 * }
 */
export interface NotionSearchParams {
    query: string;
    query_type: "internal" | "user";
    data_source_url?: string;
    page_url?: string;
    teamspace_id?: string;
    filters?: {
        created_date_range?: {
            start_date?: string;
            end_date?: string;
        };
        created_by_user_ids?: string[];
    };
}

/**
 * Fetch Parameters for notion-fetch
 *
 * @example
 * {
 *   id: "28f56d55-129b-8081-83f7-c09246a7e8ce"
 * }
 */
export interface NotionFetchParams {
    id: string; // Page ID or URL
}

/**
 * Create Page Parameters for notion-create-pages
 *
 * @example
 * {
 *   parent: { data_source_id: "28e56d55-129b-8181-b82b-000b7a52fcfb" },
 *   pages: [{
 *     properties: { "Name": "Task Title", "Priority": "High", "Type": "Feature" },
 *     content: "### Description\n..."
 *   }]
 * }
 */
export interface NotionCreatePagesParams {
    parent?: {
        page_id?: string;
        database_id?: string;
        data_source_id?: string;
    };
    pages: Array<{
        properties: NotionPageProperties;
        content?: string;
    }>;
}

/**
 * Update Page Parameters for notion-update-page
 *
 * Commands:
 * - update_properties: Update page properties only
 * - replace_content: Replace entire page content
 * - replace_content_range: Replace specific content range
 * - insert_content_after: Insert content after specific text
 *
 * @example
 * {
 *   data: {
 *     page_id: "28f56d55-129b-8081-83f7-c09246a7e8ce",
 *     command: "update_properties",
 *     properties: { "Status": "In progress" }
 *   }
 * }
 */
export interface NotionUpdatePageParams {
    data: {
        page_id: string;
        command: "update_properties" | "replace_content" | "replace_content_range" | "insert_content_after";
        properties?: NotionPageProperties;
        new_str?: string;
        selection_with_ellipsis?: string;
    };
}

/**
 * Build search parameters for finding an artifact by ID
 */
export function buildSearchByIdParams(id: string): NotionSearchParams {
    return {
        query: id,
        query_type: "internal",
    };
}

/**
 * Build create page parameters for a new artifact
 */
export function buildCreatePageParams(type: DataSourceType, properties: NotionPageProperties, content: string): NotionCreatePagesParams {
    return {
        parent: {
            data_source_id: DATA_SOURCE_IDS[type],
        },
        pages: [
            {
                properties,
                content,
            },
        ],
    };
}

/**
 * Build update properties parameters
 */
export function buildUpdatePropertiesParams(pageId: string, properties: NotionPageProperties): NotionUpdatePageParams {
    return {
        data: {
            page_id: pageId,
            command: "update_properties",
            properties,
        },
    };
}

/**
 * Build replace content parameters
 */
export function buildReplaceContentParams(pageId: string, content: string): NotionUpdatePageParams {
    return {
        data: {
            page_id: pageId,
            command: "replace_content",
            new_str: content,
        },
    };
}

/**
 * Extract page ID from Notion fetch result
 * The result contains URL in format: https://www.notion.so/<id>
 */
export function extractPageIdFromResult(url: string): string | null {
    const match = url.match(/([0-9a-f]{32})/i);
    return match ? match[1] : null;
}

/**
 * Format properties for Epic creation
 */
export function buildEpicProperties(name: string, priority: string, status: string = "Not started"): NotionPageProperties {
    return {
        Name: name,
        Priority: priority,
        Status: status,
    };
}

/**
 * Format properties for Story creation
 */
export function buildStoryProperties(name: string, priority: string, epicUrl: string, status: string = "Not started"): NotionPageProperties {
    return {
        Name: name,
        Priority: priority,
        Status: status,
        Epic: epicUrl,
    };
}

/**
 * Format properties for Task creation
 */
export function buildTaskProperties(name: string, priority: string, taskType: string, storyUrl: string, status: string = "Not started"): NotionPageProperties {
    return {
        Name: name,
        Priority: priority,
        Status: status,
        Type: taskType.charAt(0).toUpperCase() + taskType.slice(1).toLowerCase(),
        Story: storyUrl,
    };
}

/**
 * MCP Call Examples (for agent reference)
 *
 * Search for a task:
 * ```
 * CallMcpTool
 * server: project-0-spellwright-Notion
 * toolName: notion-search
 * arguments: { "query": "SPWT-41", "query_type": "internal" }
 * ```
 *
 * Fetch page details:
 * ```
 * CallMcpTool
 * server: project-0-spellwright-Notion
 * toolName: notion-fetch
 * arguments: { "id": "28f56d55-129b-8081-83f7-c09246a7e8ce" }
 * ```
 *
 * Create a task:
 * ```
 * CallMcpTool
 * server: project-0-spellwright-Notion
 * toolName: notion-create-pages
 * arguments: {
 *   "parent": { "data_source_id": "28e56d55-129b-8181-b82b-000b7a52fcfb" },
 *   "pages": [{
 *     "properties": { "Name": "Task Title", "Priority": "High", "Type": "Feature" },
 *     "content": "### Description\n..."
 *   }]
 * }
 * ```
 *
 * Update task status:
 * ```
 * CallMcpTool
 * server: project-0-spellwright-Notion
 * toolName: notion-update-page
 * arguments: {
 *   "data": {
 *     "page_id": "28f56d55-129b-8081-83f7-c09246a7e8ce",
 *     "command": "update_properties",
 *     "properties": { "Status": "In progress" }
 *   }
 * }
 * ```
 */
