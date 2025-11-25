/**
 * @fileoverview Skill to update a database.
 */
import { UpdateDatabaseInput, UpdateDatabaseUseCase } from "../src/application/update-database";
import { NotionAdapter } from "../src/infrastructure/notion-adapter";

/**
 * Main entry point.
 * Usage: ts-node notion/update_database.ts <databaseId> [title=Value] [properties=JSON]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/update_database.ts <databaseId> [title=Value] [properties=JSON]" }, null, 2));
        process.exit(1);
    }

    const [databaseId, ...updates] = args;
    let title: string | undefined;
    let properties: any | undefined;

    updates.forEach((update) => {
        const [key, ...valueParts] = update.split("=");
        const value = valueParts.join("=");
        if (key === "title") {
            title = value;
        } else if (key === "properties") {
            try {
                properties = JSON.parse(value);
            } catch {
                console.warn("Warning: properties value is not valid JSON, ignoring");
            }
        }
    });

    const adapter = new NotionAdapter();
    const useCase = new UpdateDatabaseUseCase(adapter);

    const input: UpdateDatabaseInput = {
        databaseId,
        title,
        properties,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
