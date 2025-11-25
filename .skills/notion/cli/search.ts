/**
 * @fileoverview Skill to search for pages and databases in Notion.
 */
import { NotionAdapter } from "../src/infrastructure/notion-adapter";
import { SearchUseCase, SearchInput } from "../src/application/search";

/**
 * Main entry point.
 * Usage: ts-node notion/search.ts "query string" [sort]
 * Sort options: "last_edited_time", "relevance"
 */
async function main() {
    const args = process.argv.slice(2);
    const query = args[0] || undefined;
    const sortArg = args[1];

    let sort: "last_edited_time" | "relevance" | undefined;
    if (sortArg === "last_edited_time" || sortArg === "relevance") {
        sort = sortArg;
    }

    const adapter = new NotionAdapter();
    const useCase = new SearchUseCase(adapter);

    const input: SearchInput = {
        query,
        sort,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
