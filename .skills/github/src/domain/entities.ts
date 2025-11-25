/**
 * @interface IRepository
 * @property {string} owner - Repository owner username or organization
 * @property {string} name - Repository name
 * @property {string} [description] - Repository description
 * @property {string} url - Repository URL
 * @property {string} defaultBranch - Default branch name
 */
export interface IRepository {
    owner: string;
    name: string;
    description?: string;
    url: string;
    defaultBranch: string;
}

/**
 * @interface IFileContent
 * @property {string} name - File name
 * @property {string} path - File path in repository
 * @property {string} sha - Git SHA hash
 * @property {number} size - File size in bytes
 * @property {string} url - API URL
 * @property {string} html_url - Web URL
 * @property {string} git_url - Git URL
 * @property {string} download_url - Download URL
 * @property {string} type - Content type
 * @property {string} content - Base64 encoded content
 * @property {string} encoding - Content encoding
 */
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
    content: string;
    encoding: string;
}

/**
 * @interface IUser
 * @property {string} login - GitHub username
 * @property {string} [name] - Display name
 * @property {string} [email] - Email address
 */
export interface IUser {
    login: string;
    name?: string;
    email?: string;
}

/**
 * @interface IIssue
 * @property {number} number - Issue number
 * @property {string} title - Issue title
 * @property {string} [body] - Issue description
 * @property {"open" | "closed"} state - Issue state
 * @property {string} html_url - Web URL
 * @property {Object} user - Issue author
 * @property {string} user.login - Author username
 * @property {string} created_at - Creation timestamp
 * @property {string} updated_at - Last update timestamp
 * @property {string[]} [labels] - Array of label names
 */
export interface IIssue {
    number: number;
    title: string;
    body?: string;
    state: "open" | "closed";
    html_url: string;
    user: {
        login: string;
    };
    created_at: string;
    updated_at: string;
    labels?: string[];
}

/**
 * @interface IPullRequest
 * @property {number} number - Pull request number
 * @property {string} title - Pull request title
 * @property {string} [body] - Pull request description
 * @property {"open" | "closed"} state - Pull request state
 * @property {string} html_url - Web URL
 * @property {Object} head - Source branch info
 * @property {string} head.ref - Source branch name
 * @property {string} head.sha - Source commit SHA
 * @property {Object} base - Target branch info
 * @property {string} base.ref - Target branch name
 * @property {boolean} merged - Merge status
 * @property {Object} user - Pull request author
 * @property {string} user.login - Author username
 */
export interface IPullRequest {
    number: number;
    title: string;
    body?: string;
    state: "open" | "closed";
    html_url: string;
    head: {
        ref: string;
        sha: string;
    };
    base: {
        ref: string;
    };
    merged: boolean;
    user: {
        login: string;
    };
}

/**
 * @interface ILabel
 * @property {string} name - Label name
 * @property {string} color - Label color (hex without #)
 * @property {string} [description] - Label description
 */
export interface ILabel {
    name: string;
    color: string;
    description?: string;
}

/**
 * @interface IPullRequestReview
 * @property {number} id - Review ID
 * @property {Object} user - Reviewer
 * @property {string} user.login - Reviewer username
 * @property {string} body - Review comment
 * @property {"APPROVED" | "CHANGES_REQUESTED" | "COMMENTED"} state - Review state
 * @property {string} submitted_at - Submission timestamp
 * @property {string} html_url - Web URL
 */
export interface IPullRequestReview {
    id: number;
    user: {
        login: string;
    };
    body: string;
    state: "APPROVED" | "CHANGES_REQUESTED" | "COMMENTED";
    submitted_at: string;
    html_url: string;
}

/**
 * @interface IWorkflowRun
 * @property {number} id - Workflow run ID
 * @property {string} name - Workflow name
 * @property {string} head_branch - Branch name
 * @property {string} head_sha - Commit SHA
 * @property {"queued" | "in_progress" | "completed"} status - Run status
 * @property {"success" | "failure" | "neutral" | "cancelled" | "skipped" | "timed_out"} [conclusion] - Run conclusion
 * @property {string} html_url - Web URL
 * @property {string} created_at - Creation timestamp
 * @property {string} updated_at - Last update timestamp
 */
export interface IWorkflowRun {
    id: number;
    name: string;
    head_branch: string;
    head_sha: string;
    status: "queued" | "in_progress" | "completed";
    conclusion?: "success" | "failure" | "neutral" | "cancelled" | "skipped" | "timed_out";
    html_url: string;
    created_at: string;
    updated_at: string;
}

/**
 * @interface INotification
 * @property {string} id - Notification ID
 * @property {Object} repository - Repository info
 * @property {string} repository.full_name - Full repository name
 * @property {Object} subject - Notification subject
 * @property {string} subject.title - Subject title
 * @property {string} subject.type - Subject type (Issue, PullRequest, etc)
 * @property {string} reason - Notification reason
 * @property {boolean} unread - Read status
 * @property {string} updated_at - Last update timestamp
 */
export interface INotification {
    id: string;
    repository: {
        full_name: string;
    };
    subject: {
        title: string;
        type: string;
    };
    reason: string;
    unread: boolean;
    updated_at: string;
}

/**
 * @interface IBranch
 * @property {string} name - Branch name
 * @property {Object} commit - Latest commit info
 * @property {string} commit.sha - Commit SHA
 * @property {boolean} protected - Protection status
 */
export interface IBranch {
    name: string;
    commit: {
        sha: string;
    };
    protected: boolean;
}

/**
 * @interface ICommit
 * @property {string} sha - Commit SHA
 * @property {Object} commit - Commit details
 * @property {string} commit.message - Commit message
 * @property {Object} commit.author - Commit author
 * @property {string} commit.author.name - Author name
 * @property {string} commit.author.email - Author email
 * @property {string} commit.author.date - Commit date
 * @property {string} html_url - Web URL
 */
export interface ICommit {
    sha: string;
    commit: {
        message: string;
        author: {
            name: string;
            email: string;
            date: string;
        };
    };
    html_url: string;
}

/**
 * @interface ISearchResult
 * @property {Array<Object>} items - Search result items
 * @property {number} total_count - Total number of results
 */
export interface ISearchResult {
    items: Array<{
        name: string;
        path: string;
        sha: string;
        url: string;
        repository: {
            full_name: string;
        };
    }>;
    total_count: number;
}
