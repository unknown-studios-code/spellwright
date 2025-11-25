/**
 * @fileoverview Skill to append children blocks to a parent block.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { AppendBlockChildrenUseCase, AppendBlockChildrenInput } from "../src/application/append-block-children";

/**
 * Main entry point.
 * Usage: ts-node notion/append_block_children.ts <blockId> <childrenJson>
 * childrenJson example: '[{"type": "paragraph", "content": "Hello"}]'
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/append_block_children.ts <blockId> <childrenJson>" }, null, 2));
        process.exit(1);
    }

    const [blockId, childrenJson] = args;

    let children;
    try {
        children = JSON.parse(childrenJson);
    } catch (e) {
        console.error(JSON.stringify({ success: false, error: "Invalid JSON for children" }, null, 2));
        process.exit(1);
    }

    const adapter = new NotionAdapter();
    const useCase = new AppendBlockChildrenUseCase(adapter);

    const input: AppendBlockChildrenInput = {
        blockId,
        children,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
