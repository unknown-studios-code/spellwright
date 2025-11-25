import { IGitHubRepository, IPullRequestReview, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface ListPRReviewsInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {number} prNumber - Pull request number
 */
export interface ListPRReviewsInput {
    owner: string;
    repo: string;
    prNumber: number;
}

/**
 * @class ListPRReviewsUseCase
 * @implements {IUseCase<ListPRReviewsInput, IPullRequestReview[]>}
 * @description Lists reviews on a pull request
 */
export class ListPRReviewsUseCase implements IUseCase<ListPRReviewsInput, IPullRequestReview[]> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {ListPRReviewsInput} input - List parameters
     * @returns {Promise<ISkillResult<IPullRequestReview[]>>} List of reviews or error
     */
    async execute(input: ListPRReviewsInput): Promise<ISkillResult<IPullRequestReview[]>> {
        try {
            const result = await this.repository.listPullRequestReviews(input.owner, input.repo, input.prNumber);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
