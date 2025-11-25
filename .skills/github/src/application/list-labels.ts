import { IGitHubRepository, ILabel, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface ListLabelsInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 */
export interface ListLabelsInput {
    owner: string;
    repo: string;
}

/**
 * @class ListLabelsUseCase
 * @implements {IUseCase<ListLabelsInput, ILabel[]>}
 * @description Lists all labels in a repository
 */
export class ListLabelsUseCase implements IUseCase<ListLabelsInput, ILabel[]> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {ListLabelsInput} input - List parameters
     * @returns {Promise<ISkillResult<ILabel[]>>} List of labels or error
     */
    async execute(input: ListLabelsInput): Promise<ISkillResult<ILabel[]>> {
        try {
            const result = await this.repository.listLabels(input.owner, input.repo);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
