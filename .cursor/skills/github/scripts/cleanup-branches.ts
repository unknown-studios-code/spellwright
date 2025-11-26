#!/usr/bin/env npx ts-node

import { getCurrentBranch, fetchAndPrune, getMergedLocalBranches, getMergedRemoteBranches, getBranchLastCommitAge, deleteLocalBranch, deleteRemoteBranch } from "./lib/git";
import { getMergedPRForBranch } from "./lib/github";
import { parseArgs, exitWithUsage, printSuccess, printInfo, printSeparator } from "./lib/cli";
import { MergedBranchInfo, CleanupConfig, PROTECTED_BRANCHES } from "./lib/types";

const USAGE = `
Usage: cleanup-branches.ts [options]

Deletes local and remote branches that have been merged via PR.

Options:
  -y, --yes             Skip confirmation prompt (required for non-interactive use)
      --dry-run         Show what would be deleted without deleting
      --local-only      Only delete local branches
      --remote-only     Only delete remote branches
      --include <regex> Only include branches matching regex
      --exclude <regex> Exclude branches matching regex
      --skip-pr-check   Delete branches even without a merged PR
  -h, --help            Show this help

Protected branches (main, master, develop) are never deleted.
By default, only branches with a merged PR are deleted.

Examples:
  cleanup-branches.ts --dry-run          # Preview deletions
  cleanup-branches.ts -y                 # Auto-confirm deletions
  cleanup-branches.ts --include "SPWT-"  # Only task branches
  cleanup-branches.ts --skip-pr-check -y # Skip PR verification
`;

const ARG_DEFINITIONS = [
    { flags: ["-y", "--yes"], isBoolean: true },
    { flags: ["--dry-run"], isBoolean: true },
    { flags: ["--local-only"], isBoolean: true },
    { flags: ["--remote-only"], isBoolean: true },
    { flags: ["--include"], hasValue: true },
    { flags: ["--exclude"], hasValue: true },
    { flags: ["--skip-pr-check"], isBoolean: true },
    { flags: ["-h", "--help"], isBoolean: true },
];

function filterByPattern(branches: string[], include?: string, exclude?: string): string[] {
    let filtered = branches;

    if (include) {
        const regex = new RegExp(include);
        filtered = filtered.filter((b) => regex.test(b));
    }

    if (exclude) {
        const regex = new RegExp(exclude);
        filtered = filtered.filter((b) => !regex.test(b));
    }

    return filtered;
}

function collectMergedBranches(config: CleanupConfig): MergedBranchInfo[] {
    console.log("Fetching merged branches...");
    fetchAndPrune();

    if (!config.skipPRCheck) {
        console.log("Checking PRs on GitHub (use --skip-pr-check to skip)...");
    }

    const localBranches = config.remoteOnly ? [] : getMergedLocalBranches(PROTECTED_BRANCHES);
    const remoteBranches = config.localOnly ? [] : getMergedRemoteBranches(PROTECTED_BRANCHES);

    const branchMap = new Map<string, MergedBranchInfo>();

    for (const name of localBranches) {
        const prInfo = config.skipPRCheck ? { hasMergedPR: true } : getMergedPRForBranch(name);
        branchMap.set(name, {
            name,
            isLocal: true,
            isRemote: false,
            lastCommitAge: getBranchLastCommitAge(name),
            prNumber: prInfo.prNumber,
            prUrl: prInfo.prUrl,
        });
    }

    for (const name of remoteBranches) {
        const existing = branchMap.get(name);
        if (existing) {
            existing.isRemote = true;
        } else if (!config.localOnly) {
            const prInfo = config.skipPRCheck ? { hasMergedPR: true } : getMergedPRForBranch(name);
            branchMap.set(name, {
                name,
                isLocal: false,
                isRemote: true,
                lastCommitAge: getBranchLastCommitAge(name),
                prNumber: prInfo.prNumber,
                prUrl: prInfo.prUrl,
            });
        }
    }

    return Array.from(branchMap.values());
}

