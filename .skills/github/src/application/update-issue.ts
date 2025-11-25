import { IGitHubRepository, IUseCase, ISkillResult, IIssue } from "../domain/interfaces";

/**
 * @interface UpdateIssueInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {number} issueNumber - Issue number
 * @property {Object} updates - Fields to update
 * @property {string} [updates.title] - New title
 * @property {string} [updates.body] - New body
 * @property {"open" | "closed"} [updates.state] - New state
 */
export interface UpdateIssueInput {
    owner: string;
    repo: string;
    issueNumber: number;
    updates: {
        title?: string;
        body?: string;
        state?: "open" | "closed";
    };
}

/**
 * @class UpdateIssueUseCase
 * @implements {IUseCase<UpdateIssueInput, IIssue>}
 * @description Updates an existing issue
 */
export class UpdateIssueUseCase implements IUseCase<UpdateIssueInput, IIssue> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {UpdateIssueInput} input - Update parameters
     * @returns {Promise<ISkillResult<IIssue>>} Updated issue or error
     */
    async execute(input: UpdateIssueInput): Promise<ISkillResult<IIssue>> {
        try {
            const result = await this.repository.updateIssue(input.owner, input.repo, input.issueNumber, input.updates);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
