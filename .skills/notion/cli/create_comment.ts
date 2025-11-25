/**
 * @fileoverview Skill to create a comment on a page or block.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { CreateCommentUseCase, CreateCommentInput } from "../src/application/create-comment";

/**
 * Main entry point.
 * Usage: ts-node notion/create_comment.ts <pageId> <content>
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error(JSON.stringify({ success: false, error: "Usage: ts-node notion/create_comment.ts <pageId> <content>" }, null, 2));
        process.exit(1);
    }

    const [pageId, content] = args;

    const adapter = new NotionAdapter();
    const useCase = new CreateCommentUseCase(adapter);

    const input: CreateCommentInput = {
        pageId,
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