function displayBranches(withPR: MergedBranchInfo[], withoutPR: MergedBranchInfo[]): void {
    if (withPR.length > 0) {
        console.log("\nBranches with merged PR (will be deleted):\n");
        for (const branch of withPR) {
            const location = [branch.isLocal ? "local" : null, branch.isRemote ? "remote" : null].filter(Boolean).join(" + ");
            const prLabel = branch.prNumber ? ` PR #${branch.prNumber}` : "";
            console.log(`  - ${branch.name}${prLabel}`);
            console.log(`    └─ ${branch.lastCommitAge} (${location})`);
        }
    }

    if (withoutPR.length > 0) {
        console.log("\nBranches WITHOUT merged PR (skipped):\n");
        for (const branch of withoutPR) {
            const location = [branch.isLocal ? "local" : null, branch.isRemote ? "remote" : null].filter(Boolean).join(" + ");
            console.log(`  - ${branch.name}`);
            console.log(`    └─ git considers merged, but no PR found (${location})`);
        }
        console.log("\n  Use --skip-pr-check to delete these branches too.");
    }

    console.log();
}

function deleteBranch(branch: MergedBranchInfo, dryRun: boolean): void {
    const prefix = dryRun ? "[DRY-RUN] " : "";

    if (branch.isLocal) {
        console.log(`${prefix}Deleting local: ${branch.name}`);
        if (!dryRun) {
            deleteLocalBranch(branch.name);
        }
    }

    if (branch.isRemote) {
        console.log(`${prefix}Deleting remote: origin/${branch.name}`);
        if (!dryRun) {
            deleteRemoteBranch(branch.name);
        }
    }
}

function main(): void {
    const args = process.argv.slice(2);
    const parsed = parseArgs(args, ARG_DEFINITIONS);

    if (parsed.options["help"] || args.length === 0) {
        exitWithUsage(USAGE);
    }

    const config: CleanupConfig = {
        dryRun: parsed.options["dry-run"] === true,
        localOnly: parsed.options["local-only"] === true,
        remoteOnly: parsed.options["remote-only"] === true,
        skipPRCheck: parsed.options["skip-pr-check"] === true,
        include: parsed.options["include"] as string | undefined,
        exclude: parsed.options["exclude"] as string | undefined,
    };

    const autoConfirm = parsed.options["yes"] === true;

    if (!autoConfirm && !config.dryRun) {
        console.error("Error: Use -y to confirm deletions or --dry-run to preview");
        process.exit(1);
    }

    const currentBranch = getCurrentBranch();
    let branches = collectMergedBranches(config);

    branches = branches.filter((b) => b.name !== currentBranch);

    if (config.include || config.exclude) {
        const filteredNames = filterByPattern(
            branches.map((b) => b.name),
            config.include,
            config.exclude
        );
        branches = branches.filter((b) => filteredNames.includes(b.name));
    }

    const withPR = branches.filter((b) => b.prNumber !== undefined || config.skipPRCheck);
    const withoutPR = branches.filter((b) => b.prNumber === undefined && !config.skipPRCheck);

    if (withPR.length === 0 && withoutPR.length === 0) {
        console.log("\nNo merged branches found to delete.");
        return;
    }

    displayBranches(withPR, withoutPR);

    if (withPR.length === 0) {
        console.log("No branches with merged PR to delete.");
        return;
    }

    printSeparator();
    printInfo("To delete", `${withPR.length} branch(es) with merged PR`);
    if (withoutPR.length > 0) {
        printInfo("Skipped", `${withoutPR.length} branch(es) without merged PR`);
    }
    if (config.dryRun) {
        printInfo("Mode", "DRY-RUN (no changes will be made)");
    }
    printSeparator();

    console.log();

    for (const branch of withPR) {
        deleteBranch(branch, config.dryRun);
    }

    if (config.dryRun) {
        console.log("\n[DRY-RUN] No branches were deleted.");
    } else {
        printSuccess(`${withPR.length} branch(es) deleted`);
    }
}

main();
