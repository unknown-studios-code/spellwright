import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { GetFileContentUseCase, GetFileContentInput } from "../src/application/get-file-content";

/**
 * @description Entry point for retrieving file content from a repository
 * @example npx ts-node get_file_content.ts owner repo path/to/file [ref]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 3) {
        console.error("Usage: ts-node get_file_content.ts <owner> <repo> <path> [ref]");
        process.exit(1);
    }

    const [owner, repo, path, ref] = args;

    const adapter = new GitHubAdapter();
    const useCase = new GetFileContentUseCase(adapter);

    const input: GetFileContentInput = {
        owner,
        repo,
        path,
        ref,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
