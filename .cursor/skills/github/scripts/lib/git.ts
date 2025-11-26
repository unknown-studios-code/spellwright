import { execSync } from "child_process";
import type { ExecResult, BranchInfo, BranchType } from "./types";
import { BRANCH_TYPES, BASE_BRANCH_MAP } from "./types";

interface ExecOptions {
    silent?: boolean;
    env?: Record<string, string>;
}

export function exec(command: string, options: ExecOptions = {}): ExecResult {
    const { silent = false, env = {} } = options;

    try {
        const result = execSync(command, {
            encoding: "utf-8",
            env: { ...process.env, ...env },
        }).trim();

        if (!silent) {
            console.log(result);
        }

        return { success: true, output: result };
    } catch (error) {
        const execError = error as { message?: string; stderr?: Buffer | string };
        const errorMessage = execError.stderr?.toString() || execError.message || "Unknown error";

        return {
            success: false,
            output: "",
            error: `Command failed: ${command}\n${errorMessage}`,
        };
    }
}

export function execOrThrow(command: string, options: ExecOptions = {}): string {
    const result = exec(command, options);
    if (!result.success) {
        throw new Error(result.error);
    }
    return result.output;
}

export function getCurrentBranch(): string {
    return execOrThrow("git branch --show-current", { silent: true });
}

export function isWorkingDirectoryClean(): boolean {
    const result = exec("git status --porcelain", { silent: true });
    return result.success && result.output === "";
}

export function hasStagedChanges(): boolean {
    const result = exec("git diff --cached --name-only", { silent: true });
    return result.success && result.output !== "";
}

export function hasUnpushedCommits(): boolean {
    const result = exec("git status -sb", { silent: true });
    return result.success && result.output.includes("[ahead");
}

export function branchExists(branchName: string): boolean {
    const result = exec(`git rev-parse --verify ${branchName}`, { silent: true });
    return result.success;
}

export function remoteBranchExists(branchName: string): boolean {
    const result = exec(`git ls-remote --heads origin ${branchName}`, { silent: true });
    return result.success && result.output !== "";
}

export function getRepositoryUrl(): string | null {
    const result = exec("git remote get-url origin", { silent: true });
    if (!result.success) return null;

    const match = result.output.match(/github\.com[:/]([^/]+)\/([^/.]+)(\.git)?$/);
    if (match) {
        return `https://github.com/${match[1]}/${match[2]}`;
    }
    return result.output;
}

const TASK_BRANCH_REGEX = /^(feature|tech|bug|hotfix|docs)\/(SPWS-\d+)\/(SPWT-\d+)$/i;
const STORY_BRANCH_REGEX = /^feature\/develop\/(SPWS-\d+)$/i;

export function parseBranchName(branch: string): BranchInfo | null {
    const taskMatch = branch.match(TASK_BRANCH_REGEX);
    if (taskMatch) {
        const type = taskMatch[1].toLowerCase();
        if (isBranchType(type)) {
            return {
                type,
                storyId: taskMatch[2].toUpperCase(),
                taskId: taskMatch[3].toUpperCase(),
            };
        }
    }

    const storyMatch = branch.match(STORY_BRANCH_REGEX);
    if (storyMatch) {
        return {
            type: "feature",
            storyId: storyMatch[1].toUpperCase(),
        };
    }

    return null;
}

export function isBranchType(value: string): value is BranchType {
    return BRANCH_TYPES.includes(value as BranchType);
}

export function buildBranchName(type: BranchType, storyId: string, taskId?: string): string {
    if (taskId) {
        return `${type}/${storyId}/${taskId}`;
    }
    return `feature/develop/${storyId}`;
}

export function getBaseBranch(branchName: string): string {
    if (branchName.startsWith("hotfix/")) {
        return "main";
    }
    return "develop";
}

export function createBranch(branchName: string, baseBranch: string): void {
    console.log(`Creating branch: ${branchName}`);
    console.log(`Base branch: ${baseBranch}`);

    execOrThrow(`git fetch origin ${baseBranch}`, { silent: true });
    execOrThrow(`git checkout ${baseBranch}`, { silent: true });
    execOrThrow(`git pull origin ${baseBranch}`, { silent: true });
    execOrThrow(`git checkout -b ${branchName}`, { silent: true });

    console.log(`Created and switched to ${branchName}`);
}

export function pushBranch(branchName: string): void {
    execOrThrow(`git push -u origin ${branchName}`, { silent: true });
    console.log("Pushed to remote");
}

export function fetchAndPrune(): void {
    exec("git fetch --prune", { silent: true });
}

export function getMergedLocalBranches(protectedBranches: readonly string[]): string[] {
    const result = exec("git branch --merged", { silent: true });
    if (!result.success) return [];

    return result.output
        .split("\n")
        .map((b) => b.trim().replace(/^\* /, ""))
        .filter((b) => b && !protectedBranches.includes(b));
}

export function getMergedRemoteBranches(protectedBranches: readonly string[]): string[] {
    const result = exec("git branch -r --merged", { silent: true });
    if (!result.success) return [];

    return result.output
        .split("\n")
        .map((b) => b.trim().replace("origin/", ""))
        .filter((b) => b && !protectedBranches.includes(b) && !b.includes("->"));
}

export function getBranchLastCommitAge(branch: string): string {
    const result = exec(`git log -1 --format="%cr" origin/${branch} 2>/dev/null || git log -1 --format="%cr" ${branch}`, {
        silent: true,
    });
    return result.success && result.output ? result.output : "unknown";
}

export function deleteLocalBranch(branch: string): boolean {
    const result = exec(`git branch -d ${branch}`, { silent: true });
    return result.success;
}

export function deleteRemoteBranch(branch: string): boolean {
    const result = exec(`git push origin --delete ${branch}`, { silent: true });
    return result.success;
}

export function commit(message: string): void {
    const escaped = escapeForShell(message);
    execOrThrow(`git commit -m $'${escaped}'`, { env: { HUSKY: "0" } });
}

export function stageAll(): void {
    execOrThrow("git add .", { silent: true });
}

function escapeForShell(str: string): string {
    const escapeMap: Record<string, string> = {
        "\\": "\\\\",
        "'": "'\\''",
        "\n": "\\n",
        $: "\\$",
    };
    return str.replace(/[\\'\n$]/g, (char) => escapeMap[char] || char);
}
