import { ICommit, IGitHubRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface GetCommitInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {string} sha - Commit SHA or reference
 */
export interface GetCommitInput {
    owner: string;
    repo: string;
    sha: string;
}

/**
 * @class GetCommitUseCase
 * @implements {IUseCase<GetCommitInput, ICommit>}
 * @description Retrieves a specific commit by SHA or reference
 */
export class GetCommitUseCase implements IUseCase<GetCommitInput, ICommit> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {GetCommitInput} input - Commit retrieval parameters
     * @returns {Promise<ISkillResult<ICommit>>} Commit details or error
     */
    async execute(input: GetCommitInput): Promise<ISkillResult<ICommit>> {
        try {
            const result = await this.repository.getCommit(input.owner, input.repo, input.sha);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
