import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { RemoveLabelUseCase, RemoveLabelInput } from "../src/application/remove-label";

/**
 * @description Entry point for removing a label from an issue or pull request
 * @example npx ts-node remove_label.ts owner repo issueNumber labelName
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 4) {
        console.error("Usage: ts-node remove_label.ts <owner> <repo> <issueNumber> <labelName>");
        process.exit(1);
    }

    const [owner, repo, issueNumberStr, labelName] = args;
    const issueNumber = parseInt(issueNumberStr, 10);

    if (isNaN(issueNumber)) {
        console.error("Error: issueNumber must be a valid number");
        process.exit(1);
    }

    const adapter = new GitHubAdapter();
    const useCase = new RemoveLabelUseCase(adapter);

    const input: RemoveLabelInput = {
        owner,
        repo,
        issueNumber,
        labelName,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
