import { IGitHubRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface StarRepositoryInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 */
export interface StarRepositoryInput {
    owner: string;
    repo: string;
}

/**
 * @class StarRepositoryUseCase
 * @implements {IUseCase<StarRepositoryInput, void>}
 * @description Stars a repository
 */
export class StarRepositoryUseCase implements IUseCase<StarRepositoryInput, void> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {StarRepositoryInput} input - Star parameters
     * @returns {Promise<ISkillResult<void>>} Success status or error
     */
    async execute(input: StarRepositoryInput): Promise<ISkillResult<void>> {
        try {
            await this.repository.starRepository(input.owner, input.repo);
            return { success: true };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
