/**
 * @fileoverview Skill to retrieve a Notion page by its task ID property.
 */
import { GetPageByTaskIdInput, GetPageByTaskIdUseCase } from "../src/application/get-page-by-task-id";
import { NotionAdapter } from "../src/infrastructure/notion-adapter";

/**
 * Main entry point.
 * Usage: ts-node notion/get_page_by_task_id.ts <taskId>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 1) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/get_page_by_task_id.ts <taskId>" }, null, 2));
        process.exit(1);
    }

    const [taskId] = args;

    const adapter = new NotionAdapter();
    const useCase = new GetPageByTaskIdUseCase(adapter);

    const input: GetPageByTaskIdInput = {
        taskId,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
