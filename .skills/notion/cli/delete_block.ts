/**
 * @fileoverview Skill to delete a Notion block.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { DeleteBlockUseCase, DeleteBlockInput } from "../src/application/delete-block";

/**
 * Main entry point.
 * Usage: ts-node notion/delete_block.ts <blockId>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 1) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/delete_block.ts <blockId>" }, null, 2));
        process.exit(1);
    }

    const [blockId] = args;

    const adapter = new NotionAdapter();
    const useCase = new DeleteBlockUseCase(adapter);

    const input: DeleteBlockInput = {
        blockId,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
