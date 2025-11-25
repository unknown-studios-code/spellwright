import { IGitHubRepository, ILabel, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface AddLabelsInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {number} issueNumber - Issue or PR number
 * @property {string[]} labels - Label names to add
 */
export interface AddLabelsInput {
    owner: string;
    repo: string;
    issueNumber: number;
    labels: string[];
}

/**
 * @class AddLabelsUseCase
 * @implements {IUseCase<AddLabelsInput, ILabel[]>}
 * @description Adds labels to an issue or pull request
 */
export class AddLabelsUseCase implements IUseCase<AddLabelsInput, ILabel[]> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {AddLabelsInput} input - Add labels parameters
     * @returns {Promise<ISkillResult<ILabel[]>>} Updated labels or error
     */
    async execute(input: AddLabelsInput): Promise<ISkillResult<ILabel[]>> {
        try {
            const result = await this.repository.addLabelsToIssue(input.owner, input.repo, input.issueNumber, input.labels);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
