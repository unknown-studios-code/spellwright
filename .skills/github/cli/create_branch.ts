import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { CreateBranchUseCase, CreateBranchInput } from "../src/application/create-branch";

/**
 * @description Entry point for creating a new branch
 * @example npx ts-node create_branch.ts owner repo branchName sourceSha
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 4) {
        console.error("Usage: ts-node create_branch.ts <owner> <repo> <branchName> <sourceSha>");
        process.exit(1);
    }

    const [owner, repo, branchName, sourceSha] = args;

    const adapter = new GitHubAdapter();
    const useCase = new CreateBranchUseCase(adapter);

    const input: CreateBranchInput = {
        owner,
        repo,
        branchName,
        sourceSha,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
