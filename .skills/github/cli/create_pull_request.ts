import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { CreatePullRequestUseCase, CreatePullRequestInput } from "../src/application/create-pull-request";

/**
 * @description Entry point for creating a pull request
 * @example npx ts-node create_pull_request.ts owner repo "PR Title" head base ["PR Body"]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 5) {
        console.error("Usage: ts-node create_pull_request.ts <owner> <repo> <title> <head> <base> [body]");
        process.exit(1);
    }

    const [owner, repo, title, head, base, body] = args;

    const adapter = new GitHubAdapter();
    const useCase = new CreatePullRequestUseCase(adapter);

    const input: CreatePullRequestInput = {
        owner,
        repo,
        title,
        head,
        base,
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
