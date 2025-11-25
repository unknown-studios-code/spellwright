import { INotionRepository, ISearchResult, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for searching.
 * @interface SearchInput
 * @property {string} [query] - The text to search for.
 * @property {"last_edited_time" | "relevance"} [sort] - Sort order.
 */
export interface SearchInput {
    query?: string;
    sort?: "last_edited_time" | "relevance";
}

/**
 * Use case for searching pages and databases.
 * @class SearchUseCase
 * @implements {IUseCase<SearchInput, ISearchResult>}
 */
export class SearchUseCase implements IUseCase<SearchInput, ISearchResult> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the search use case.
     * @param {SearchInput} input - The input data.
     * @returns {Promise<ISkillResult<ISearchResult>>} The search results or error.
     */
    async execute(input: SearchInput): Promise<ISkillResult<ISearchResult>> {
        try {
            const result = await this.repository.search(input.query, input.sort);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
