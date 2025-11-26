export interface ArgDefinition {
    flags: string[];
    hasValue?: boolean;
    isBoolean?: boolean;
}

export interface ParsedArgs {
    positional: string[];
    options: Record<string, string | boolean>;
}

export function parseArgs(args: string[], definitions: ArgDefinition[]): ParsedArgs {
    const result: ParsedArgs = {
        positional: [],
        options: {},
    };

    const flagMap = new Map<string, ArgDefinition>();
    for (const def of definitions) {
        for (const flag of def.flags) {
            flagMap.set(flag, def);
        }
    }

    let i = 0;
    while (i < args.length) {
        const arg = args[i];
        const def = flagMap.get(arg);

        if (def) {
            const key = def.flags[def.flags.length - 1].replace(/^--?/, "");

            if (def.isBoolean) {
                result.options[key] = true;
            } else if (def.hasValue !== false) {
                const value = args[++i];
                if (value !== undefined) {
                    result.options[key] = value;
                }
            }
        } else if (!arg.startsWith("-")) {
            result.positional.push(arg);
        }

        i++;
    }

    return result;
}

export function exitWithError(message: string, code = 1): never {
    console.error(`Error: ${message}`);
    process.exit(code);
}

export function exitWithUsage(usage: string): never {
    console.log(usage);
    process.exit(0);
}

export function printSuccess(message: string): void {
    console.log(`\n✓ ${message}`);
}

export function printInfo(label: string, value: string): void {
    console.log(`  ${label}: ${value}`);
}

export function printSeparator(char = "─", length = 50): void {
    console.log(char.repeat(length));
}
