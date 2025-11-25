import { IGitHubRepository, IPullRequest, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface ListPullRequestsInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {"open" | "closed" | "all"} [state] - Pull request state filter
 */
export interface ListPullRequestsInput {
    owner: string;
    repo: string;
    state?: "open" | "closed" | "all";
}

/**
 * @class ListPullRequestsUseCase
 * @implements {IUseCase<ListPullRequestsInput, IPullRequest[]>}
 * @description Lists pull requests in a repository
 */
export class ListPullRequestsUseCase implements IUseCase<ListPullRequestsInput, IPullRequest[]> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {ListPullRequestsInput} input - List parameters
     * @returns {Promise<ISkillResult<IPullRequest[]>>} List of pull requests or error
     */
    async execute(input: ListPullRequestsInput): Promise<ISkillResult<IPullRequest[]>> {
        try {
            const result = await this.repository.listPullRequests(input.owner, input.repo, input.state);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
