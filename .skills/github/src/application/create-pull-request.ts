import { IGitHubRepository, IUseCase, ISkillResult, IPullRequest } from "../domain/interfaces";

/**
 * @interface CreatePullRequestInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {string} title - Pull request title
 * @property {string} head - Source branch
 * @property {string} base - Target branch
 * @property {string} [body] - Pull request description
 */
export interface CreatePullRequestInput {
    owner: string;
    repo: string;
    title: string;
    head: string;
    base: string;
    body?: string;
}

/**
 * @class CreatePullRequestUseCase
 * @implements {IUseCase<CreatePullRequestInput, IPullRequest>}
 * @description Creates a new pull request
 */
export class CreatePullRequestUseCase implements IUseCase<CreatePullRequestInput, IPullRequest> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {CreatePullRequestInput} input - Pull request creation parameters
     * @returns {Promise<ISkillResult<IPullRequest>>} Created pull request or error
     */
    async execute(input: CreatePullRequestInput): Promise<ISkillResult<IPullRequest>> {
        try {
            const result = await this.repository.createPullRequest(input.owner, input.repo, input.title, input.head, input.base, input.body);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
