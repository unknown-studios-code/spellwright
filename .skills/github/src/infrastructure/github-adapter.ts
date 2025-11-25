import { Octokit } from "@octokit/rest";
import * as dotenv from "dotenv";
import { IBranch, ICommit, IFileContent, IGitHubRepository, IIssue, ILabel, INotification, IPullRequest, IPullRequestReview, ISearchResult, IUser, IWorkflowRun } from "../domain/interfaces";
import { mapBranch, mapCommit, mapFileContent, mapIssue, mapLabel, mapNotification, mapPullRequest, mapReview, mapSearchCodeItem, mapUser, mapWorkflowRun } from "./github-utils";

dotenv.config({ path: "./.env" });

/**
 * Implementation of the GitHub Repository using Octokit SDK.
 * @class GitHubAdapter
 * @implements {IGitHubRepository}
 */
export class GitHubAdapter implements IGitHubRepository {
    private client: Octokit;

    /**
     * Initializes the Octokit client.
     * @constructor
     * @throws {Error} If GITHUB_TOKEN environment variable is not set.
     */
    constructor() {
        const token = process.env.GITHUB_TOKEN;
        if (!token) {
            throw new Error("GITHUB_TOKEN environment variable is not set.");
        }
        this.client = new Octokit({ auth: token });
    }

    /**
     * Searches for code across GitHub repositories.
     * @param {string} query - The search query.
     * @returns {Promise<ISearchResult>} The search results.
     */
    async searchCode(query: string): Promise<ISearchResult> {
        const response = await this.client.search.code({ q: query });
        return {
            items: response.data.items.map(mapSearchCodeItem),
            total_count: response.data.total_count,
        };
    }

    /**
     * Gets the content of a file from a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {string} path - The file path.
     * @param {string} [ref] - The git reference (branch, tag, or commit SHA).
     * @returns {Promise<IFileContent>} The file content.
     * @throws {Error} If path points to a directory.
     */
    async getFileContent(owner: string, repo: string, path: string, ref?: string): Promise<IFileContent> {
        const response = await this.client.repos.getContent({ owner, repo, path, ref });

        if (Array.isArray(response.data)) {
            throw new Error("Path points to a directory, not a file");
        }

        return mapFileContent(response.data);
    }

    /**
     * Gets the currently authenticated user.
     * @returns {Promise<IUser>} The current user.
     */
    async getCurrentUser(): Promise<IUser> {
        const response = await this.client.users.getAuthenticated();
        return mapUser(response.data);
    }

    /**
     * Gets a user by their username.
     * @param {string} username - The GitHub username.
     * @returns {Promise<IUser>} The user details.
     */
    async getUser(username: string): Promise<IUser> {
        const response = await this.client.users.getByUsername({ username });
        return mapUser(response.data);
    }

    /**
     * Creates a new issue in a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {string} title - The issue title.
     * @param {string} [body] - The issue body.
     * @returns {Promise<IIssue>} The created issue.
     */
    async createIssue(owner: string, repo: string, title: string, body?: string): Promise<IIssue> {
        const response = await this.client.issues.create({ owner, repo, title, body });
        return mapIssue(response.data);
    }

    /**
     * Gets an issue by its number.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {number} issueNumber - The issue number.
     * @returns {Promise<IIssue>} The issue details.
     */
    async getIssue(owner: string, repo: string, issueNumber: number): Promise<IIssue> {
        const response = await this.client.issues.get({ owner, repo, issue_number: issueNumber });
        return mapIssue(response.data);
    }

    /**
     * Lists issues in a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {"open" | "closed" | "all"} [state="open"] - The issue state filter.
     * @returns {Promise<IIssue[]>} The list of issues.
     */
    async listIssues(owner: string, repo: string, state: "open" | "closed" | "all" = "open"): Promise<IIssue[]> {
        const response = await this.client.issues.listForRepo({ owner, repo, state });
        return response.data.map(mapIssue);
    }

