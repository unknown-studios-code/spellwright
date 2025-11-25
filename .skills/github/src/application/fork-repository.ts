import { IGitHubRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface ForkRepositoryInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 */
export interface ForkRepositoryInput {
    owner: string;
    repo: string;
}

/**
 * @class ForkRepositoryUseCase
 * @implements {IUseCase<ForkRepositoryInput, {html_url: string}>}
 * @description Forks a repository to the authenticated user's account
 */
export class ForkRepositoryUseCase implements IUseCase<ForkRepositoryInput, { html_url: string }> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {ForkRepositoryInput} input - Fork parameters
     * @returns {Promise<ISkillResult<{html_url: string}>>} Forked repository URL or error
     */
    async execute(input: ForkRepositoryInput): Promise<ISkillResult<{ html_url: string }>> {
        try {
            const result = await this.repository.forkRepository(input.owner, input.repo);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
