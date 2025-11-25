import { INotionRepository, ISkillResult, IUseCase, IUser } from "../domain/interfaces";

/**
 * Input parameters for getting bot info.
 * @interface GetBotInfoInput
 */
export interface GetBotInfoInput {}

/**
 * Use case for retrieving information about the bot and workspace.
 * @class GetBotInfoUseCase
 * @implements {IUseCase<GetBotInfoInput, { bot: IUser; workspace_name?: string }>}
 */
export class GetBotInfoUseCase implements IUseCase<GetBotInfoInput, { bot: IUser; workspace_name?: string }> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the get bot info use case.
     * @param {GetBotInfoInput} input - The input data.
     * @returns {Promise<ISkillResult<{ bot: IUser; workspace_name?: string }>>} The bot info or error.
     */
    async execute(input: GetBotInfoInput): Promise<ISkillResult<{ bot: IUser; workspace_name?: string }>> {
        try {
            const result = await this.repository.getBotInfo();
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
