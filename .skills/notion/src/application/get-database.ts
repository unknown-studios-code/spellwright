import { IDatabase } from "../domain/entities";
import { INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for retrieving a database.
 * @interface GetDatabaseInput
 * @property {string} databaseId - The UUID of the database to retrieve.
 */
export interface GetDatabaseInput {
    databaseId: string;
}

/**
 * Use Case to retrieve a Notion database by ID.
 * @class GetDatabaseUseCase
 * @implements {IUseCase<GetDatabaseInput, IDatabase>}
 */
export class GetDatabaseUseCase implements IUseCase<GetDatabaseInput, IDatabase> {
    /**
     * Creates an instance of GetDatabaseUseCase.
     * @param {INotionRepository} repository - The Notion repository.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the use case to retrieve a database.
     * @param {GetDatabaseInput} input - The input parameters.
     * @returns {Promise<ISkillResult<IDatabase>>} The result containing the database details.
     */
    async execute(input: GetDatabaseInput): Promise<ISkillResult<IDatabase>> {
        try {
            const database = await this.repository.getDatabase(input.databaseId);
            return { success: true, data: database };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
