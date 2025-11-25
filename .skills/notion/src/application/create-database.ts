import { IDatabase, INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for creating a database.
 * @interface CreateDatabaseInput
 * @property {string} parentId - The UUID of the parent page.
 * @property {string} title - The title of the new database.
 * @property {Record<string, any>} properties - The schema properties for the database.
 */
export interface CreateDatabaseInput {
    parentId: string;
    title: string;
    properties: Record<string, any>;
}

/**
 * Use case for creating a new database in Notion.
 * @class CreateDatabaseUseCase
 * @implements {IUseCase<CreateDatabaseInput, IDatabase>}
 */
export class CreateDatabaseUseCase implements IUseCase<CreateDatabaseInput, IDatabase> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the create database use case.
     * @param {CreateDatabaseInput} input - The input data.
     * @returns {Promise<ISkillResult<IDatabase>>} The created database or error.
     */
    async execute(input: CreateDatabaseInput): Promise<ISkillResult<IDatabase>> {
        try {
            const result = await this.repository.createDatabase(input.parentId, input.title, input.properties);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
