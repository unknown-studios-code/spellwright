import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { CreatePRReviewUseCase, CreatePRReviewInput } from "../src/application/create-pr-review";

/**
 * @description Entry point for creating a review on a pull request
 * @example npx ts-node create_pr_review.ts owner repo prNumber APPROVE ["Optional comment"]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 4) {
        console.error("Usage: ts-node create_pr_review.ts <owner> <repo> <prNumber> <event> [body]");
        console.error("Events: APPROVE, REQUEST_CHANGES, COMMENT");
        process.exit(1);
    }

    const [owner, repo, prNumberStr, event, body] = args;
    const prNumber = parseInt(prNumberStr, 10);

    if (isNaN(prNumber)) {
        console.error("Error: prNumber must be a valid number");
        process.exit(1);
    }

    if (!["APPROVE", "REQUEST_CHANGES", "COMMENT"].includes(event)) {
        console.error("Error: event must be APPROVE, REQUEST_CHANGES, or COMMENT");
        process.exit(1);
    }

    const adapter = new GitHubAdapter();
    const useCase = new CreatePRReviewUseCase(adapter);

    const input: CreatePRReviewInput = {
        owner,
        repo,
        prNumber,
        event: event as "APPROVE" | "REQUEST_CHANGES" | "COMMENT",
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
