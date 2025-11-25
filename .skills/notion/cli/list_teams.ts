/**
 * @fileoverview Skill to list teamspaces (if supported).
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { ListTeamspacesUseCase, ListTeamspacesInput } from "../src/application/list-teams";

/**
 * Main entry point.
 */
async function main() {
    const adapter = new NotionAdapter();
    const useCase = new ListTeamspacesUseCase(adapter);

    const input: ListTeamspacesInput = {};

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
