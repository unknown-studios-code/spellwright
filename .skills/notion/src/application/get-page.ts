import { IPage } from "../domain/entities";
import { INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for retrieving a page.
 * @interface GetPageInput
 * @property {string} pageId - The UUID of the page to retrieve.
 */
export interface GetPageInput {
    pageId: string;
}

/**
 * Use Case to retrieve a Notion page by ID.
 * @class GetPageUseCase
 * @implements {IUseCase<GetPageInput, IPage>}
 */
export class GetPageUseCase implements IUseCase<GetPageInput, IPage> {
    /**
     * Creates an instance of GetPageUseCase.
     * @param {INotionRepository} repository - The Notion repository.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the use case to retrieve a page.
     * @param {GetPageInput} input - The input parameters.
     * @returns {Promise<ISkillResult<IPage>>} The result containing the page details.
     */
    async execute(input: GetPageInput): Promise<ISkillResult<IPage>> {
        try {
            const page = await this.repository.getPage(input.pageId);
            return { success: true, data: page };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
