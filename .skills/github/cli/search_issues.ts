import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { SearchIssuesUseCase, SearchIssuesInput } from "../src/application/search-issues";

/**
 * @description Entry point for searching issues and pull requests
 * @example npx ts-node search_issues.ts "query string"
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 1) {
        console.error("Usage: ts-node search_issues.ts <query>");
        process.exit(1);
    }

    const query = args.join(" ");

    const adapter = new GitHubAdapter();
    const useCase = new SearchIssuesUseCase(adapter);

    const input: SearchIssuesInput = {
        query,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
