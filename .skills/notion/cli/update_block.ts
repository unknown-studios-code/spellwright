/**
 * @fileoverview Skill to update a Notion block's content.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { UpdateBlockUseCase, UpdateBlockInput } from "../src/application/update-block";

/**
 * Main entry point.
 * Usage: ts-node notion/update_block.ts <blockId> <content>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/update_block.ts <blockId> <content>" }, null, 2));
        process.exit(1);
    }

    const [blockId, content] = args;

    const adapter = new NotionAdapter();
    const useCase = new UpdateBlockUseCase(adapter);

    const input: UpdateBlockInput = {
        blockId,
        content,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
