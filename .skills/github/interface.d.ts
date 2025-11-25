/**
 * Standard result wrapper for all skill operations.
 */
export interface ISkillResult<T> {
    success: boolean;
    data?: T;
    error?: string;
}

/**
 * GitHub Skills API
 * Import from: `../github/index`
 */
export namespace GitHub {
    // Entities
    export interface IUser {
        login: string;
        name?: string;
        email?: string;
    }

    export interface IIssue {
        number: number;
        title: string;
        body?: string;
        state: "open" | "closed";
        html_url: string;
        user: { login: string };
        created_at: string;
        updated_at: string;
        labels?: string[];
    }

    export interface IPullRequest {
        number: number;
        title: string;
        body?: string;
        state: "open" | "closed";
        html_url: string;
        head: { ref: string; sha: string };
        base: { ref: string };
        merged: boolean;
        user: { login: string };
    }

    export interface ILabel {
        name: string;
        color: string;
        description?: string;
    }

    export interface IFileContent {
        name: string;
        path: string;
        sha: string;
        size: number;
        url: string;
        html_url: string;
        git_url: string;
        download_url: string;
        type: string;
        content: string; // Base64 encoded
        encoding: string;
    }

    // Functions
    export function addLabels(owner: string, repo: string, issueNumber: number, labels: string[]): Promise<ISkillResult<ILabel[]>>;
    export function createBranch(owner: string, repo: string, branchName: string, sourceSha: string): Promise<ISkillResult<any>>;
    export function createIssue(owner: string, repo: string, title: string, body?: string): Promise<ISkillResult<IIssue>>;
    export function createPrReview(owner: string, repo: string, prNumber: number, event: "APPROVE" | "REQUEST_CHANGES" | "COMMENT", body?: string): Promise<ISkillResult<any>>;
    export function createPullRequest(owner: string, repo: string, title: string, head: string, base: string, body?: string): Promise<ISkillResult<IPullRequest>>;
    export function forkRepository(owner: string, repo: string): Promise<ISkillResult<{ html_url: string }>>;
    export function getCommit(owner: string, repo: string, sha: string): Promise<ISkillResult<any>>;
    export function getCurrentUser(): Promise<ISkillResult<IUser>>;
    export function getFileContent(owner: string, repo: string, path: string, ref?: string): Promise<ISkillResult<IFileContent>>;
    export function getUser(username: string): Promise<ISkillResult<IUser>>;
    export function listBranches(owner: string, repo: string): Promise<ISkillResult<any[]>>;
    export function listCommits(owner: string, repo: string, branch?: string, limit?: number): Promise<ISkillResult<any[]>>;
    export function listIssues(owner: string, repo: string, state?: "open" | "closed" | "all"): Promise<ISkillResult<IIssue[]>>;
    export function listLabels(owner: string, repo: string): Promise<ISkillResult<ILabel[]>>;
    export function listNotifications(all?: boolean, participating?: boolean): Promise<ISkillResult<any[]>>;
    export function listPrReviews(owner: string, repo: string, prNumber: number): Promise<ISkillResult<any[]>>;
    export function listPullRequests(owner: string, repo: string, state?: "open" | "closed" | "all"): Promise<ISkillResult<IPullRequest[]>>;
    export function listWorkflowRuns(owner: string, repo: string, workflowId?: string): Promise<ISkillResult<any[]>>;
    export function mergePullRequest(owner: string, repo: string, prNumber: number, method?: "merge" | "squash" | "rebase"): Promise<ISkillResult<{ merged: boolean; message: string }>>;
    export function removeLabel(owner: string, repo: string, issueNumber: number, labelName: string): Promise<ISkillResult<void>>;
    export function searchCode(query: string): Promise<ISkillResult<any>>;
    export function searchIssues(query: string): Promise<ISkillResult<{ items: IIssue[]; total_count: number }>>;
    export function starRepository(owner: string, repo: string): Promise<ISkillResult<void>>;
    export function updateIssue(owner: string, repo: string, issueNumber: number, updates: { title?: string; body?: string; state?: "open" | "closed" }): Promise<ISkillResult<IIssue>>;
}
