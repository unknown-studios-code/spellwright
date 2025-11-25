import { IGitHubRepository, IWorkflowRun, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * @interface ListWorkflowRunsInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {string} [workflowId] - Specific workflow ID or filename
 */
export interface ListWorkflowRunsInput {
    owner: string;
    repo: string;
    workflowId?: string;
}

/**
 * @class ListWorkflowRunsUseCase
 * @implements {IUseCase<ListWorkflowRunsInput, IWorkflowRun[]>}
 * @description Lists workflow runs in a repository
 */
export class ListWorkflowRunsUseCase implements IUseCase<ListWorkflowRunsInput, IWorkflowRun[]> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {ListWorkflowRunsInput} input - List parameters
     * @returns {Promise<ISkillResult<IWorkflowRun[]>>} List of workflow runs or error
     */
    async execute(input: ListWorkflowRunsInput): Promise<ISkillResult<IWorkflowRun[]>> {
        try {
            const result = await this.repository.listWorkflowRuns(input.owner, input.repo, input.workflowId);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
