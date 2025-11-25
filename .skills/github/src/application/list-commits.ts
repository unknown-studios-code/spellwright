import { IGitHubRepository, IUseCase, ISkillResult, ICommit } from "../domain/interfaces";

/**
 * @interface ListCommitsInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {string} [branch] - Branch name or SHA
 * @property {number} [limit] - Maximum number of commits to return
 */
export interface ListCommitsInput {
    owner: string;
    repo: string;
    branch?: string;
    limit?: number;
}

/**
 * @class ListCommitsUseCase
 * @implements {IUseCase<ListCommitsInput, ICommit[]>}
 * @description Lists commits in a repository
 */
export class ListCommitsUseCase implements IUseCase<ListCommitsInput, ICommit[]> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {ListCommitsInput} input - List parameters
     * @returns {Promise<ISkillResult<ICommit[]>>} List of commits or error
     */
    async execute(input: ListCommitsInput): Promise<ISkillResult<ICommit[]>> {
        try {
            const result = await this.repository.listCommits(input.owner, input.repo, input.branch, input.limit);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
