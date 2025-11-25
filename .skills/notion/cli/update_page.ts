/**
 * @fileoverview Skill to update page properties or status.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { UpdatePageUseCase, UpdatePageInput } from "../src/application/update-page";

/**
 * Main entry point.
 * Usage: ts-node notion/update_page.ts <pageId> [key=value...] [archived=true|false]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/update_page.ts <pageId> [key=value...] [archived=true|false]" }, null, 2));
        process.exit(1);
    }

    const [pageId, ...updates] = args;
    const properties: any = {};
    let archived: boolean | undefined = undefined;

    updates.forEach((update) => {
        const [key, ...valueParts] = update.split("=");
        const value = valueParts.join("=");
        if (key === "archived") {
            archived = value === "true";
        } else if (key && value) {
            try {
                properties[key] = JSON.parse(value);
            } catch {
                properties[key] = value;
            }
        }
    });

    const adapter = new NotionAdapter();
    const useCase = new UpdatePageUseCase(adapter);

    const input: UpdatePageInput = {
        pageId,
        properties: Object.keys(properties).length > 0 ? properties : undefined,
        archived,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
