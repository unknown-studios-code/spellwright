/**
 * @fileoverview Skill to retrieve a specific user by ID.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { GetUserUseCase, GetUserInput } from "../src/application/get-user";

/**
 * Main entry point.
 * Usage: ts-node notion/get_user.ts <userId>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 1) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/get_user.ts <userId>" }, null, 2));
        process.exit(1);
    }

    const [userId] = args;

    const adapter = new NotionAdapter();
    const useCase = new GetUserUseCase(adapter);

    const input: GetUserInput = {
        userId,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
