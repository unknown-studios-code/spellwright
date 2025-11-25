import { GetCurrentUserInput, GetCurrentUserUseCase } from "../src/application/get-current-user";
import { GitHubAdapter } from "../src/infrastructure/github-adapter";

/**
 * @description Entry point for getting the current authenticated user
 * @example npx ts-node get_current_user.ts
 */
async function main() {
    const adapter = new GitHubAdapter();
    const useCase = new GetCurrentUserUseCase(adapter);

    const input: GetCurrentUserInput = {};

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
