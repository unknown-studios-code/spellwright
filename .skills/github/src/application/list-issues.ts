import { IGitHubRepository, IUseCase, ISkillResult, IIssue } from "../domain/interfaces";

/**
 * @interface ListIssuesInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {"open" | "closed" | "all"} [state] - Issue state filter
 */
export interface ListIssuesInput {
    owner: string;
    repo: string;
    state?: "open" | "closed" | "all";
}

/**
 * @class ListIssuesUseCase
 * @implements {IUseCase<ListIssuesInput, IIssue[]>}
 * @description Lists issues in a repository
 */
export class ListIssuesUseCase implements IUseCase<ListIssuesInput, IIssue[]> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {ListIssuesInput} input - List parameters
     * @returns {Promise<ISkillResult<IIssue[]>>} List of issues or error
     */
    async execute(input: ListIssuesInput): Promise<ISkillResult<IIssue[]>> {
        try {
            const result = await this.repository.listIssues(input.owner, input.repo, input.state);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
