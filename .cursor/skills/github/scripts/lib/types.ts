export const BRANCH_TYPES = ["feature", "tech", "bug", "hotfix", "docs"] as const;
export type BranchType = (typeof BRANCH_TYPES)[number];

export const COMMIT_TYPES = ["feat", "fix", "perf", "docs", "style", "refactor", "test", "build", "ci", "chore", "revert"] as const;
export type CommitType = (typeof COMMIT_TYPES)[number];

export const BASE_BRANCH_MAP: Record<BranchType, string> = {
    feature: "develop",
    tech: "develop",
    bug: "develop",
    hotfix: "main",
    docs: "develop",
};

export interface BranchInfo {
    type: BranchType;
    storyId: string;
    taskId?: string;
}

export interface CommitConfig {
    type: CommitType;
    scope?: string;
    subject: string;
    body?: string;
    implements?: string;
    partOf?: string;
    related?: string;
}

export interface PrConfig {
    title: string;
    body: string;
    baseBranch: string;
    labels: string[];
    draft: boolean;
}

export interface ExecResult {
    success: boolean;
    output: string;
    error?: string;
}

export interface ValidationResult {
    valid: boolean;
    error?: string;
}

export interface MergedBranchInfo {
    name: string;
    isLocal: boolean;
    isRemote: boolean;
    lastCommitAge: string;
    prNumber?: number;
    prUrl?: string;
}

export interface CleanupConfig {
    dryRun: boolean;
    localOnly: boolean;
    remoteOnly: boolean;
    skipPRCheck: boolean;
    include?: string;
    exclude?: string;
}

export const PROTECTED_BRANCHES = ["main", "master", "develop", "HEAD"] as const;
