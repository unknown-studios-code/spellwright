#!/usr/bin/env npx ts-node

import * as fs from "fs";
import { getCurrentBranch, parseBranchName, hasUnpushedCommits, pushBranch, getBaseBranch, execOrThrow } from "./lib/git";
import { validatePrTitle } from "./lib/validation";
import { parseArgs, exitWithError, exitWithUsage, printSuccess, printInfo } from "./lib/cli";
import { PrConfig } from "./lib/types";

const USAGE = `
Usage: create-pr.ts --title "<title>" [options]

Options:
  -t, --title <title>      PR title (required)
  -b, --body <body>        PR body content
  -B, --body-file <path>   Read PR body from file
      --base <branch>      Base branch (default: auto-detect)
  -l, --label <label>      Add label (can be repeated)
  -d, --draft              Create as draft PR
  -h, --help               Show this help

The script auto-pushes unpushed commits before creating the PR.

Examples:
  create-pr.ts --title "feat(collision): add aoe detection"
  create-pr.ts --title "feat(collision): add aoe detection" \\
    --label "type: feature" --label "priority: medium"
  create-pr.ts --title "feat(collision): add aoe detection" \\
    --body-file /tmp/pr-body.md --label "type: feature"
`;

const ARG_DEFINITIONS = [
    { flags: ["-t", "--title"], hasValue: true },
    { flags: ["-b", "--body"], hasValue: true },
    { flags: ["-B", "--body-file"], hasValue: true },
    { flags: ["--base"], hasValue: true },
    { flags: ["-l", "--label"], hasValue: true },
    { flags: ["-d", "--draft"], isBoolean: true },
    { flags: ["-h", "--help"], isBoolean: true },
];

function parseArgsWithLabels(args: string[]): {
    title?: string;
    body?: string;
    bodyFile?: string;
    base?: string;
    labels: string[];
    draft: boolean;
} {
    const result = {
        title: undefined as string | undefined,
        body: undefined as string | undefined,
        bodyFile: undefined as string | undefined,
        base: undefined as string | undefined,
        labels: [] as string[],
        draft: false,
    };

    let i = 0;
    while (i < args.length) {
        const arg = args[i];

        if (arg === "-t" || arg === "--title") {
            result.title = args[++i];
        } else if (arg === "-b" || arg === "--body") {
            result.body = args[++i];
        } else if (arg === "-B" || arg === "--body-file") {
            result.bodyFile = args[++i];
        } else if (arg === "--base") {
            result.base = args[++i];
        } else if (arg === "-l" || arg === "--label") {
            result.labels.push(args[++i]);
        } else if (arg === "-d" || arg === "--draft") {
            result.draft = true;
        } else if (arg === "-h" || arg === "--help") {
            exitWithUsage(USAGE);
        } else if (!arg.startsWith("-") && !result.title) {
            result.title = arg;
        }

        i++;
    }

    return result;
}

function generateDefaultBody(): string {
    const branch = getCurrentBranch();
    const branchInfo = parseBranchName(branch);

    const taskId = branchInfo?.taskId || branchInfo?.storyId || "SPWT-X";
    const storyId = branchInfo?.storyId || "SPWS-X";

    let taskType = "Feature";
    if (branchInfo?.type === "tech") taskType = "Tech";
    else if (branchInfo?.type === "bug") taskType = "Bug";

    return `## 📋 Task: ${taskId}

**Story:** ${storyId}
**Type:** ${taskType}
**Priority:** Medium

---

## 🎯 What Changed

(Description of changes)

---

## 📁 Files Created/Modified

(List of files)

---

## ✅ Definition of Done

- [ ] (DoD items)

---

## 🔗 References

- **Notion Task:** [${taskId}](notion://task)
- **Story:** [${storyId}](notion://story)`;
}

function createPullRequest(config: PrConfig): string {
    const labelArgs = config.labels.map((l) => `--label "${l}"`).join(" ");
    const draftArg = config.draft ? "--draft" : "";

    const escapedBody = config.body.replace(/\\/g, "\\\\").replace(/'/g, "'\\''").replace(/\$/g, "\\$");

    const command = `gh pr create --title "${config.title}" --body $'${escapedBody}' --base ${config.baseBranch} ${labelArgs} ${draftArg}`.trim();

    return execOrThrow(command, { silent: true });
}

function main(): void {
    const args = process.argv.slice(2);

    if (args.length === 0) {
        exitWithUsage(USAGE);
    }

    const parsed = parseArgsWithLabels(args);

    if (!parsed.title) {
        exitWithError("PR title is required");
    }

    const titleValidation = validatePrTitle(parsed.title);
    if (!titleValidation.valid) {
        console.warn(`Warning: ${titleValidation.error}`);
    }

    if (hasUnpushedCommits()) {
        const branch = getCurrentBranch();
        pushBranch(branch);
    }

    const currentBranch = getCurrentBranch();
    const baseBranch = parsed.base || getBaseBranch(currentBranch);

    let body = parsed.body;

    if (parsed.bodyFile) {
        try {
            body = fs.readFileSync(parsed.bodyFile, "utf-8");
        } catch {
            exitWithError(`Cannot read body file: ${parsed.bodyFile}`);
        }
    }

    if (!body) {
        body = generateDefaultBody();
    }

    console.log("\nCreating PR:");
    printInfo("Title", parsed.title);
    printInfo("Base", baseBranch);
    printInfo("Draft", String(parsed.draft));
    printInfo("Labels", parsed.labels.length > 0 ? parsed.labels.join(", ") : "(none)");

    const config: PrConfig = {
        title: parsed.title,
        body,
        baseBranch,
        labels: parsed.labels,
        draft: parsed.draft,
    };

    const prUrl = createPullRequest(config);

    printSuccess("Pull Request created");
    printInfo("URL", prUrl);
}

main();
