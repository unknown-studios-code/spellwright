import { IBranch, IGitHubRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface CreateBranchInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {string} branchName - New branch name
 * @property {string} sourceSha - Source commit SHA
 */
export interface CreateBranchInput {
    owner: string;
    repo: string;
    branchName: string;
    sourceSha: string;
}

/**
 * @class CreateBranchUseCase
 * @implements {IUseCase<CreateBranchInput, IBranch>}
 * @description Creates a new branch from a commit
 */
export class CreateBranchUseCase implements IUseCase<CreateBranchInput, IBranch> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {CreateBranchInput} input - Branch creation parameters
     * @returns {Promise<ISkillResult<IBranch>>} Created branch or error
     */
    async execute(input: CreateBranchInput): Promise<ISkillResult<IBranch>> {
        try {
            const result = await this.repository.createBranch(input.owner, input.repo, input.branchName, input.sourceSha);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
