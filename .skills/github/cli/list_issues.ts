import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { ListIssuesUseCase, ListIssuesInput } from "../src/application/list-issues";

/**
 * @description Entry point for listing issues in a repository
 * @example npx ts-node list_issues.ts owner repo [open|closed|all]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error("Usage: ts-node list_issues.ts <owner> <repo> [state]");
        process.exit(1);
    }

    const [owner, repo, state] = args;

    const adapter = new GitHubAdapter();
    const useCase = new ListIssuesUseCase(adapter);

    const input: ListIssuesInput = {
        owner,
        repo,
        state: state as "open" | "closed" | "all" | undefined,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
