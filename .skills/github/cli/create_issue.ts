import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { CreateIssueUseCase, CreateIssueInput } from "../src/application/create-issue";

/**
 * @description Entry point for creating a GitHub issue
 * @example npx ts-node create_issue.ts owner repo "Issue title" "Optional body"
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 3) {
        console.error("Usage: ts-node create_issue.ts <owner> <repo> <title> [body]");
        process.exit(1);
    }

    const [owner, repo, title, body] = args;

    const adapter = new GitHubAdapter();
    const useCase = new CreateIssueUseCase(adapter);

    const input: CreateIssueInput = {
        owner,
        repo,
        title,
        body,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
