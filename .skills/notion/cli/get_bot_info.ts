/**
 * @fileoverview Skill to retrieve information about the bot and workspace.
 */
import { GetBotInfoInput, GetBotInfoUseCase } from "../src/application/get-bot-info";
import { NotionAdapter } from "../src/infrastructure/notion-adapter";

/**
 * Main entry point.
 */
async function main() {
    const adapter = new NotionAdapter();
    const useCase = new GetBotInfoUseCase(adapter);

    const input: GetBotInfoInput = {};

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
