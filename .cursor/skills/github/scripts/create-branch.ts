#!/usr/bin/env npx ts-node

import { isWorkingDirectoryClean, branchExists, remoteBranchExists, buildBranchName, createBranch, pushBranch, getRepositoryUrl, isBranchType } from "./lib/git";
import { validateStoryId, validateTaskId, normalizeId } from "./lib/validation";
import { parseArgs, exitWithError, exitWithUsage, printSuccess, printInfo } from "./lib/cli";
import { BRANCH_TYPES, BASE_BRANCH_MAP, BranchType } from "./lib/types";

const USAGE = `
Usage: create-branch.ts --type <type> --story <SPWS-X> [--task <SPWT-X>]

Options:
  -t, --type <type>     Branch type: ${BRANCH_TYPES.join(", ")}
  -s, --story <id>      Story ID (e.g., SPWS-4)
  -T, --task <id>       Task ID (e.g., SPWT-13) - optional for story branches
      --no-push         Don't push to remote after creation
  -h, --help            Show this help

Examples:
  create-branch.ts --type feature --story SPWS-4
  create-branch.ts --type tech --story SPWS-4 --task SPWT-13
  create-branch.ts tech SPWS-4 SPWT-13
`;

const ARG_DEFINITIONS = [
    { flags: ["-t", "--type"], hasValue: true },
    { flags: ["-s", "--story"], hasValue: true },
    { flags: ["-T", "--task"], hasValue: true },
    { flags: ["--no-push"], isBoolean: true },
    { flags: ["-h", "--help"], isBoolean: true },
];

function inferFromPositional(positional: string[]): { type?: string; storyId?: string; taskId?: string } {
    const result: { type?: string; storyId?: string; taskId?: string } = {};

    for (const arg of positional) {
        if (!result.type && isBranchType(arg.toLowerCase())) {
            result.type = arg.toLowerCase();
        } else if (!result.storyId && /^SPWS-\d+$/i.test(arg)) {
            result.storyId = arg;
        } else if (!result.taskId && /^SPWT-\d+$/i.test(arg)) {
            result.taskId = arg;
        }
    }

    return result;
}

function main(): void {
    const args = process.argv.slice(2);

    if (args.length === 0) {
        exitWithUsage(USAGE);
    }

    const parsed = parseArgs(args, ARG_DEFINITIONS);

    if (parsed.options["help"]) {
        exitWithUsage(USAGE);
    }

    const inferred = inferFromPositional(parsed.positional);

    const typeValue = (parsed.options["type"] as string) || inferred.type;
    const storyValue = (parsed.options["story"] as string) || inferred.storyId;
    const taskValue = (parsed.options["task"] as string) || inferred.taskId;
    const noPush = parsed.options["no-push"] === true;

    if (!typeValue) {
        exitWithError(`Branch type is required. Valid types: ${BRANCH_TYPES.join(", ")}`);
    }

    if (!isBranchType(typeValue)) {
        exitWithError(`Invalid branch type '${typeValue}'. Valid types: ${BRANCH_TYPES.join(", ")}`);
    }

    if (!storyValue) {
        exitWithError("Story ID is required (e.g., SPWS-4)");
    }

    const storyId = normalizeId(storyValue);
    if (!validateStoryId(storyId)) {
        exitWithError("Invalid story ID format. Must be SPWS-X");
    }

    let taskId: string | undefined;
    if (taskValue) {
        taskId = normalizeId(taskValue);
        if (!validateTaskId(taskId)) {
            exitWithError("Invalid task ID format. Must be SPWT-X");
        }
    }

    if (!isWorkingDirectoryClean()) {
        exitWithError("Working directory is not clean. Commit or stash your changes first.");
    }

    const branchType = typeValue as BranchType;
    const baseBranch = BASE_BRANCH_MAP[branchType];
    const branchName = buildBranchName(branchType, storyId, taskId);

    if (branchExists(branchName)) {
        exitWithError(`Branch '${branchName}' already exists locally`);
    }

    if (remoteBranchExists(branchName)) {
        exitWithError(`Branch '${branchName}' already exists on remote`);
    }

    createBranch(branchName, baseBranch);

    if (!noPush) {
        pushBranch(branchName);
    }

    printSuccess(`Branch created: ${branchName}`);

    const repoUrl = getRepositoryUrl();
    if (repoUrl) {
        printInfo("URL", `${repoUrl}/tree/${branchName}`);
    }
}

main();