    /**
     * Updates an existing issue.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {number} issueNumber - The issue number.
     * @param {Partial<IIssue>} updates - The fields to update.
     * @returns {Promise<IIssue>} The updated issue.
     */
    async updateIssue(owner: string, repo: string, issueNumber: number, updates: Partial<IIssue>): Promise<IIssue> {
        const response = await this.client.issues.update({ owner, repo, issue_number: issueNumber, ...updates });
        return mapIssue(response.data);
    }

    /**
     * Searches for issues and pull requests.
     * @param {string} query - The search query.
     * @returns {Promise<{ items: IIssue[]; total_count: number }>} The search results.
     */
    async searchIssues(query: string): Promise<{ items: IIssue[]; total_count: number }> {
        const response = await this.client.search.issuesAndPullRequests({ q: query });
        return {
            items: response.data.items.map(mapIssue),
            total_count: response.data.total_count,
        };
    }

    /**
     * Creates a new pull request.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {string} title - The pull request title.
     * @param {string} head - The source branch.
     * @param {string} base - The target branch.
     * @param {string} [body] - The pull request body.
     * @returns {Promise<IPullRequest>} The created pull request.
     */
    async createPullRequest(owner: string, repo: string, title: string, head: string, base: string, body?: string): Promise<IPullRequest> {
        const response = await this.client.pulls.create({ owner, repo, title, head, base, body });
        return mapPullRequest(response.data);
    }

    /**
     * Gets a pull request by its number.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {number} prNumber - The pull request number.
     * @returns {Promise<IPullRequest>} The pull request details.
     */
    async getPullRequest(owner: string, repo: string, prNumber: number): Promise<IPullRequest> {
        const response = await this.client.pulls.get({ owner, repo, pull_number: prNumber });
        return mapPullRequest(response.data);
    }

    /**
     * Lists pull requests in a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {"open" | "closed" | "all"} [state="open"] - The pull request state filter.
     * @returns {Promise<IPullRequest[]>} The list of pull requests.
     */
    async listPullRequests(owner: string, repo: string, state: "open" | "closed" | "all" = "open"): Promise<IPullRequest[]> {
        const response = await this.client.pulls.list({ owner, repo, state });
        return response.data.map(mapPullRequest);
    }

    /**
     * Merges a pull request.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {number} prNumber - The pull request number.
     * @param {"merge" | "squash" | "rebase"} [method="merge"] - The merge method.
     * @returns {Promise<{ merged: boolean; message: string }>} The merge result.
     */
    async mergePullRequest(owner: string, repo: string, prNumber: number, method: "merge" | "squash" | "rebase" = "merge"): Promise<{ merged: boolean; message: string }> {
        const response = await this.client.pulls.merge({ owner, repo, pull_number: prNumber, merge_method: method });
        return {
            merged: response.data.merged,
            message: response.data.message,
        };
    }

    /**
     * Lists reviews on a pull request.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {number} prNumber - The pull request number.
     * @returns {Promise<IPullRequestReview[]>} The list of reviews.
     */
    async listPullRequestReviews(owner: string, repo: string, prNumber: number): Promise<IPullRequestReview[]> {
        const response = await this.client.pulls.listReviews({ owner, repo, pull_number: prNumber });
        return response.data.map(mapReview);
    }

    /**
     * Creates a review on a pull request.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {number} prNumber - The pull request number.
     * @param {"APPROVE" | "REQUEST_CHANGES" | "COMMENT"} event - The review event type.
     * @param {string} [body] - The review comment.
     * @returns {Promise<IPullRequestReview>} The created review.
     */
    async createPullRequestReview(owner: string, repo: string, prNumber: number, event: "APPROVE" | "REQUEST_CHANGES" | "COMMENT", body?: string): Promise<IPullRequestReview> {
        const response = await this.client.pulls.createReview({ owner, repo, pull_number: prNumber, event, body });
        return mapReview(response.data);
    }

    /**
     * Lists labels in a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @returns {Promise<ILabel[]>} The list of labels.
     */
    async listLabels(owner: string, repo: string): Promise<ILabel[]> {
        const response = await this.client.issues.listLabelsForRepo({ owner, repo });
        return response.data.map(mapLabel);
    }

