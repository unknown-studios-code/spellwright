import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { ListCommitsUseCase, ListCommitsInput } from "../src/application/list-commits";

/**
 * @description Entry point for listing commits in a repository
 * @example npx ts-node list_commits.ts owner repo [branch] [limit]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error("Usage: ts-node list_commits.ts <owner> <repo> [branch] [limit]");
        process.exit(1);
    }

    const [owner, repo, branch, limitStr] = args;
    const limit = limitStr ? parseInt(limitStr, 10) : undefined;

    const adapter = new GitHubAdapter();
    const useCase = new ListCommitsUseCase(adapter);

    const input: ListCommitsInput = {
        owner,
        repo,
        branch,
        limit,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
