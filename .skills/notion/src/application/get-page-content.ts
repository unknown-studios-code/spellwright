import { IBlock, INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for getting page content (blocks).
 * @interface GetPageContentInput
 * @property {string} blockId - The UUID of the page or parent block.
 */
export interface GetPageContentInput {
    blockId: string;
}

/**
 * Use case for retrieving the content (children blocks) of a page or block.
 * @class GetPageContentUseCase
 * @implements {IUseCase<GetPageContentInput, IBlock[]>}
 */
export class GetPageContentUseCase implements IUseCase<GetPageContentInput, IBlock[]> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the get page content use case.
     * @param {GetPageContentInput} input - The input data.
     * @returns {Promise<ISkillResult<IBlock[]>>} The list of blocks or error.
     */
    async execute(input: GetPageContentInput): Promise<ISkillResult<IBlock[]>> {
        try {
            const result = await this.repository.getPageContent(input.blockId);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
