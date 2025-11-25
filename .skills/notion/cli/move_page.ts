/**
 * @fileoverview Skill to move a page to a new parent (Copy & Archive).
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { MovePageUseCase, MovePageInput } from "../src/application/move-page";

/**
 * Main entry point.
 * Usage: ts-node notion/move_page.ts <pageId> <newParentId>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/move_page.ts <pageId> <newParentId>" }, null, 2));
        process.exit(1);
    }

    const [pageId, newParentId] = args;

    const adapter = new NotionAdapter();
    const useCase = new MovePageUseCase(adapter);

    const input: MovePageInput = {
        pageId,
        newParentId,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
