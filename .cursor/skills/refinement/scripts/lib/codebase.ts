import { execSync } from "child_process";
import * as fs from "fs";
import * as path from "path";

/**
 * Codebase Analysis Module
 *
 * Provides functions to analyze the Unity project structure and suggest
 * file paths for new components, systems, and other artifacts.
 */

// Base paths for Unity project
const SCRIPTS_BASE = "Assets/Scripts";

// Path mappings for different component types
const PATH_MAPPINGS: Record<string, string> = {
    // Components
    component: `${SCRIPTS_BASE}/Components`,
    generator: `${SCRIPTS_BASE}/Components/Generators`,
    modifier: `${SCRIPTS_BASE}/Components/Modifiers`,
    payload: `${SCRIPTS_BASE}/Components/Payloads`,
    statusEffect: `${SCRIPTS_BASE}/Components/StatusEffect`,

    // Systems
    system: `${SCRIPTS_BASE}/Systems`,
    generatorSystem: `${SCRIPTS_BASE}/Systems/Generators`,
    modifierSystem: `${SCRIPTS_BASE}/Systems/Modifiers`,
    payloadSystem: `${SCRIPTS_BASE}/Systems/Payloads`,
    collisionSystem: `${SCRIPTS_BASE}/Systems/Collision`,
    movementSystem: `${SCRIPTS_BASE}/Systems/Movement`,
    lifetimeSystem: `${SCRIPTS_BASE}/Systems/Lifetime`,
    statusEffectSystem: `${SCRIPTS_BASE}/Systems/StatusEffect`,

    // Jobs
    job: `${SCRIPTS_BASE}/Jobs`,
    statusEffectJob: `${SCRIPTS_BASE}/Jobs/StatusEffect`,

    // ScriptableObjects
    scriptableObject: `${SCRIPTS_BASE}/ScriptableObjects`,
    spellDefinition: `${SCRIPTS_BASE}/ScriptableObjects/Spells`,

    // Enums
    enum: `${SCRIPTS_BASE}/ScriptableObjects/Enums`,

    // Tests
    test: "Assets/Tests",
    editModeTest: "Assets/Tests/EditMode",
    playModeTest: "Assets/Tests/PlayMode",
};

/**
 * Get the base path for a given component type
 */
export function getBasePath(componentType: string): string {
    const key = componentType.toLowerCase().replace(/\s+/g, "");
    return PATH_MAPPINGS[key] || `${SCRIPTS_BASE}/${componentType}`;
}

/**
 * Suggest a file path for a new script
 */
export function suggestFilePath(name: string, type: string, subFolder?: string): string {
    const basePath = getBasePath(type);
    const fileName = name.endsWith(".cs") ? name : `${name}.cs`;

    if (subFolder) {
        return `${basePath}/${subFolder}/${fileName}`;
    }
    return `${basePath}/${fileName}`;
}

/**
 * Check if a file exists in the project
 */
export function fileExists(filePath: string): boolean {
    try {
        return fs.existsSync(filePath);
    } catch {
        return false;
    }
}

/**
 * Find files matching a glob pattern using ripgrep
 */
export function findFiles(pattern: string, directory: string = "."): string[] {
    try {
        const result = execSync(`find "${directory}" -type f -name "${pattern}" 2>/dev/null`, {
            encoding: "utf-8",
            maxBuffer: 10 * 1024 * 1024,
        });
        return result
            .trim()
            .split("\n")
            .filter((line) => line.length > 0);
    } catch {
        return [];
    }
}

/**
 * Find files containing a specific pattern using ripgrep
 */
export function findFilesWithContent(searchPattern: string, filePattern: string = "*.cs"): string[] {
    try {
        const result = execSync(`rg -l "${searchPattern}" --glob "${filePattern}" 2>/dev/null`, {
            encoding: "utf-8",
            maxBuffer: 10 * 1024 * 1024,
        });
        return result
            .trim()
            .split("\n")
            .filter((line) => line.length > 0);
    } catch {
        return [];
    }
}

/**
 * Get existing components in a folder
 */
