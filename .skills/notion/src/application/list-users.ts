import { INotionRepository, ISkillResult, IUseCase, IUser } from "../domain/interfaces";

/**
 * Input parameters for listing users.
 * @interface ListUsersInput
 */
export interface ListUsersInput {}

/**
 * Use case for listing all users in the workspace.
 * @class ListUsersUseCase
 * @implements {IUseCase<ListUsersInput, IUser[]>}
 */
export class ListUsersUseCase implements IUseCase<ListUsersInput, IUser[]> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the list users use case.
     * @param {ListUsersInput} input - The input data.
     * @returns {Promise<ISkillResult<IUser[]>>} The list of users or error.
     */
    async execute(input: ListUsersInput): Promise<ISkillResult<IUser[]>> {
        try {
            const result = await this.repository.listUsers();
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
