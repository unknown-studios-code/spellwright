import { IComment, INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for listing comments.
 * @interface ListCommentsInput
 * @property {string} blockId - The UUID of the page or block.
 */
export interface ListCommentsInput {
    blockId: string;
}

/**
 * Use case for listing comments on a page or block.
 * @class ListCommentsUseCase
 * @implements {IUseCase<ListCommentsInput, IComment[]>}
 */
export class ListCommentsUseCase implements IUseCase<ListCommentsInput, IComment[]> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the list comments use case.
     * @param {ListCommentsInput} input - The input data.
     * @returns {Promise<ISkillResult<IComment[]>>} The list of comments or error.
     */
    async execute(input: ListCommentsInput): Promise<ISkillResult<IComment[]>> {
        try {
            const result = await this.repository.listComments(input.blockId);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
