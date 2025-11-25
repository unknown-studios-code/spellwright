/**
 * @fileoverview Skill to list all users in the workspace.
 */
import { ListUsersInput, ListUsersUseCase } from "../src/application/list-users";
import { NotionAdapter } from "../src/infrastructure/notion-adapter";

/**
 * Main entry point.
 */
async function main() {
    const adapter = new NotionAdapter();
    const useCase = new ListUsersUseCase(adapter);

    const input: ListUsersInput = {};

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
