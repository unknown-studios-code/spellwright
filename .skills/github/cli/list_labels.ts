import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { ListLabelsUseCase, ListLabelsInput } from "../src/application/list-labels";

/**
 * @description Entry point for listing labels in a repository
 * @example npx ts-node list_labels.ts owner repo
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error("Usage: ts-node list_labels.ts <owner> <repo>");
        process.exit(1);
    }

    const [owner, repo] = args;

    const adapter = new GitHubAdapter();
    const useCase = new ListLabelsUseCase(adapter);

    const input: ListLabelsInput = {
        owner,
        repo,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
