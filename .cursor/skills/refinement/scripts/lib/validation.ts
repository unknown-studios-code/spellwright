import { ID_PATTERNS, ARTIFACT_TYPES, TASK_TYPES, MOSCOW_PRIORITIES, BUG_SEVERITIES, ArtifactType, TaskType, MoscowPriority, BugSeverity, ValidationResult } from "./types";

// ID Validation
export function validateEpicId(id: string): ValidationResult {
    const normalized = normalizeId(id);
    if (!ID_PATTERNS.epic.test(normalized)) {
        return { valid: false, error: `Invalid Epic ID format. Expected SPWE-X, got: ${id}` };
    }
    return { valid: true };
}

export function validateStoryId(id: string): ValidationResult {
    const normalized = normalizeId(id);
    if (!ID_PATTERNS.story.test(normalized)) {
        return { valid: false, error: `Invalid Story ID format. Expected SPWS-X, got: ${id}` };
    }
    return { valid: true };
}

export function validateTaskId(id: string): ValidationResult {
    const normalized = normalizeId(id);
    if (!ID_PATTERNS.task.test(normalized)) {
        return { valid: false, error: `Invalid Task ID format. Expected SPWT-X, got: ${id}` };
    }
    return { valid: true };
}

export function validateArtifactId(id: string, type: ArtifactType): ValidationResult {
    switch (type) {
        case "epic":
            return validateEpicId(id);
        case "story":
            return validateStoryId(id);
        case "task":
            return validateTaskId(id);
        default:
            return { valid: false, error: `Unknown artifact type: ${type}` };
    }
}

// Normalize ID (uppercase, trim)
export function normalizeId(id: string): string {
    return id.trim().toUpperCase();
}

// Extract numeric part from ID
export function extractIdNumber(id: string): number | null {
    const match = id.match(/\d+$/);
    return match ? parseInt(match[0], 10) : null;
}

// Type Validation
export function isArtifactType(value: string): value is ArtifactType {
    return ARTIFACT_TYPES.includes(value.toLowerCase() as ArtifactType);
}

export function isTaskType(value: string): value is TaskType {
    return TASK_TYPES.includes(value.toLowerCase() as TaskType);
}

export function isMoscowPriority(value: string): value is MoscowPriority {
    return MOSCOW_PRIORITIES.includes(value as MoscowPriority);
}

export function isBugSeverity(value: string): value is BugSeverity {
    return BUG_SEVERITIES.includes(value as BugSeverity);
}

// Validate Priority based on artifact type
export function validatePriority(priority: string, taskType?: TaskType): ValidationResult {
    if (taskType === "bug") {
        if (!isBugSeverity(priority)) {
            return {
                valid: false,
                error: `Invalid Bug Severity. Expected one of: ${BUG_SEVERITIES.join(", ")}. Got: ${priority}`,
            };
        }
    } else {
        if (!isMoscowPriority(priority)) {
            return {
                valid: false,
                error: `Invalid Priority. Expected one of: ${MOSCOW_PRIORITIES.join(", ")}. Got: ${priority}`,
            };
        }
    }
    return { valid: true };
}

// Validate Required Fields
export function validateRequired(value: unknown, fieldName: string): ValidationResult {
    if (value === undefined || value === null || value === "") {
        return { valid: false, error: `${fieldName} is required` };
    }
    if (Array.isArray(value) && value.length === 0) {
        return { valid: false, error: `${fieldName} must not be empty` };
    }
    return { valid: true };
}

// Validate String Length
export function validateStringLength(value: string, fieldName: string, min: number, max: number): ValidationResult {
    if (value.length < min) {
        return { valid: false, error: `${fieldName} must be at least ${min} characters` };
    }
    if (value.length > max) {
        return { valid: false, error: `${fieldName} must be at most ${max} characters` };
    }
    return { valid: true };
}

// Validate Array Length
export function validateArrayLength(value: unknown[], fieldName: string, min: number, max?: number): ValidationResult {
    if (value.length < min) {
        return { valid: false, error: `${fieldName} must have at least ${min} items` };
    }
    if (max !== undefined && value.length > max) {
        return { valid: false, error: `${fieldName} must have at most ${max} items` };
    }
    return { valid: true };
}

// Combine multiple validation results
export function combineValidations(...results: ValidationResult[]): ValidationResult {
    for (const result of results) {
        if (!result.valid) {
            return result;
        }
    }
    return { valid: true };
}

// Validate Notion Page URL
export function validateNotionUrl(url: string): ValidationResult {
    if (!url.startsWith("https://www.notion.so/") && !url.startsWith("https://notion.so/")) {
        return { valid: false, error: `Invalid Notion URL: ${url}` };
    }
    return { valid: true };
}

// Validate UUID format
export function validateUuid(id: string): ValidationResult {
    const uuidPattern = /^[0-9a-f]{8}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{12}$/i;
    if (!uuidPattern.test(id)) {
        return { valid: false, error: `Invalid UUID format: ${id}` };
    }
    return { valid: true };
}

// Extract page ID from Notion URL
export function extractPageIdFromUrl(url: string): string | null {
    // URL format: https://www.notion.so/workspace/Page-Title-<32-char-id>
    // or: https://www.notion.so/<32-char-id>
    const match = url.match(/([0-9a-f]{32})(?:\?|$)/i);
    if (match) {
        return match[1];
    }
    // Try with dashes
    const uuidMatch = url.match(/([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})(?:\?|$)/i);
    if (uuidMatch) {
        return uuidMatch[1].replace(/-/g, "");
    }
    return null;
}
