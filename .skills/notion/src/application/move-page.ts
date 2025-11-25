import { INotionRepository, IPage, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for moving a page.
 * @interface MovePageInput
 * @property {string} pageId - The UUID of the page to move.
 * @property {string} newParentId - The UUID of the new parent page/database.
 */
export interface MovePageInput {
    pageId: string;
    newParentId: string;
}

/**
 * Use case for moving a page to a new parent (copy & archive strategy).
 * @class MovePageUseCase
 * @implements {IUseCase<MovePageInput, IPage>}
 */
export class MovePageUseCase implements IUseCase<MovePageInput, IPage> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the move page use case.
     * @param {MovePageInput} input - The input data.
     * @returns {Promise<ISkillResult<IPage>>} The moved page or error.
     */
    async execute(input: MovePageInput): Promise<ISkillResult<IPage>> {
        try {
            const result = await this.repository.movePage(input.pageId, input.newParentId);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
