import { IGitHubRepository, ISkillResult, IUseCase, IUser } from "../domain/interfaces";

/**
 * @interface GetCurrentUserInput
 */
export interface GetCurrentUserInput {}

/**
 * @class GetCurrentUserUseCase
 * @implements {IUseCase<GetCurrentUserInput, IUser>}
 * @description Retrieves current authenticated user information
 */
export class GetCurrentUserUseCase implements IUseCase<GetCurrentUserInput, IUser> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {GetCurrentUserInput} input - Empty input object
     * @returns {Promise<ISkillResult<IUser>>} Current user information or error
     */
    async execute(input: GetCurrentUserInput): Promise<ISkillResult<IUser>> {
        try {
            const result = await this.repository.getCurrentUser();
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
