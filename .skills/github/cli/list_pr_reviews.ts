import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { ListPRReviewsUseCase, ListPRReviewsInput } from "../src/application/list-pr-reviews";

/**
 * @description Entry point for listing reviews on a pull request
 * @example npx ts-node list_pr_reviews.ts owner repo prNumber
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 3) {
        console.error("Usage: ts-node list_pr_reviews.ts <owner> <repo> <prNumber>");
        process.exit(1);
    }

    const [owner, repo, prNumberStr] = args;
    const prNumber = parseInt(prNumberStr, 10);

    if (isNaN(prNumber)) {
        console.error("Error: prNumber must be a valid number");
        process.exit(1);
    }

    const adapter = new GitHubAdapter();
    const useCase = new ListPRReviewsUseCase(adapter);

    const input: ListPRReviewsInput = {
        owner,
        repo,
        prNumber,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
