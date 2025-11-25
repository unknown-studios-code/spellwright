import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { UpdateIssueUseCase, UpdateIssueInput } from "../src/application/update-issue";

/**
 * @description Entry point for updating a GitHub issue
 * @example npx ts-node update_issue.ts owner repo issueNumber title="New Title" body="New Body" state=closed
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 3) {
        console.error("Usage: ts-node update_issue.ts <owner> <repo> <issueNumber> [key=value...]");
        process.exit(1);
    }

    const [owner, repo, issueNumberStr, ...updates] = args;
    const issueNumber = parseInt(issueNumberStr, 10);

    if (isNaN(issueNumber)) {
        console.error("Error: issueNumber must be a valid number");
        process.exit(1);
    }

    const updateFields: any = {};
    updates.forEach((update) => {
        const [key, ...valueParts] = update.split("=");
        const value = valueParts.join("=");
        if (key && value) {
            updateFields[key] = value;
        }
    });

    const adapter = new GitHubAdapter();
    const useCase = new UpdateIssueUseCase(adapter);

    const input: UpdateIssueInput = {
        owner,
        repo,
        issueNumber,
        updates: updateFields,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
