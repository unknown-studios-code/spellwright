import { IGitHubRepository, IPullRequestReview, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface CreatePRReviewInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {number} prNumber - Pull request number
 * @property {"APPROVE" | "REQUEST_CHANGES" | "COMMENT"} event - Review event
 * @property {string} [body] - Review comment
 */
export interface CreatePRReviewInput {
    owner: string;
    repo: string;
    prNumber: number;
    event: "APPROVE" | "REQUEST_CHANGES" | "COMMENT";
    body?: string;
}

/**
 * @class CreatePRReviewUseCase
 * @implements {IUseCase<CreatePRReviewInput, IPullRequestReview>}
 * @description Creates a review on a pull request
 */
export class CreatePRReviewUseCase implements IUseCase<CreatePRReviewInput, IPullRequestReview> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {CreatePRReviewInput} input - Review creation parameters
     * @returns {Promise<ISkillResult<IPullRequestReview>>} Created review or error
     */
    async execute(input: CreatePRReviewInput): Promise<ISkillResult<IPullRequestReview>> {
        try {
            const result = await this.repository.createPullRequestReview(input.owner, input.repo, input.prNumber, input.event, input.body);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
