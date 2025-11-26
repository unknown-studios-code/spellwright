import type { ValidationResult, CommitType } from "./types";
import { COMMIT_TYPES, BRANCH_TYPES, BranchType } from "./types";

const STORY_ID_REGEX = /^SPWS-\d+$/i;
const TASK_ID_REGEX = /^SPWT-\d+$/i;

const PAST_TENSE_WORDS = ["added", "fixed", "changed", "updated", "removed", "created", "implemented"] as const;

export function validateStoryId(storyId: string): boolean {
    return STORY_ID_REGEX.test(storyId);
}

export function validateTaskId(taskId: string): boolean {
    return TASK_ID_REGEX.test(taskId);
}

export function normalizeId(id: string): string {
    return id.toUpperCase();
}

export function isValidCommitType(value: string): value is CommitType {
    return COMMIT_TYPES.includes(value.toLowerCase() as CommitType);
}

export function isValidBranchType(value: string): value is BranchType {
    return BRANCH_TYPES.includes(value.toLowerCase() as BranchType);
}

export function validateSubject(subject: string): ValidationResult {
    if (!subject) {
        return { valid: false, error: "Subject is required" };
    }

    if (subject.length > 72) {
        return {
            valid: false,
            error: `Subject is too long (${subject.length}/72 characters)`,
        };
    }

    if (subject.endsWith(".")) {
        return { valid: false, error: "Subject should not end with a period" };
    }

    if (subject[0] !== subject[0].toLowerCase()) {
        return { valid: false, error: "Subject should start with lowercase" };
    }

    const firstWord = subject.split(" ")[0].toLowerCase();
    if (PAST_TENSE_WORDS.includes(firstWord as (typeof PAST_TENSE_WORDS)[number])) {
        const suggestion = firstWord.replace(/ied$/, "y").replace(/ed$/, "");
        return {
            valid: false,
            error: `Use imperative mood: "${firstWord}" → "${suggestion}"`,
        };
    }

    return { valid: true };
}

export function validatePrTitle(title: string): ValidationResult {
    if (!title) {
        return { valid: false, error: "PR title is required" };
    }

    if (title.length > 72) {
        return {
            valid: false,
            error: `Title is ${title.length} characters (recommended: <72)`,
        };
    }

    return { valid: true };
}
