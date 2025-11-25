import { INotionRepository, IPage, ISkillResult, IUseCase } from "../domain/interfaces";

export interface GetPageByTaskIdInput {
    taskId: string;
}

export class GetPageByTaskIdUseCase implements IUseCase<GetPageByTaskIdInput, IPage | null> {
    constructor(private repository: INotionRepository) {}

    async execute(input: GetPageByTaskIdInput): Promise<ISkillResult<IPage | null>> {
        try {
            const result = await this.repository.getPageByTaskId(input.taskId);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
