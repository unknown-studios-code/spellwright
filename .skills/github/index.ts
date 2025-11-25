import { GitHubAdapter } from "./src/infrastructure/github-adapter";
import { AddLabelsUseCase } from "./src/application/add-labels";
import { CreateBranchUseCase } from "./src/application/create-branch";
import { CreateIssueUseCase } from "./src/application/create-issue";
import { CreatePRReviewUseCase } from "./src/application/create-pr-review";
import { CreatePullRequestUseCase } from "./src/application/create-pull-request";
import { ForkRepositoryUseCase } from "./src/application/fork-repository";
import { GetCommitUseCase } from "./src/application/get-commit";
import { GetCurrentUserUseCase } from "./src/application/get-current-user";
import { GetFileContentUseCase } from "./src/application/get-file-content";
import { GetUserUseCase } from "./src/application/get-user";
import { ListBranchesUseCase } from "./src/application/list-branches";
import { ListCommitsUseCase } from "./src/application/list-commits";
import { ListIssuesUseCase } from "./src/application/list-issues";
import { ListLabelsUseCase } from "./src/application/list-labels";
import { ListNotificationsUseCase } from "./src/application/list-notifications";
import { ListPRReviewsUseCase } from "./src/application/list-pr-reviews";
import { ListPullRequestsUseCase } from "./src/application/list-pull-requests";
import { ListWorkflowRunsUseCase } from "./src/application/list-workflow-runs";
import { MergePullRequestUseCase } from "./src/application/merge-pull-request";
import { RemoveLabelUseCase } from "./src/application/remove-label";
import { SearchCodeUseCase } from "./src/application/search-code";
import { SearchIssuesUseCase } from "./src/application/search-issues";
import { StarRepositoryUseCase } from "./src/application/star-repository";
import { UpdateIssueUseCase } from "./src/application/update-issue";

const adapter = new GitHubAdapter();

export async function addLabels(owner: string, repo: string, issueNumber: number, labels: string[]) {
    const useCase = new AddLabelsUseCase(adapter);
    return await useCase.execute({ owner, repo, issueNumber, labels });
}

export async function createBranch(owner: string, repo: string, branchName: string, sourceSha: string) {
    const useCase = new CreateBranchUseCase(adapter);
    return await useCase.execute({ owner, repo, branchName, sourceSha });
}

export async function createIssue(owner: string, repo: string, title: string, body?: string) {
    const useCase = new CreateIssueUseCase(adapter);
    return await useCase.execute({ owner, repo, title, body });
}

export async function createPrReview(owner: string, repo: string, prNumber: number, event: "APPROVE" | "REQUEST_CHANGES" | "COMMENT", body?: string) {
    const useCase = new CreatePRReviewUseCase(adapter);
    return await useCase.execute({ owner, repo, prNumber, event, body });
}

export async function createPullRequest(owner: string, repo: string, title: string, head: string, base: string, body?: string) {
    const useCase = new CreatePullRequestUseCase(adapter);
    return await useCase.execute({ owner, repo, title, head, base, body });
}

export async function forkRepository(owner: string, repo: string) {
    const useCase = new ForkRepositoryUseCase(adapter);
    return await useCase.execute({ owner, repo });
}

export async function getCommit(owner: string, repo: string, sha: string) {
    const useCase = new GetCommitUseCase(adapter);
    return await useCase.execute({ owner, repo, sha });
}

export async function getCurrentUser() {
    const useCase = new GetCurrentUserUseCase(adapter);
    return await useCase.execute({});
}

export async function getFileContent(owner: string, repo: string, path: string, ref?: string) {
    const useCase = new GetFileContentUseCase(adapter);
    return await useCase.execute({ owner, repo, path, ref });
}

export async function getUser(username: string) {
    const useCase = new GetUserUseCase(adapter);
    return await useCase.execute({ username });
}

export async function listBranches(owner: string, repo: string) {
    const useCase = new ListBranchesUseCase(adapter);
    return await useCase.execute({ owner, repo });
}

export async function listCommits(owner: string, repo: string, branch?: string, limit: number = 30) {
    const useCase = new ListCommitsUseCase(adapter);
    return await useCase.execute({ owner, repo, branch, limit });
}

export async function listIssues(owner: string, repo: string, state: "open" | "closed" | "all" = "open") {
    const useCase = new ListIssuesUseCase(adapter);
    return await useCase.execute({ owner, repo, state });
}

export async function listLabels(owner: string, repo: string) {
    const useCase = new ListLabelsUseCase(adapter);
    return await useCase.execute({ owner, repo });
}

export async function listNotifications(all: boolean = false, participating: boolean = false) {
    const useCase = new ListNotificationsUseCase(adapter);
    return await useCase.execute({ all, participating });
}

export async function listPrReviews(owner: string, repo: string, prNumber: number) {
    const useCase = new ListPRReviewsUseCase(adapter);
    return await useCase.execute({ owner, repo, prNumber });
}

export async function listPullRequests(owner: string, repo: string, state: "open" | "closed" | "all" = "open") {
    const useCase = new ListPullRequestsUseCase(adapter);
    return await useCase.execute({ owner, repo, state });
}

export async function listWorkflowRuns(owner: string, repo: string, workflowId?: string) {
    const useCase = new ListWorkflowRunsUseCase(adapter);
    return await useCase.execute({ owner, repo, workflowId });
}

export async function mergePullRequest(owner: string, repo: string, prNumber: number, method: "merge" | "squash" | "rebase" = "merge") {
    const useCase = new MergePullRequestUseCase(adapter);
    return await useCase.execute({ owner, repo, prNumber, method });
}

export async function removeLabel(owner: string, repo: string, issueNumber: number, labelName: string) {
    const useCase = new RemoveLabelUseCase(adapter);
    return await useCase.execute({ owner, repo, issueNumber, labelName });
}

export async function searchCode(query: string) {
    const useCase = new SearchCodeUseCase(adapter);
    return await useCase.execute({ query });
}

export async function searchIssues(query: string) {
    const useCase = new SearchIssuesUseCase(adapter);
    return await useCase.execute({ query });
}

export async function starRepository(owner: string, repo: string) {
    const useCase = new StarRepositoryUseCase(adapter);
    return await useCase.execute({ owner, repo });
}

export async function updateIssue(owner: string, repo: string, issueNumber: number, updates: { title?: string; body?: string; state?: "open" | "closed" }) {
    const useCase = new UpdateIssueUseCase(adapter);
    return await useCase.execute({ owner, repo, issueNumber, updates });
}
