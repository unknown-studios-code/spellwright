import { IGitHubRepository, INotification, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface ListNotificationsInput
 * @property {boolean} [all] - Include read notifications
 * @property {boolean} [participating] - Only show notifications where user is participating
 */
export interface ListNotificationsInput {
    all?: boolean;
    participating?: boolean;
}

/**
 * @class ListNotificationsUseCase
 * @implements {IUseCase<ListNotificationsInput, INotification[]>}
 * @description Lists notifications for the authenticated user
 */
export class ListNotificationsUseCase implements IUseCase<ListNotificationsInput, INotification[]> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {ListNotificationsInput} input - List parameters
     * @returns {Promise<ISkillResult<INotification[]>>} List of notifications or error
     */
    async execute(input: ListNotificationsInput): Promise<ISkillResult<INotification[]>> {
        try {
            const result = await this.repository.listNotifications(input.all, input.participating);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
