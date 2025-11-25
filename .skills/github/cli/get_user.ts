import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { GetUserUseCase, GetUserInput } from "../src/application/get-user";

/**
 * @description Entry point for getting user information by username
 * @example npx ts-node get_user.ts username
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 1) {
        console.error("Usage: ts-node get_user.ts <username>");
        process.exit(1);
    }

    const [username] = args;

    const adapter = new GitHubAdapter();
    const useCase = new GetUserUseCase(adapter);

    const input: GetUserInput = {
        username,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
