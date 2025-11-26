import { ArgDefinition, ParsedArgs } from "./types";

/**
 * CLI Utilities for refinement scripts
 */

// Color codes for terminal output
const COLORS = {
    reset: "\x1b[0m",
    red: "\x1b[31m",
    green: "\x1b[32m",
    yellow: "\x1b[33m",
    blue: "\x1b[34m",
    magenta: "\x1b[35m",
    cyan: "\x1b[36m",
    bold: "\x1b[1m",
    dim: "\x1b[2m",
};

/**
 * Parse command line arguments
 */
export function parseArgs(args: string[], definitions: ArgDefinition[]): ParsedArgs {
    const result: ParsedArgs = {
        options: {},
        positional: [],
    };

    let i = 0;
    while (i < args.length) {
        const arg = args[i];
        let matched = false;

        for (const def of definitions) {
            if (def.flags.includes(arg)) {
                matched = true;
                if (def.isBoolean) {
                    // Find the option name (last flag without dashes)
                    const name = def.flags[def.flags.length - 1].replace(/^-+/, "");
                    result.options[name] = true;
                } else if (def.hasValue) {
                    const name = def.flags[def.flags.length - 1].replace(/^-+/, "");
                    i++;
                    if (i < args.length) {
                        result.options[name] = args[i];
                    }
                }
                break;
            }
        }

        if (!matched && !arg.startsWith("-")) {
            result.positional.push(arg);
        }

        i++;
    }

    return result;
}

/**
 * Print error message and exit
 */
export function exitWithError(message: string): never {
    console.error(`${COLORS.red}Error:${COLORS.reset} ${message}`);
    process.exit(1);
}

/**
 * Print usage and exit
 */
export function exitWithUsage(usage: string): never {
    console.log(usage);
    process.exit(0);
}

/**
 * Print success message
 */
export function printSuccess(message: string): void {
    console.log(`${COLORS.green}✓${COLORS.reset} ${message}`);
}

/**
 * Print info message
 */
export function printInfo(label: string, value: string): void {
    console.log(`${COLORS.cyan}${label}:${COLORS.reset} ${value}`);
}

/**
 * Print warning message
 */
export function printWarning(message: string): void {
    console.log(`${COLORS.yellow}Warning:${COLORS.reset} ${message}`);
}

/**
 * Print header
 */
export function printHeader(title: string): void {
    console.log(`\n${COLORS.bold}${COLORS.blue}${title}${COLORS.reset}`);
    console.log("─".repeat(title.length));
}

/**
 * Print section
 */
export function printSection(title: string): void {
    console.log(`\n${COLORS.bold}${title}${COLORS.reset}`);
}

/**
 * Print list item
 */
export function printListItem(item: string, indent: number = 0): void {
    const padding = "  ".repeat(indent);
    console.log(`${padding}• ${item}`);
}

/**
 * Print key-value pair
 */
export function printKeyValue(key: string, value: string): void {
    console.log(`  ${COLORS.dim}${key}:${COLORS.reset} ${value}`);
}

/**
 * Print JSON object with formatting
 */
export function printJson(obj: unknown): void {
    console.log(JSON.stringify(obj, null, 2));
}

/**
 * Prompt user for confirmation (for interactive mode)
 */
export async function confirm(message: string): Promise<boolean> {
    const readline = await import("readline");
    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout,
    });

    return new Promise((resolve) => {
        rl.question(`${message} (y/n): `, (answer) => {
            rl.close();
            resolve(answer.toLowerCase() === "y" || answer.toLowerCase() === "yes");
        });
    });
}

/**
 * Format Notion URL for display
 */
export function formatNotionUrl(url: string): string {
    return `${COLORS.blue}${url}${COLORS.reset}`;
}

/**
 * Format ID for display
 */
export function formatId(id: string): string {
    return `${COLORS.magenta}${id}${COLORS.reset}`;
}

/**
 * Format file path for display
 */
export function formatPath(filePath: string): string {
    return `${COLORS.cyan}${filePath}${COLORS.reset}`;
}
