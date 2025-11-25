import { GitHubAdapter } from "../src/infrastructure/github-adapter";
import { ListWorkflowRunsUseCase, ListWorkflowRunsInput } from "../src/application/list-workflow-runs";

/**
 * @description Entry point for listing workflow runs
 * @example npx ts-node list_workflow_runs.ts owner repo [workflowId]
 */
async function main() {
    const args = process.argv.slice(2);
    if (args.length < 2) {
        console.error("Usage: ts-node list_workflow_runs.ts <owner> <repo> [workflowId]");
        process.exit(1);
    }

    const [owner, repo, workflowId] = args;

    const adapter = new GitHubAdapter();
    const useCase = new ListWorkflowRunsUseCase(adapter);

    const input: ListWorkflowRunsInput = {
        owner,
        repo,
        workflowId,
    };

    const result = await useCase.execute(input);
    console.log(JSON.stringify(result, null, 2));
    process.exit(result.success ? 0 : 1);
}

main().catch((error) => {
    console.error(JSON.stringify({ success: false, error: error.message }, null, 2));
    process.exit(1);
});
