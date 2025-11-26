#!/usr/bin/env npx ts-node

import { getCurrentBranch, parseBranchName, hasStagedChanges, stageAll, commit } from "./lib/git";
import { validateSubject, validateStoryId, validateTaskId, isValidCommitType } from "./lib/validation";
import { parseArgs, exitWithError, exitWithUsage, printSuccess, printSeparator } from "./lib/cli";
import { COMMIT_TYPES, CommitType, CommitConfig } from "./lib/types";

const USAGE = `
Usage: create-commit.ts <type> "<subject>" [options]

Arguments:
  type                  Commit type: ${COMMIT_TYPES.join(", ")}
  subject               Commit subject (imperative, lowercase, no period)

Options:
  -s, --scope <scope>      Scope (e.g., collision, components)
  -b, --body <body>        Commit body (multi-line supported)
  -i, --implements <id>    Implements task (SPWT-X)
  -p, --part-of <id>       Part of story (SPWS-X)
  -r, --related <ids>      Related items (comma-separated)
  -a, --stage-all          Stage all changes before commit
      --no-auto-footers    Don't auto-detect footers from branch
  -h, --help               Show this help

Footers are auto-detected from branch name unless --implements or --part-of is specified.

Examples:
  create-commit.ts feat "add collision detection"
  create-commit.ts feat "add aoe system" --scope collision
  create-commit.ts feat "add aoe system" --implements SPWT-13 --part-of SPWS-4
`;

const ARG_DEFINITIONS = [
    { flags: ["-s", "--scope"], hasValue: true },
    { flags: ["-b", "--body"], hasValue: true },
    { flags: ["-i", "--implements"], hasValue: true },
    { flags: ["-p", "--part-of"], hasValue: true },
    { flags: ["-r", "--related"], hasValue: true },
    { flags: ["-a", "--stage-all"], isBoolean: true },
    { flags: ["--no-auto-footers"], isBoolean: true },
    { flags: ["-h", "--help"], isBoolean: true },
];

function buildCommitMessage(config: CommitConfig): string {
    let message = config.type;

    if (config.scope) {
        message += `(${config.scope})`;
    }

    message += `: ${config.subject}`;

    if (config.body) {
        message += `\n\n${config.body}`;
    }

    const footers: string[] = [];
    if (config.implements) {
        footers.push(`Implements: ${config.implements.toUpperCase()}`);
    }
    if (config.partOf) {
        footers.push(`Part of: ${config.partOf.toUpperCase()}`);
    }
    if (config.related) {
        footers.push(`Related: ${config.related.toUpperCase()}`);
    }

    if (footers.length > 0) {
        message += "\n\n" + footers.join("\n");
    }

    return message;
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

    const [typeArg, subjectArg] = parsed.positional;

    if (!typeArg) {
        exitWithError(`Commit type is required. Valid types: ${COMMIT_TYPES.join(", ")}`);
    }

    if (!isValidCommitType(typeArg)) {
        exitWithError(`Invalid commit type '${typeArg}'. Valid types: ${COMMIT_TYPES.join(", ")}`);
    }

    if (!subjectArg) {
        exitWithError("Commit subject is required");
    }

    const subjectValidation = validateSubject(subjectArg);
    if (!subjectValidation.valid) {
        exitWithError(subjectValidation.error!);
    }

    if (parsed.options["stage-all"]) {
        stageAll();
        console.log("Staged all changes");
    }

    if (!hasStagedChanges()) {
        exitWithError("No staged changes to commit. Stage changes with 'git add' or use --stage-all");
    }

    const implementsArg = parsed.options["implements"] as string | undefined;
    const partOfArg = parsed.options["part-of"] as string | undefined;
    const noAutoFooters = parsed.options["no-auto-footers"] === true;

    if (implementsArg && !validateTaskId(implementsArg.toUpperCase())) {
        exitWithError("Invalid implements ID. Must be SPWT-X");
    }

    if (partOfArg && !validateStoryId(partOfArg.toUpperCase())) {
        exitWithError("Invalid part-of ID. Must be SPWS-X");
    }

    let implementsId = implementsArg;
    let partOfId = partOfArg;

    const shouldAutoDetect = !noAutoFooters && !implementsArg && !partOfArg;
    if (shouldAutoDetect) {
        const branch = getCurrentBranch();
        const branchInfo = parseBranchName(branch);

        if (branchInfo) {
            implementsId = branchInfo.taskId;
            partOfId = branchInfo.storyId;

            if (implementsId || partOfId) {
                console.log("Auto-detected from branch:");
                if (implementsId) console.log(`  Implements: ${implementsId}`);
                if (partOfId) console.log(`  Part of: ${partOfId}`);
            }
        }
    }

    const config: CommitConfig = {
        type: typeArg.toLowerCase() as CommitType,
        scope: parsed.options["scope"] as string | undefined,
        subject: subjectArg,
        body: parsed.options["body"] as string | undefined,
        implements: implementsId,
        partOf: partOfId,
        related: parsed.options["related"] as string | undefined,
    };

    const message = buildCommitMessage(config);

    console.log("\nCommit message:");
    printSeparator();
    console.log(message);
    printSeparator();

    commit(message);

    printSuccess("Commit created successfully");
}

main();
