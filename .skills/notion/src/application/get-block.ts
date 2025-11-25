import { IBlock } from "../domain/entities";
import { INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for retrieving a block.
 * @interface GetBlockInput
 * @property {string} blockId - The UUID of the block to retrieve.
 */
export interface GetBlockInput {
    blockId: string;
}

/**
 * Use Case to retrieve a Notion block by ID.
 * @class GetBlockUseCase
 * @implements {IUseCase<GetBlockInput, IBlock>}
 */
export class GetBlockUseCase implements IUseCase<GetBlockInput, IBlock> {
    /**
     * Creates an instance of GetBlockUseCase.
     * @param {INotionRepository} repository - The Notion repository.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the use case to retrieve a block.
     * @param {GetBlockInput} input - The input parameters.
     * @returns {Promise<ISkillResult<IBlock>>} The result containing the block details.
     */
    async execute(input: GetBlockInput): Promise<ISkillResult<IBlock>> {
        try {
            const block = await this.repository.getBlock(input.blockId);
            return { success: true, data: block };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
