import { IGitHubRepository, IUseCase, ISkillResult, IBranch } from "../domain/interfaces";

/**
 * @interface ListBranchesInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 */
export interface ListBranchesInput {
    owner: string;
    repo: string;
}

/**
 * @class ListBranchesUseCase
 * @implements {IUseCase<ListBranchesInput, IBranch[]>}
 * @description Lists all branches in a repository
 */
export class ListBranchesUseCase implements IUseCase<ListBranchesInput, IBranch[]> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {ListBranchesInput} input - List parameters
     * @returns {Promise<ISkillResult<IBranch[]>>} List of branches or error
     */
    async execute(input: ListBranchesInput): Promise<ISkillResult<IBranch[]>> {
        try {
            const result = await this.repository.listBranches(input.owner, input.repo);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
