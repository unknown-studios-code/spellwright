import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { StarRepositoryUseCase, StarRepositoryInput } from "../src/application/star-repository";

/**
 * @description Entry point for starring a repository
 * @example npx ts-node star_repository.ts owner repo
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error("Usage: ts-node star_repository.ts <owner> <repo>");
        process.exit(1);
    }

    const [owner, repo] = args;

    const adapter = new GitHubAdapter();
    const useCase = new StarRepositoryUseCase(adapter);

    const input: StarRepositoryInput = {
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
