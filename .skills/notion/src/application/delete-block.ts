import { INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for deleting a block.
 * @interface DeleteBlockInput
 * @property {string} blockId - The UUID of the block to delete.
 */
export interface DeleteBlockInput {
    blockId: string;
}

/**
 * Use Case to delete a Notion block.
 * @class DeleteBlockUseCase
 * @implements {IUseCase<DeleteBlockInput, void>}
 */
export class DeleteBlockUseCase implements IUseCase<DeleteBlockInput, void> {
    /**
     * Creates an instance of DeleteBlockUseCase.
     * @param {INotionRepository} repository - The Notion repository.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the use case to delete a block.
     * @param {DeleteBlockInput} input - The input parameters.
     * @returns {Promise<ISkillResult<void>>} The result of the operation.
     */
    async execute(input: DeleteBlockInput): Promise<ISkillResult<void>> {
        try {
            await this.repository.deleteBlock(input.blockId);
            return { success: true };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
