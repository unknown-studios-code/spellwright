import { INotionRepository, IPage, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for duplicating a page.
 * @interface DuplicatePageInput
 * @property {string} pageId - The UUID of the page to duplicate.
 */
export interface DuplicatePageInput {
    pageId: string;
}

/**
 * Use case for duplicating a Notion page (best effort).
 * @class DuplicatePageUseCase
 * @implements {IUseCase<DuplicatePageInput, IPage>}
 */
export class DuplicatePageUseCase implements IUseCase<DuplicatePageInput, IPage> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the duplicate page use case.
     * @param {DuplicatePageInput} input - The input data.
     * @returns {Promise<ISkillResult<IPage>>} The duplicated page or error.
     */
    async execute(input: DuplicatePageInput): Promise<ISkillResult<IPage>> {
        try {
            const result = await this.repository.duplicatePage(input.pageId);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
