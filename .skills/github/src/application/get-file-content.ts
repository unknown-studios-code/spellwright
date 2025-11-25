import { IGitHubRepository, IUseCase, ISkillResult, IFileContent } from "../domain/interfaces";

/**
 * @interface GetFileContentInput
 * @property {string} owner - Repository owner
 * @property {string} repo - Repository name
 * @property {string} path - File path
 * @property {string} [ref] - Git reference (branch, tag, commit)
 */
export interface GetFileContentInput {
    owner: string;
    repo: string;
    path: string;
    ref?: string;
}

/**
 * @class GetFileContentUseCase
 * @implements {IUseCase<GetFileContentInput, IFileContent>}
 * @description Retrieves file content from a repository
 */
export class GetFileContentUseCase implements IUseCase<GetFileContentInput, IFileContent> {
    /**
     * @param {IGitHubRepository} repository - GitHub repository adapter
     */
    constructor(private repository: IGitHubRepository) {}

    /**
     * @param {GetFileContentInput} input - File retrieval parameters
     * @returns {Promise<ISkillResult<IFileContent>>} File content or error
     */
    async execute(input: GetFileContentInput): Promise<ISkillResult<IFileContent>> {
        try {
            const result = await this.repository.getFileContent(input.owner, input.repo, input.path, input.ref);
            return { success: true, data: result };
        } catch (error: any) {
            return { success: false, error: error.message };
        }
    }
}
