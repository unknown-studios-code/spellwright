import { IGitHubRepository, IIssue, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface SearchIssuesInput
 * @property {string} query - GitHub issues search query
 */
export interface SearchIssuesInput {
    query: string;
}

/**
 * @class SearchIssuesUseCase
 * @implements {IUseCase<SearchIssuesInput, {items: IIssue[], total_count: number}>}
 * @description Searches issues and pull requests across GitHub
 */
export class SearchIssuesUseCase implements IUseCase<SearchIssuesInput, { items: IIssue[]; total_count: number }> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {SearchIssuesInput} input - Search parameters
     * @returns {Promise<ISkillResult<{items: IIssue[], total_count: number}>>} Search results or error
     */
    async execute(input: SearchIssuesInput): Promise<ISkillResult<{ items: IIssue[]; total_count: number }>> {
        try {
            const result = await this.repository.searchIssues(input.query);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
