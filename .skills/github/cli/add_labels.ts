import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { AddLabelsUseCase, AddLabelsInput } from "../src/application/add-labels";

/**
 * @description Entry point for adding labels to an issue or pull request
 * @example npx ts-node add_labels.ts owner repo issueNumber label1 label2 label3
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 4) {
        console.error("Usage: ts-node add_labels.ts <owner> <repo> <issueNumber> <label1> [label2] ...");
        process.exit(1);
    }

    const [owner, repo, issueNumberStr, ...labels] = args;
    const issueNumber = parseInt(issueNumberStr, 10);

    if (isNaN(issueNumber)) {
        console.error("Error: issueNumber must be a valid number");
        process.exit(1);
    }

    const adapter = new GitHubAdapter();
    const useCase = new AddLabelsUseCase(adapter);

    const input: AddLabelsInput = {
        owner,
        repo,
        issueNumber,
        labels,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
