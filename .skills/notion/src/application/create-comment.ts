import { IComment, INotionRepository, ISkillResult, IUseCase } from "../domain/interfaces";

/**
 * Input parameters for creating a comment.
 * @interface CreateCommentInput
 * @property {string} pageId - The UUID of the page/block to comment on.
 * @property {string} content - The text content of the comment.
 */
export interface CreateCommentInput {
    pageId: string;
    content: string;
}

/**
 * Use case for creating a comment on a Notion page.
 * @class CreateCommentUseCase
 * @implements {IUseCase<CreateCommentInput, IComment>}
 */
export class CreateCommentUseCase implements IUseCase<CreateCommentInput, IComment> {
    /**
     * @constructor
     * @param {INotionRepository} repository - The Notion repository instance.
     */
    constructor(private repository: INotionRepository) {}

    /**
     * Executes the create comment use case.
     * @param {CreateCommentInput} input - The input data.
     * @returns {Promise<ISkillResult<IComment>>} The created comment or error.
     */
    async execute(input: CreateCommentInput): Promise<ISkillResult<IComment>> {
        try {
            const result = await this.repository.createComment(input.pageId, input.content);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
