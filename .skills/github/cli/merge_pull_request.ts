import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { MergePullRequestUseCase, MergePullRequestInput } from "../src/application/merge-pull-request";

/**
 * @description Entry point for merging a pull request
 * @example npx ts-node merge_pull_request.ts owner repo prNumber [merge|squash|rebase]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 3) {
        console.error("Usage: ts-node merge_pull_request.ts <owner> <repo> <prNumber> [method]");
        process.exit(1);
    }

    const [owner, repo, prNumberStr, method] = args;
    const prNumber = parseInt(prNumberStr, 10);

    if (isNaN(prNumber)) {
        console.error("Error: prNumber must be a valid number");
        process.exit(1);
    }

    const adapter = new GitHubAdapter();
    const useCase = new MergePullRequestUseCase(adapter);

    const input: MergePullRequestInput = {
        owner,
        repo,
        prNumber,
        method: method as "merge" | "squash" | "rebase" | undefined,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
