import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { ListNotificationsUseCase, ListNotificationsInput } from "../src/application/list-notifications";

/**
 * @description Entry point for listing notifications
 * @example npx ts-node list_notifications.ts [--all] [--participating]
 */
async function main() {
    const args = process.argv.slice(2);
    const all = args.includes("--all");
    const participating = args.includes("--participating");

    const adapter = new GitHubAdapter();
    const useCase = new ListNotificationsUseCase(adapter);

    const input: ListNotificationsInput = {
        all,
        participating,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
