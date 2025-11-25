/**
 * @fileoverview Skill to create a new page.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { CreatePageUseCase, CreatePageInput } from "../src/application/create-page";

/**
 * Main entry point.
 * Usage: ts-node notion/create_page.ts <parentId> <page|database> <title> [content]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 3) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/create_page.ts <parentId> <page|database> <title> [content]" }, null, 2));
        process.exit(1);
    }

    const [parentId, parentType, title, content] = args;

    if (parentType !== "page" && parentType !== "database") {
        console.error(JSON.stringify({ success: false, error: "Error: parentType must be 'page' or 'database'" }, null, 2));
        process.exit(1);
    }

    const adapter = new NotionAdapter();
    const useCase = new CreatePageUseCase(adapter);

    const input: CreatePageInput = {
        parentId,
        parentType: parentType as "page" | "database",
        title,
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
