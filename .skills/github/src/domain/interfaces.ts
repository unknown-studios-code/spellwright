import { IBranch, ICommit, IFileContent, IIssue, ILabel, INotification, IPullRequest, IPullRequestReview, ISearchResult, IUser, IWorkflowRun } from "./entities";

export { IBranch, ICommit, IFileContent, IIssue, ILabel, INotification, IPullRequest, IPullRequestReview, IRepository, ISearchResult, IUser, IWorkflowRun } from "./entities";

/**
 * @interface IGitHubRepository
 * @description Contract for GitHub API operations
 */
export interface IGitHubRepository {
    searchCode(query: string): Promise<ISearchResult>;

    getFileContent(owner: string, repo: string, path: string, ref?: string): Promise<IFileContent>;

    getCurrentUser(): Promise<IUser>;
    getUser(username: string): Promise<IUser>;

    createIssue(owner: string, repo: string, title: string, body?: string): Promise<IIssue>;
    getIssue(owner: string, repo: string, issueNumber: number): Promise<IIssue>;
    listIssues(owner: string, repo: string, state?: "open" | "closed" | "all"): Promise<IIssue[]>;
    updateIssue(owner: string, repo: string, issueNumber: number, updates: Partial<IIssue>): Promise<IIssue>;
    searchIssues(query: string): Promise<{ items: IIssue[]; total_count: number }>;

    createPullRequest(owner: string, repo: string, title: string, head: string, base: string, body?: string): Promise<IPullRequest>;
    getPullRequest(owner: string, repo: string, prNumber: number): Promise<IPullRequest>;
    listPullRequests(owner: string, repo: string, state?: "open" | "closed" | "all"): Promise<IPullRequest[]>;
    mergePullRequest(owner: string, repo: string, prNumber: number, method?: "merge" | "squash" | "rebase"): Promise<{ merged: boolean; message: string }>;

    listPullRequestReviews(owner: string, repo: string, prNumber: number): Promise<IPullRequestReview[]>;
    createPullRequestReview(owner: string, repo: string, prNumber: number, event: "APPROVE" | "REQUEST_CHANGES" | "COMMENT", body?: string): Promise<IPullRequestReview>;

    listLabels(owner: string, repo: string): Promise<ILabel[]>;
    addLabelsToIssue(owner: string, repo: string, issueNumber: number, labels: string[]): Promise<ILabel[]>;
    removeLabelFromIssue(owner: string, repo: string, issueNumber: number, labelName: string): Promise<void>;

    listBranches(owner: string, repo: string): Promise<IBranch[]>;
    createBranch(owner: string, repo: string, branchName: string, sha: string): Promise<IBranch>;

    listCommits(owner: string, repo: string, sha?: string, perPage?: number): Promise<ICommit[]>;
    getCommit(owner: string, repo: string, ref: string): Promise<ICommit>;

    listWorkflowRuns(owner: string, repo: string, workflowId?: string): Promise<IWorkflowRun[]>;

    listNotifications(all?: boolean, participating?: boolean): Promise<INotification[]>;

    forkRepository(owner: string, repo: string): Promise<{ html_url: string }>;
    starRepository(owner: string, repo: string): Promise<void>;
}

/**
 * @template T
 * @typedef {Object} ISkillResult
 * @property {boolean} success - Operation success status
 * @property {T} [data] - Result data if successful
 * @property {string} [error] - Error message if failed
 */
export interface ISkillResult<T> {
    success: boolean;
    data?: T;
    error?: string;
}

/**
 * @template TInput,TOutput
 * @interface IUseCase
 * @description Base interface for all use cases following Clean Architecture
 */
export interface IUseCase<TInput, TOutput> {
    /**
     * @param {TInput} input - Use case input
     * @returns {Promise<ISkillResult<TOutput>>} Operation result
     */
    execute(input: TInput): Promise<ISkillResult<TOutput>>;
}
