import { IGitHubRepository, IIssue, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface CreateIssueInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {string} title - Issue title
 * @property {string} [body] - Issue description
 */
export interface CreateIssueInput {
    owner: string;
    repo: string;
    title: string;
    body?: string;
}

/**
 * @class CreateIssueUseCase
 * @implements {IUseCase<CreateIssueInput, IIssue>}
 * @description Creates a new issue in a repository
 */
export class CreateIssueUseCase implements IUseCase<CreateIssueInput, IIssue> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {CreateIssueInput} input - Issue creation parameters
     * @returns {Promise<ISkillResult<IIssue>>} Created issue or error
     */
    async execute(input: CreateIssueInput): Promise<ISkillResult<IIssue>> {
        try {
            const result = await this.repository.createIssue(input.owner, input.repo, input.title, input.body);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
