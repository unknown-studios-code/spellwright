import { INotionRepository, ISkillResult, IUseCase, IUser } from "../domain/interfaces";

/**
 * Input parameters for getting a user.
 * @interface GetUserInput
 * @property {string} userId - The UUID of the user.
 */
export interface GetUserInput {
    userId: string;
}

/**
 * Use case for retrieving a specific user.
 * @class GetUserUseCase
 * @implements {IUseCase<GetUserInput, IUser>}
 */
export class GetUserUseCase implements IUseCase<GetUserInput, IUser> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the get user use case.
     * @param {GetUserInput} input - The input data.
     * @returns {Promise<ISkillResult<IUser>>} The user details or error.
     */
    async execute(input: GetUserInput): Promise<ISkillResult<IUser>> {
        try {
            const result = await this.repository.getUser(input.userId);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
