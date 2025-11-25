import { INotionRepository, IPage, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for updating a page.
 * @interface UpdatePageInput
 * @property {string} pageId - The UUID of the page.
 * @property {Record<string, any>} [properties] - The properties to update.
 * @property {boolean} [archived] - Whether to archive or unarchive the page.
 */
export interface UpdatePageInput {
    pageId: string;
    properties?: Record<string, any>;
    archived?: boolean;
}

/**
 * Use case for updating a page properties or status.
 * @class UpdatePageUseCase
 * @implements {IUseCase<UpdatePageInput, IPage>}
 */
export class UpdatePageUseCase implements IUseCase<UpdatePageInput, IPage> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the update page use case.
     * @param {UpdatePageInput} input - The input data.
     * @returns {Promise<ISkillResult<IPage>>} The updated page or error.
     */
    async execute(input: UpdatePageInput): Promise<ISkillResult<IPage>> {
        try {
            const result = await this.repository.updatePage(input.pageId, input.properties, input.archived);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
