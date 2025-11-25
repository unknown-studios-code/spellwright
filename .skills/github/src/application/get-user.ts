import { IGitHubRepository, ISkillResult, IUseCase, IUser } from "../domain/interfaces";

/**
 * @interface GetUserInput
 * @property {string} username - GitHub username
 */
export interface GetUserInput {
    username: string;
}

/**
 * @class GetUserUseCase
 * @implements {IUseCase<GetUserInput, IUser>}
 * @description Retrieves user information by username
 */
export class GetUserUseCase implements IUseCase<GetUserInput, IUser> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {GetUserInput} input - User retrieval parameters
     * @returns {Promise<ISkillResult<IUser>>} User information or error
     */
    async execute(input: GetUserInput): Promise<ISkillResult<IUser>> {
        try {
            const result = await this.repository.getUser(input.username);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
