import { IDatabase, INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for updating a database.
 * @interface UpdateDatabaseInput
 * @property {string} databaseId - The UUID of the database.
 * @property {string} [title] - The new title for the database.
 * @property {Record<string, any>} [properties] - The schema updates.
 */
export interface UpdateDatabaseInput {
    databaseId: string;
    title?: string;
    properties?: Record<string, any>;
}

/**
 * Use case for updating a database.
 * @class UpdateDatabaseUseCase
 * @implements {IUseCase<UpdateDatabaseInput, IDatabase>}
 */
export class UpdateDatabaseUseCase implements IUseCase<UpdateDatabaseInput, IDatabase> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the update database use case.
     * @param {UpdateDatabaseInput} input - The input data.
     * @returns {Promise<ISkillResult<IDatabase>>} The updated database or error.
     */
    async execute(input: UpdateDatabaseInput): Promise<ISkillResult<IDatabase>> {
        try {
            const result = await this.repository.updateDatabase(input.databaseId, {
                title: input.title,
                properties: input.properties,
            });
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
