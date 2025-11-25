/**
 * @fileoverview Skill to retrieve a Notion database by ID.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { GetDatabaseUseCase, GetDatabaseInput } from "../src/application/get-database";

/**
 * Main entry point.
 * Usage: ts-node notion/get_database.ts <databaseId>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 1) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/get_database.ts <databaseId>" }, null, 2));
        process.exit(1);
    }

    const [databaseId] = args;

    const adapter = new NotionAdapter();
    const useCase = new GetDatabaseUseCase(adapter);

    const input: GetDatabaseInput = {
        databaseId,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
