import { INotionRepository, IPage, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for creating a page.
 * @interface CreatePageInput
 * @property {string} parentId - The UUID of the parent (page or database).
 * @property {"page" | "database"} parentType - The type of the parent object.
 * @property {string} title - The title of the new page.
 * @property {string} [content] - Optional initial text content (paragraph).
 */
export interface CreatePageInput {
    parentId: string;
    parentType: "page" | "database";
    title: string;
    content?: string;
}

/**
 * Use case for creating a new page in Notion.
 * @class CreatePageUseCase
 * @implements {IUseCase<CreatePageInput, IPage>}
 */
export class CreatePageUseCase implements IUseCase<CreatePageInput, IPage> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the create page use case.
     * @param {CreatePageInput} input - The input data.
     * @returns {Promise<ISkillResult<IPage>>} The created page or error.
     */
    async execute(input: CreatePageInput): Promise<ISkillResult<IPage>> {
        try {
            const parent = input.parentType === "database" ? { database_id: input.parentId } : { page_id: input.parentId };

            const result = await this.repository.createPage(parent, input.title, input.content);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
