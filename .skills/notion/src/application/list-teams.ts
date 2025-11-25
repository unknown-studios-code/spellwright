import { INotionRepository, ISkillResult, ITeamspace, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for listing teamspaces.
 * @interface ListTeamspacesInput
 */
export interface ListTeamspacesInput {}

/**
 * Use case for listing teamspaces.
 * Note: Currently returns empty list as API support is limited.
 * @class ListTeamspacesUseCase
 * @implements {IUseCase<ListTeamspacesInput, ITeamspace[]>}
 */
export class ListTeamspacesUseCase implements IUseCase<ListTeamspacesInput, ITeamspace[]> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the list teamspaces use case.
     * @param {ListTeamspacesInput} input - The input data.
     * @returns {Promise<ISkillResult<ITeamspace[]>>} The list of teamspaces or error.
     */
    async execute(input: ListTeamspacesInput): Promise<ISkillResult<ITeamspace[]>> {
        try {
            const result = await this.repository.listTeamspaces();
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