export function getExistingFiles(folderPath: string): string[] {
    try {
        if (!fs.existsSync(folderPath)) {
            return [];
        }
        return fs.readdirSync(folderPath).filter((file) => file.endsWith(".cs") && !file.endsWith(".meta"));
    } catch {
        return [];
    }
}

/**
 * Extract class/struct names from a C# file
 */
export function extractClassNames(filePath: string): string[] {
    try {
        const content = fs.readFileSync(filePath, "utf-8");
        const classPattern = /(?:public|internal|private)?\s*(?:partial\s+)?(?:class|struct|interface|enum)\s+(\w+)/g;
        const matches: string[] = [];
        let match;

        while ((match = classPattern.exec(content)) !== null) {
            matches.push(match[1]);
        }

        return matches;
    } catch {
        return [];
    }
}

/**
 * Get the namespace for a given file path
 */
export function inferNamespace(filePath: string): string {
    // Convert path to namespace
    // e.g., Assets/Scripts/Components/Generators/Projectile -> Spellwright.Components.Generators.Projectile
    const relativePath = filePath.replace(/^Assets\/Scripts\//, "").replace(/\/[^/]+\.cs$/, "");

    return `Spellwright.${relativePath.replace(/\//g, ".")}`;
}

/**
 * Suggest related files based on a component/system name
 */
export function suggestRelatedFiles(name: string): { type: string; path: string }[] {
    const suggestions: { type: string; path: string }[] = [];
    const baseName = name.replace(/(?:Component|System|Job|Data|Config|Tag)$/, "");

    // If it's a system, suggest the component
    if (name.endsWith("System")) {
        suggestions.push({
            type: "Component",
            path: suggestFilePath(`${baseName}Data`, "component"),
        });
        suggestions.push({
            type: "Job",
            path: suggestFilePath(`${baseName}Job`, "job"),
        });
    }

    // If it's a component, suggest the system
    if (name.endsWith("Data") || name.endsWith("Component")) {
        suggestions.push({
            type: "System",
            path: suggestFilePath(`${baseName}System`, "system"),
        });
    }

    // Always suggest a test file
    suggestions.push({
        type: "Test",
        path: suggestFilePath(`${name}Tests`, "editModeTest"),
    });

    return suggestions;
}

/**
 * Get project structure summary
 */
export function getProjectStructure(): Record<string, string[]> {
    const structure: Record<string, string[]> = {};

    for (const [type, basePath] of Object.entries(PATH_MAPPINGS)) {
        if (fs.existsSync(basePath)) {
            structure[type] = getExistingFiles(basePath);
        }
    }

    return structure;
}

/**
 * Find existing similar components/systems
 */
export function findSimilar(name: string, type: string): string[] {
    const basePath = getBasePath(type);
    const files = getExistingFiles(basePath);

    // Simple similarity based on common words
    const words = name
        .toLowerCase()
        .split(/(?=[A-Z])/)
        .map((w) => w.toLowerCase());

    return files.filter((file) => {
        const fileWords = file
            .replace(".cs", "")
            .split(/(?=[A-Z])/)
            .map((w) => w.toLowerCase());
        return words.some((word) => fileWords.includes(word));
    });
}

/**
 * Analyze existing code to extract patterns
 */
export function analyzeExistingCode(filePaths: string[]): {
    components: string[];
    systems: string[];
    interfaces: string[];
} {
    const result = {
        components: [] as string[],
        systems: [] as string[],
        interfaces: [] as string[],
    };

    for (const filePath of filePaths) {
        try {
            const content = fs.readFileSync(filePath, "utf-8");

            // Find IComponentData implementations
            if (content.includes("IComponentData") || content.includes("IBufferElementData")) {
                const classes = extractClassNames(filePath);
                result.components.push(...classes);
            }

            // Find ISystem implementations
            if (content.includes("ISystem") || content.includes("SystemBase")) {
                const classes = extractClassNames(filePath);
                result.systems.push(...classes);
            }

            // Find interfaces
            const interfacePattern = /interface\s+(\w+)/g;
            let match;
            while ((match = interfacePattern.exec(content)) !== null) {
                result.interfaces.push(match[1]);
            }
        } catch {
            // Skip files that can't be read
        }
    }

    return result;
}
