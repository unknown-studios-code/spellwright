/**
 * @fileoverview Skill to retrieve a Notion page by ID.
 */
import { GetPageInput, GetPageUseCase } from "../src/application/get-page";
import { NotionAdapter } from "../src/infrastructure/notion-adapter";

/**
 * Main entry point.
 * Usage: ts-node notion/get_page.ts <pageId>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 1) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/get_page.ts <pageId>" }, null, 2));
        process.exit(1);
    }

    const [pageId] = args;

    const adapter = new NotionAdapter();
    const useCase = new GetPageUseCase(adapter);

    const input: GetPageInput = {
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
