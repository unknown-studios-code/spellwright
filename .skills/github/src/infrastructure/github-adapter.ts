import { Octokit } from "@octokit/rest";
import * as dotenv from "dotenv";
import { IBranch, ICommit, IFileContent, IGitHubRepository, IIssue, ILabel, INotification, IPullRequest, IPullRequestReview, ISearchResult, IUser, IWorkflowRun } from "../domain/interfaces";

dotenv.config({ path: "./.env" });

/**
 * @class GitHubAdapter
 * @implements {IGitHubRepository}
 * @description Adapter for GitHub API using Octokit
 */
export class GitHubAdapter implements IGitHubRepository {
    private client: Octokit;

    /**
     * @constructor
     * @throws {Error} If GITHUB_TOKEN environment variable is not set
     */
    constructor() {
        const token = process.env.GITHUB_TOKEN;
        if (!token) {
            throw new Error("GITHUB_TOKEN environment variable is not set.");
        }
        this.client = new Octokit({ auth: token });
    }

    /**
     * @param {string} query - Search query
     * @returns {Promise<ISearchResult>} Search results
     */
    async searchCode(query: string): Promise<ISearchResult> {
        const response = await this.client.search.code({ q: query });
        return {
            items: response.data.items.map((item) => ({
                name: item.name,
                path: item.path,
                sha: item.sha,
                url: item.html_url,
                repository: {
                    full_name: item.repository.full_name,
                },
            })),
            total_count: response.data.total_count,
        };
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {string} path - File path
     * @param {string} [ref] - Git reference
     * @returns {Promise<IFileContent>} File content
     * @throws {Error} If path points to a directory
     */
    async getFileContent(owner: string, repo: string, path: string, ref?: string): Promise<IFileContent> {
        const response = await this.client.repos.getContent({ owner, repo, path, ref });

        if (Array.isArray(response.data)) {
            throw new Error("Path points to a directory, not a file");
        }

        const data = response.data as any;
        return {
            name: data.name,
            path: data.path,
            sha: data.sha,
            size: data.size,
            url: data.url,
            html_url: data.html_url,
            git_url: data.git_url,
            download_url: data.download_url,
            type: data.type,
            content: data.content,
            encoding: data.encoding,
        };
    }

    /**
     * @returns {Promise<IUser>} Current authenticated user
     */
    async getCurrentUser(): Promise<IUser> {
        const response = await this.client.users.getAuthenticated();
        return {
            login: response.data.login,
            name: response.data.name || undefined,
            email: response.data.email || undefined,
        };
    }

    /**
     * @param {string} username - GitHub username
     * @returns {Promise<IUser>} User information
     */
    async getUser(username: string): Promise<IUser> {
        const response = await this.client.users.getByUsername({ username });
        return {
            login: response.data.login,
            name: response.data.name || undefined,
            email: response.data.email || undefined,
        };
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {string} title - Issue title
     * @param {string} [body] - Issue body
     * @returns {Promise<IIssue>} Created issue
     */
    async createIssue(owner: string, repo: string, title: string, body?: string): Promise<IIssue> {
        const response = await this.client.issues.create({ owner, repo, title, body });
        return this.mapIssue(response.data);
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {number} issueNumber - Issue number
     * @returns {Promise<IIssue>} Issue details
     */
    async getIssue(owner: string, repo: string, issueNumber: number): Promise<IIssue> {
        const response = await this.client.issues.get({ owner, repo, issue_number: issueNumber });
        return this.mapIssue(response.data);
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {"open" | "closed" | "all"} [state="open"] - Issue state filter
     * @returns {Promise<IIssue[]>} List of issues
     */
    async listIssues(owner: string, repo: string, state: "open" | "closed" | "all" = "open"): Promise<IIssue[]> {
        const response = await this.client.issues.listForRepo({ owner, repo, state });
        return response.data.map(this.mapIssue);
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {number} issueNumber - Issue number
     * @param {Partial<IIssue>} updates - Fields to update
     * @returns {Promise<IIssue>} Updated issue
     */
    async updateIssue(owner: string, repo: string, issueNumber: number, updates: Partial<IIssue>): Promise<IIssue> {
        const response = await this.client.issues.update({ owner, repo, issue_number: issueNumber, ...updates });
        return this.mapIssue(response.data);
    }

    /**
     * @param {string} query - Search query
     * @returns {Promise<{items: IIssue[], total_count: number}>} Search results
     */
    async searchIssues(query: string): Promise<{ items: IIssue[]; total_count: number }> {
        const response = await this.client.search.issuesAndPullRequests({ q: query });
        return {
            items: response.data.items.map(this.mapIssue),
            total_count: response.data.total_count,
        };
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {string} title - Pull request title
     * @param {string} head - Source branch
     * @param {string} base - Target branch
     * @param {string} [body] - Pull request body
     * @returns {Promise<IPullRequest>} Created pull request
     */
    async createPullRequest(owner: string, repo: string, title: string, head: string, base: string, body?: string): Promise<IPullRequest> {
        const response = await this.client.pulls.create({ owner, repo, title, head, base, body });
        return this.mapPullRequest(response.data);
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {number} prNumber - Pull request number
     * @returns {Promise<IPullRequest>} Pull request details
     */
    async getPullRequest(owner: string, repo: string, prNumber: number): Promise<IPullRequest> {
        const response = await this.client.pulls.get({ owner, repo, pull_number: prNumber });
        return this.mapPullRequest(response.data);
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {"open" | "closed" | "all"} [state="open"] - Pull request state filter
     * @returns {Promise<IPullRequest[]>} List of pull requests
     */
    async listPullRequests(owner: string, repo: string, state: "open" | "closed" | "all" = "open"): Promise<IPullRequest[]> {
        const response = await this.client.pulls.list({ owner, repo, state });
        return response.data.map(this.mapPullRequest);
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {number} prNumber - Pull request number
     * @param {"merge" | "squash" | "rebase"} [method="merge"] - Merge method
     * @returns {Promise<{merged: boolean, message: string}>} Merge result
     */
    async mergePullRequest(owner: string, repo: string, prNumber: number, method: "merge" | "squash" | "rebase" = "merge"): Promise<{ merged: boolean; message: string }> {
        const response = await this.client.pulls.merge({ owner, repo, pull_number: prNumber, merge_method: method });
        return {
            merged: response.data.merged,
            message: response.data.message,
        };
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {number} prNumber - Pull request number
     * @returns {Promise<IPullRequestReview[]>} List of reviews
     */
    async listPullRequestReviews(owner: string, repo: string, prNumber: number): Promise<IPullRequestReview[]> {
        const response = await this.client.pulls.listReviews({ owner, repo, pull_number: prNumber });
        return response.data.map((review) => ({
            id: review.id,
            user: { login: review.user?.login || "unknown" },
            body: review.body || "",
            state: review.state as "APPROVED" | "CHANGES_REQUESTED" | "COMMENTED",
            submitted_at: review.submitted_at || "",
            html_url: review.html_url,
        }));
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {number} prNumber - Pull request number
     * @param {"APPROVE" | "REQUEST_CHANGES" | "COMMENT"} event - Review event
     * @param {string} [body] - Review comment
     * @returns {Promise<IPullRequestReview>} Created review
     */
    async createPullRequestReview(owner: string, repo: string, prNumber: number, event: "APPROVE" | "REQUEST_CHANGES" | "COMMENT", body?: string): Promise<IPullRequestReview> {
        const response = await this.client.pulls.createReview({
            owner,
            repo,
            pull_number: prNumber,
            event,
            body,
        });
        return {
            id: response.data.id,
            user: { login: response.data.user?.login || "unknown" },
            body: response.data.body || "",
            state: response.data.state as "APPROVED" | "CHANGES_REQUESTED" | "COMMENTED",
            submitted_at: response.data.submitted_at || "",
            html_url: response.data.html_url,
        };
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @returns {Promise<ILabel[]>} List of labels
     */
    async listLabels(owner: string, repo: string): Promise<ILabel[]> {
        const response = await this.client.issues.listLabelsForRepo({ owner, repo });
        return response.data.map((label) => ({
            name: label.name,
            color: label.color,
            description: label.description || undefined,
        }));
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {number} issueNumber - Issue or PR number
     * @param {string[]} labels - Label names to add
     * @returns {Promise<ILabel[]>} Updated labels
     */
    async addLabelsToIssue(owner: string, repo: string, issueNumber: number, labels: string[]): Promise<ILabel[]> {
        const response = await this.client.issues.addLabels({ owner, repo, issue_number: issueNumber, labels });
        return response.data.map((label) => ({
            name: label.name,
            color: label.color,
            description: label.description || undefined,
        }));
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {number} issueNumber - Issue or PR number
     * @param {string} labelName - Label name to remove
     * @returns {Promise<void>}
     */
    async removeLabelFromIssue(owner: string, repo: string, issueNumber: number, labelName: string): Promise<void> {
        await this.client.issues.removeLabel({ owner, repo, issue_number: issueNumber, name: labelName });
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @returns {Promise<IBranch[]>} List of branches
     */
    async listBranches(owner: string, repo: string): Promise<IBranch[]> {
        const response = await this.client.repos.listBranches({ owner, repo });
        return response.data.map((branch) => ({
            name: branch.name,
            commit: { sha: branch.commit.sha },
            protected: branch.protected,
        }));
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {string} branchName - New branch name
     * @param {string} sha - Commit SHA to branch from
     * @returns {Promise<IBranch>} Created branch reference
     */
    async createBranch(owner: string, repo: string, branchName: string, sha: string): Promise<IBranch> {
        const ref = `refs/heads/${branchName}`;
        const response = await this.client.git.createRef({ owner, repo, ref, sha });
        return {
            name: branchName,
            commit: { sha: response.data.object.sha },
            protected: false,
        };
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {string} [sha] - Starting SHA
     * @param {number} [perPage=30] - Results per page
     * @returns {Promise<ICommit[]>} List of commits
     */
    async listCommits(owner: string, repo: string, sha?: string, perPage: number = 30): Promise<ICommit[]> {
        const response = await this.client.repos.listCommits({ owner, repo, sha, per_page: perPage });
        return response.data.map((commit) => ({
            sha: commit.sha,
            commit: {
                message: commit.commit.message,
                author: {
                    name: commit.commit.author?.name || "unknown",
                    email: commit.commit.author?.email || "",
                    date: commit.commit.author?.date || "",
                },
            },
            html_url: commit.html_url,
        }));
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {string} ref - Commit reference (SHA, branch, tag)
     * @returns {Promise<ICommit>} Commit details
     */
    async getCommit(owner: string, repo: string, ref: string): Promise<ICommit> {
        const response = await this.client.repos.getCommit({ owner, repo, ref });
        return {
            sha: response.data.sha,
            commit: {
                message: response.data.commit.message,
                author: {
                    name: response.data.commit.author?.name || "unknown",
                    email: response.data.commit.author?.email || "",
                    date: response.data.commit.author?.date || "",
                },
            },
            html_url: response.data.html_url,
        };
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @param {string} [workflowId] - Specific workflow ID or filename
     * @returns {Promise<IWorkflowRun[]>} List of workflow runs
     */
    async listWorkflowRuns(owner: string, repo: string, workflowId?: string): Promise<IWorkflowRun[]> {
        const response = workflowId ? await this.client.actions.listWorkflowRuns({ owner, repo, workflow_id: workflowId }) : await this.client.actions.listWorkflowRunsForRepo({ owner, repo });

        return response.data.workflow_runs.map((run) => ({
            id: run.id,
            name: run.name || "Unknown",
            head_branch: run.head_branch || "",
            head_sha: run.head_sha,
            status: run.status as "queued" | "in_progress" | "completed",
            conclusion: run.conclusion as "success" | "failure" | "neutral" | "cancelled" | "skipped" | "timed_out" | undefined,
            html_url: run.html_url,
            created_at: run.created_at,
            updated_at: run.updated_at,
        }));
    }

    /**
     * @param {boolean} [all=false] - Include read notifications
     * @param {boolean} [participating=false] - Only show notifications user is participating in
     * @returns {Promise<INotification[]>} List of notifications
     */
    async listNotifications(all: boolean = false, participating: boolean = false): Promise<INotification[]> {
        const response = await this.client.activity.listNotificationsForAuthenticatedUser({ all, participating });
        return response.data.map((notification) => ({
            id: notification.id,
            repository: { full_name: notification.repository.full_name },
            subject: {
                title: notification.subject.title,
                type: notification.subject.type,
            },
            reason: notification.reason,
            unread: notification.unread,
            updated_at: notification.updated_at,
        }));
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @returns {Promise<{html_url: string}>} Forked repository URL
     */
    async forkRepository(owner: string, repo: string): Promise<{ html_url: string }> {
        const response = await this.client.repos.createFork({ owner, repo });
        return { html_url: response.data.html_url };
    }

    /**
     * @param {string} owner - Repository owner
     * @param {string} repo - Repository name
     * @returns {Promise<void>}
     */
    async starRepository(owner: string, repo: string): Promise<void> {
        await this.client.activity.starRepoForAuthenticatedUser({ owner, repo });
    }

    /**
     * @private
     * @param {any} data - GitHub API issue data
     * @returns {IIssue} Mapped issue entity
     */
    private mapIssue(data: any): IIssue {
        return {
            number: data.number,
            title: data.title,
            body: data.body,
            state: data.state,
            html_url: data.html_url,
            user: { login: data.user?.login || "unknown" },
            created_at: data.created_at,
            updated_at: data.updated_at,
            labels: data.labels?.map((label: any) => (typeof label === "string" ? label : label.name)),
        };
    }

    /**
     * @private
     * @param {any} data - GitHub API pull request data
     * @returns {IPullRequest} Mapped pull request entity
     */
    private mapPullRequest(data: any): IPullRequest {
        return {
            number: data.number,
            title: data.title,
            body: data.body,
            state: data.state,
            html_url: data.html_url,
            head: { ref: data.head.ref, sha: data.head.sha },
            base: { ref: data.base.ref },
            merged: data.merged || false,
            user: { login: data.user?.login || "unknown" },
        };
    }
}
