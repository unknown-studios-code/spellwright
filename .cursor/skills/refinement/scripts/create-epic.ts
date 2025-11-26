#!/usr/bin/env npx ts-node

/**
 * Create Epic Script
 *
 * Generates Epic content and outputs the MCP call parameters for Notion creation.
 * This script helps structure the data; the actual MCP call is made by the agent.
 *
 * Usage:
 *   npx ts-node create-epic.ts --name "Epic Name" --priority High
 *
 * The script outputs JSON that can be used with the Notion MCP.
 */

import { parseArgs, exitWithError, exitWithUsage, printHeader, printJson, printInfo } from "./lib/cli";
import { isMoscowPriority, validateRequired } from "./lib/validation";
import { buildEpicProperties, getDataSourceId, NOTION_SERVER, NOTION_TOOLS } from "./lib/notion";
import { generateEpicContent } from "./lib/templates";
import { EpicData, MoscowPriority, MOSCOW_PRIORITIES } from "./lib/types";

const USAGE = `
Create Epic - Generate Epic for Notion

Usage: create-epic.ts --name <name> --priority <priority> [options]

Required:
  -n, --name <name>         Epic name/title
  -p, --priority <priority> Priority: ${MOSCOW_PRIORITIES.join(", ")}

Optional:
  --strategic-goal <text>   Strategic goal description
  --problem <text>          Problem statement
  --target-user <text>      Target user/persona
  --output-json             Output full JSON for MCP call
  -h, --help                Show this help

Example:
  create-epic.ts --name "User Authentication System" --priority High
`;

const ARG_DEFINITIONS = [
    { flags: ["-n", "--name"], hasValue: true },
    { flags: ["-p", "--priority"], hasValue: true },
    { flags: ["--strategic-goal"], hasValue: true },
    { flags: ["--problem"], hasValue: true },
    { flags: ["--target-user"], hasValue: true },
    { flags: ["--output-json"], isBoolean: true },
    { flags: ["-h", "--help"], isBoolean: true },
];

function main(): void {
    const args = process.argv.slice(2);

    if (args.length === 0 || args.includes("-h") || args.includes("--help")) {
        exitWithUsage(USAGE);
    }

    const parsed = parseArgs(args, ARG_DEFINITIONS);

    const name = parsed.options["name"] as string;
    const priority = parsed.options["priority"] as string;
    const strategicGoal = parsed.options["strategic-goal"] as string;
    const problem = parsed.options["problem"] as string;
    const targetUser = parsed.options["target-user"] as string;
    const outputJson = parsed.options["output-json"] === true;

    // Validate required fields
    const nameValidation = validateRequired(name, "Epic name");
    if (!nameValidation.valid) {
        exitWithError(nameValidation.error!);
    }

    const priorityValidation = validateRequired(priority, "Priority");
    if (!priorityValidation.valid) {
        exitWithError(priorityValidation.error!);
    }

    if (!isMoscowPriority(priority)) {
        exitWithError(`Invalid priority. Must be one of: ${MOSCOW_PRIORITIES.join(", ")}`);
    }

    // Build Epic data structure (with placeholders for sections that need user input)
    const epicData: EpicData = {
        name,
        priority: priority as MoscowPriority,
        strategicGoal: strategicGoal || "[STRATEGIC GOAL - To be filled by agent]",
        problemStatement: {
            problem: problem || "[PROBLEM STATEMENT - To be filled by agent]",
            targetUser: targetUser || "[TARGET USER - To be filled by agent]",
        },
        valueHypothesis: {
            action: "[ACTION - To be filled by agent]",
            outcome: "[OUTCOME - To be filled by agent]",
            measurement: "[MEASUREMENT - To be filled by agent]",
        },
        scope: {
            inScope: ["[IN SCOPE ITEMS - To be filled by agent]"],
            outOfScope: ["[OUT OF SCOPE ITEMS - To be filled by agent]"],
        },
        successCriteria: {
            quantitative: ["[QUANTITATIVE METRICS - To be filled by agent]"],
            qualitative: ["[QUALITATIVE CRITERIA - To be filled by agent]"],
        },
        risks: [
            {
                category: "technical",
                description: "[RISK DESCRIPTION - To be filled by agent]",
                mitigation: "[MITIGATION - To be filled by agent]",
            },
        ],
        references: {
            documentation: [{ title: "[DOC TITLE]", url: "[URL]" }],
            externalResources: [{ title: "[RESOURCE TITLE]", url: "[URL]" }],
        },
    };

    // Generate content
    const content = generateEpicContent(epicData);

    // Build properties
    const properties = buildEpicProperties(name, priority);

    // Build MCP call parameters
    const mcpParams = {
        parent: {
            data_source_id: getDataSourceId("epics"),
        },
        pages: [
            {
                properties,
                content,
            },
        ],
    };

    if (outputJson) {
        printHeader("MCP Call Parameters");
        printInfo("Server", NOTION_SERVER);
        printInfo("Tool", NOTION_TOOLS.createPages);
        console.log("\nArguments:");
        printJson(mcpParams);
    } else {
        printHeader("Epic Created Successfully");
        printInfo("Name", name);
        printInfo("Priority", priority);
        printInfo("Data Source", getDataSourceId("epics"));

        console.log("\n--- Generated Content Preview ---\n");
        console.log(content.substring(0, 500) + "...\n");

        console.log("--- MCP Call ---");
        console.log(`Server: ${NOTION_SERVER}`);
        console.log(`Tool: ${NOTION_TOOLS.createPages}`);
        console.log("\nUse --output-json for full parameters.");
    }
}

main();
