import { IGitHubRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface RemoveLabelInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {number} issueNumber - Issue or PR number
 * @property {string} labelName - Label name to remove
 */
export interface RemoveLabelInput {
    owner: string;
    repo: string;
    issueNumber: number;
    labelName: string;
}

/**
 * @class RemoveLabelUseCase
 * @implements {IUseCase<RemoveLabelInput, void>}
 * @description Removes a label from an issue or pull request
 */
export class RemoveLabelUseCase implements IUseCase<RemoveLabelInput, void> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {RemoveLabelInput} input - Remove label parameters
     * @returns {Promise<ISkillResult<void>>} Success status or error
     */
    async execute(input: RemoveLabelInput): Promise<ISkillResult<void>> {
        try {
            await this.repository.removeLabelFromIssue(input.owner, input.repo, input.issueNumber, input.labelName);
            return { success: true };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
