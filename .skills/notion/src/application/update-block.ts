import { IBlock } from "../domain/entities";
import { INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for updating a block.
 * @interface UpdateBlockInput
 * @property {string} blockId - The UUID of the block to update.
 * @property {string} content - The new text content for the block.
 */
export interface UpdateBlockInput {
    blockId: string;
    content: string;
}

/**
 * Use Case to update a Notion block's content.
 * @class UpdateBlockUseCase
 * @implements {IUseCase<UpdateBlockInput, IBlock>}
 */
export class UpdateBlockUseCase implements IUseCase<UpdateBlockInput, IBlock> {
    /**
     * Creates an instance of UpdateBlockUseCase.
     * @param {INotionRepository} repository - The Notion repository.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the use case to update a block.
     * @param {UpdateBlockInput} input - The input parameters.
     * @returns {Promise<ISkillResult<IBlock>>} The result containing the updated block.
     */
    async execute(input: UpdateBlockInput): Promise<ISkillResult<IBlock>> {
        try {
            const block = await this.repository.updateBlock(input.blockId, input.content);
            return { success: true, data: block };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
