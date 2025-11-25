import { IGitHubRepository, IUseCase, ISkillResult } from "../domain/interfaces";

/**
 * @interface MergePullRequestInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {number} prNumber - Pull request number
 * @property {"merge" | "squash" | "rebase"} [method] - Merge method
 */
export interface MergePullRequestInput {
    owner: string;
    repo: string;
    prNumber: number;
    method?: "merge" | "squash" | "rebase";
}

/**
 * @class MergePullRequestUseCase
 * @implements {IUseCase<MergePullRequestInput, {merged: boolean, message: string}>}
 * @description Merges a pull request
 */
export class MergePullRequestUseCase implements IUseCase<MergePullRequestInput, { merged: boolean; message: string }> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {MergePullRequestInput} input - Merge parameters
     * @returns {Promise<ISkillResult<{merged: boolean, message: string}>>} Merge result or error
     */
    async execute(input: MergePullRequestInput): Promise<ISkillResult<{ merged: boolean; message: string }>> {
        try {
            const result = await this.repository.mergePullRequest(input.owner, input.repo, input.prNumber, input.method);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
