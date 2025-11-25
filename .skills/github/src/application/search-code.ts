import { IGitHubRepository, IUseCase, ISkillResult, ISearchResult } from "../domain/interfaces";

/**
 * @interface SearchCodeInput
 * @property {string} query - GitHub code search query
 */
export interface SearchCodeInput {
    query: string;
}

/**
 * @class SearchCodeUseCase
 * @implements {IUseCase<SearchCodeInput, ISearchResult>}
 * @description Searches code across GitHub repositories
 */
export class SearchCodeUseCase implements IUseCase<SearchCodeInput, ISearchResult> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {SearchCodeInput} input - Search parameters
     * @returns {Promise<ISkillResult<ISearchResult>>} Search results or error
     */
    async execute(input: SearchCodeInput): Promise<ISkillResult<ISearchResult>> {
        try {
            const result = await this.repository.searchCode(input.query);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
