import { IBlock } from "../domain/entities";
import { INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for appending block children.
 * @interface AppendBlockChildrenInput
 * @property {string} blockId - The UUID of the parent block.
 * @property {Array<{ type: string; content: string }>} children - The list of children to append.
 */
export interface AppendBlockChildrenInput {
    blockId: string;
    children: { type: string; content: string }[];
}

/**
 * Use Case to append children blocks to a parent block.
 * @class AppendBlockChildrenUseCase
 * @implements {IUseCase<AppendBlockChildrenInput, IBlock[]>}
 */
export class AppendBlockChildrenUseCase implements IUseCase<AppendBlockChildrenInput, IBlock[]> {
    /**
     * Creates an instance of AppendBlockChildrenUseCase.
     * @param {INotionRepository} repository - The Notion repository.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the use case to append block children.
     * @param {AppendBlockChildrenInput} input - The input parameters.
     * @returns {Promise<ISkillResult<IBlock[]>>} The result containing the appended blocks.
     */
    async execute(input: AppendBlockChildrenInput): Promise<ISkillResult<IBlock[]>> {
        try {
            const blocks = await this.repository.appendBlockChildren(input.blockId, input.children);
            return { success: true, data: blocks };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