    /**
     * Adds labels to an issue or pull request.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {number} issueNumber - The issue or pull request number.
     * @param {string[]} labels - The label names to add.
     * @returns {Promise<ILabel[]>} The updated labels.
     */
    async addLabelsToIssue(owner: string, repo: string, issueNumber: number, labels: string[]): Promise<ILabel[]> {
        const response = await this.client.issues.addLabels({ owner, repo, issue_number: issueNumber, labels });
        return response.data.map(mapLabel);
    }

    /**
     * Removes a label from an issue or pull request.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {number} issueNumber - The issue or pull request number.
     * @param {string} labelName - The label name to remove.
     * @returns {Promise<void>}
     */
    async removeLabelFromIssue(owner: string, repo: string, issueNumber: number, labelName: string): Promise<void> {
        await this.client.issues.removeLabel({ owner, repo, issue_number: issueNumber, name: labelName });
    }

    /**
     * Lists branches in a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @returns {Promise<IBranch[]>} The list of branches.
     */
    async listBranches(owner: string, repo: string): Promise<IBranch[]> {
        const response = await this.client.repos.listBranches({ owner, repo });
        return response.data.map(mapBranch);
    }

    /**
     * Creates a new branch.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {string} branchName - The new branch name.
     * @param {string} sha - The commit SHA to branch from.
     * @returns {Promise<IBranch>} The created branch reference.
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
     * Lists commits in a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {string} [sha] - The starting commit SHA.
     * @param {number} [perPage=30] - The number of results per page.
     * @returns {Promise<ICommit[]>} The list of commits.
     */
    async listCommits(owner: string, repo: string, sha?: string, perPage: number = 30): Promise<ICommit[]> {
        const response = await this.client.repos.listCommits({ owner, repo, sha, per_page: perPage });
        return response.data.map(mapCommit);
    }

    /**
     * Gets a commit by its reference.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {string} ref - The commit reference (SHA, branch, or tag).
     * @returns {Promise<ICommit>} The commit details.
     */
    async getCommit(owner: string, repo: string, ref: string): Promise<ICommit> {
        const response = await this.client.repos.getCommit({ owner, repo, ref });
        return mapCommit(response.data);
    }

    /**
     * Lists workflow runs in a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @param {string} [workflowId] - The specific workflow ID or filename.
     * @returns {Promise<IWorkflowRun[]>} The list of workflow runs.
     */
    async listWorkflowRuns(owner: string, repo: string, workflowId?: string): Promise<IWorkflowRun[]> {
        const response = workflowId ? await this.client.actions.listWorkflowRuns({ owner, repo, workflow_id: workflowId }) : await this.client.actions.listWorkflowRunsForRepo({ owner, repo });
        return response.data.workflow_runs.map(mapWorkflowRun);
    }

    /**
     * Lists notifications for the authenticated user.
     * @param {boolean} [all=false] - Include read notifications.
     * @param {boolean} [participating=false] - Only show notifications user is participating in.
     * @returns {Promise<INotification[]>} The list of notifications.
     */
    async listNotifications(all: boolean = false, participating: boolean = false): Promise<INotification[]> {
        const response = await this.client.activity.listNotificationsForAuthenticatedUser({ all, participating });
        return response.data.map(mapNotification);
    }

    /**
     * Forks a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @returns {Promise<{ html_url: string }>} The forked repository URL.
     */
    async forkRepository(owner: string, repo: string): Promise<{ html_url: string }> {
        const response = await this.client.repos.createFork({ owner, repo });
        return { html_url: response.data.html_url };
    }

    /**
     * Stars a repository.
     * @param {string} owner - The repository owner.
     * @param {string} repo - The repository name.
     * @returns {Promise<void>}
     */
    async starRepository(owner: string, repo: string): Promise<void> {
        await this.client.activity.starRepoForAuthenticatedUser({ owner, repo });
    }
}
