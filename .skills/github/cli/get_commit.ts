import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { GetCommitUseCase, GetCommitInput } from "../src/application/get-commit";

/**
 * @description Entry point for getting a specific commit
 * @example npx ts-node get_commit.ts owner repo sha
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 3) {
        console.error("Usage: ts-node get_commit.ts <owner> <repo> <sha>");
        process.exit(1);
    }

    const [owner, repo, sha] = args;

    const adapter = new GitHubAdapter();
    const useCase = new GetCommitUseCase(adapter);

    const input: GetCommitInput = {
        owner,
        repo,
        sha,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
