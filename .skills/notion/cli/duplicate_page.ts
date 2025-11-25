/**
 * @fileoverview Skill to duplicate a page.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { DuplicatePageUseCase, DuplicatePageInput } from "../src/application/duplicate-page";

/**
 * Main entry point.
 * Usage: ts-node notion/duplicate_page.ts <pageId>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 1) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/duplicate_page.ts <pageId>" }, null, 2));
        process.exit(1);
    }

    const [pageId] = args;

    const adapter = new NotionAdapter();
    const useCase = new DuplicatePageUseCase(adapter);

    const input: DuplicatePageInput = {
        pageId,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
