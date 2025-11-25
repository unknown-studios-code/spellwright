import { IBranch, ICommit, IFileContent, IIssue, ILabel, INotification, IPullRequest, IPullRequestReview, IUser, IWorkflowRun } from "../domain/interfaces";

/**
 * Maps a raw GitHub API issue response to the IIssue interface.
 * @param {any} data - The raw issue object from GitHub API.
 * @returns {IIssue} The mapped issue.
 */
export function mapIssue(data: any): IIssue {
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
 * Maps a raw GitHub API pull request response to the IPullRequest interface.
 * @param {any} data - The raw pull request object from GitHub API.
 * @returns {IPullRequest} The mapped pull request.
 */
export function mapPullRequest(data: any): IPullRequest {
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

/**
 * Maps a raw GitHub API review response to the IPullRequestReview interface.
 * @param {any} data - The raw review object from GitHub API.
 * @returns {IPullRequestReview} The mapped review.
 */
export function mapReview(data: any): IPullRequestReview {
    return {
        id: data.id,
        user: { login: data.user?.login || "unknown" },
        body: data.body || "",
        state: data.state as "APPROVED" | "CHANGES_REQUESTED" | "COMMENTED",
        submitted_at: data.submitted_at || "",
        html_url: data.html_url,
    };
}

/**
 * Maps a raw GitHub API label response to the ILabel interface.
 * @param {any} data - The raw label object from GitHub API.
 * @returns {ILabel} The mapped label.
 */
export function mapLabel(data: any): ILabel {
    return {
        name: data.name,
        color: data.color,
        description: data.description || undefined,
    };
}

/**
 * Maps a raw GitHub API branch response to the IBranch interface.
 * @param {any} data - The raw branch object from GitHub API.
 * @returns {IBranch} The mapped branch.
 */
export function mapBranch(data: any): IBranch {
    return {
        name: data.name,
        commit: { sha: data.commit.sha },
        protected: data.protected,
    };
}

/**
 * Maps a raw GitHub API commit response to the ICommit interface.
 * @param {any} data - The raw commit object from GitHub API.
 * @returns {ICommit} The mapped commit.
 */
export function mapCommit(data: any): ICommit {
    return {
        sha: data.sha,
        commit: {
            message: data.commit.message,
            author: {
                name: data.commit.author?.name || "unknown",
                email: data.commit.author?.email || "",
                date: data.commit.author?.date || "",
            },
        },
        html_url: data.html_url,
    };
}

/**
 * Maps a raw GitHub API workflow run response to the IWorkflowRun interface.
 * @param {any} data - The raw workflow run object from GitHub API.
 * @returns {IWorkflowRun} The mapped workflow run.
 */
export function mapWorkflowRun(data: any): IWorkflowRun {
    return {
        id: data.id,
        name: data.name || "Unknown",
        head_branch: data.head_branch || "",
        head_sha: data.head_sha,
        status: data.status as "queued" | "in_progress" | "completed",
        conclusion: data.conclusion as "success" | "failure" | "neutral" | "cancelled" | "skipped" | "timed_out" | undefined,
        html_url: data.html_url,
        created_at: data.created_at,
        updated_at: data.updated_at,
    };
}

/**
 * Maps a raw GitHub API notification response to the INotification interface.
 * @param {any} data - The raw notification object from GitHub API.
 * @returns {INotification} The mapped notification.
 */
export function mapNotification(data: any): INotification {
    return {
        id: data.id,
        repository: { full_name: data.repository.full_name },
        subject: {
            title: data.subject.title,
            type: data.subject.type,
        },
        reason: data.reason,
        unread: data.unread,
        updated_at: data.updated_at,
    };
}

/**
 * Maps a raw GitHub API user response to the IUser interface.
 * @param {any} data - The raw user object from GitHub API.
 * @returns {IUser} The mapped user.
 */
export function mapUser(data: any): IUser {
    return {
        login: data.login,
        name: data.name || undefined,
        email: data.email || undefined,
    };
}

/**
 * Maps a raw GitHub API file content response to the IFileContent interface.
 * @param {any} data - The raw file content object from GitHub API.
 * @returns {IFileContent} The mapped file content.
 */
export function mapFileContent(data: any): IFileContent {
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
 * Maps a raw GitHub API code search item to the search result item format.
 * @param {any} data - The raw code search item from GitHub API.
 * @returns {Object} The mapped search code item.
 */
export function mapSearchCodeItem(data: any): { name: string; path: string; sha: string; url: string; repository: { full_name: string } } {
    return {
        name: data.name,
        path: data.path,
        sha: data.sha,
        url: data.html_url,
        repository: {
            full_name: data.repository.full_name,
        },
    };
}
