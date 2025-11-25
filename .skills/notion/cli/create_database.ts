/**
 * @fileoverview Skill to create a new database.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { CreateDatabaseUseCase, CreateDatabaseInput } from "../src/application/create-database";

/**
 * Main entry point.
 * Usage: ts-node notion/create_database.ts <parentId> <title> <propertiesJSON>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 3) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/create_database.ts <parentId> <title> <propertiesJSON>" }, null, 2));
        process.exit(1);
    }

    const [parentId, title, propertiesJson] = args;
    let properties: any;
    try {
        properties = JSON.parse(propertiesJson);
    } catch (e) {
        console.error(JSON.stringify({ success: false, error: "Error: properties must be valid JSON" }, null, 2));
        process.exit(1);
    }

    const adapter = new NotionAdapter();
    const useCase = new CreateDatabaseUseCase(adapter);

    const input: CreateDatabaseInput = {
        parentId,
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
