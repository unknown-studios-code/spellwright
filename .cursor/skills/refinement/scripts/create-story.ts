#!/usr/bin/env npx ts-node

/**
 * Create Story Script
 *
 * Generates Story content and outputs the MCP call parameters for Notion creation.
 * This script helps structure the data; the actual MCP call is made by the agent.
 *
 * Usage:
 *   npx ts-node create-story.ts --name "Story Name" --priority High --epic SPWE-1
 *
 * The script outputs JSON that can be used with the Notion MCP.
 */

import { parseArgs, exitWithError, exitWithUsage, printHeader, printJson, printInfo } from "./lib/cli";
import { isMoscowPriority, validateRequired, validateEpicId, normalizeId } from "./lib/validation";
import { buildStoryProperties, getDataSourceId, NOTION_SERVER, NOTION_TOOLS } from "./lib/notion";
import { generateStoryContent } from "./lib/templates";
import { StoryData, MoscowPriority, MOSCOW_PRIORITIES } from "./lib/types";

const USAGE = `
Create Story - Generate User Story for Notion

Usage: create-story.ts --name <name> --priority <priority> --epic <epic-id> [options]

Required:
  -n, --name <name>         Story name/title
  -p, --priority <priority> Priority: ${MOSCOW_PRIORITIES.join(", ")}
  -e, --epic <epic-id>      Parent Epic ID (SPWE-X) or Notion URL

Optional:
  --as-a <role>             User role for story (As a...)
  --i-want <action>         User action (I want to...)
  --so-that <benefit>       User benefit (So that...)
  --output-json             Output full JSON for MCP call
  -h, --help                Show this help

Example:
  create-story.ts --name "User Login Flow" --priority High --epic SPWE-1
  create-story.ts -n "User Login Flow" -p High -e "https://notion.so/..."
`;

const ARG_DEFINITIONS = [
    { flags: ["-n", "--name"], hasValue: true },
    { flags: ["-p", "--priority"], hasValue: true },
    { flags: ["-e", "--epic"], hasValue: true },
    { flags: ["--as-a"], hasValue: true },
    { flags: ["--i-want"], hasValue: true },
    { flags: ["--so-that"], hasValue: true },
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
    const epicRef = parsed.options["epic"] as string;
    const asA = parsed.options["as-a"] as string;
    const iWant = parsed.options["i-want"] as string;
    const soThat = parsed.options["so-that"] as string;
    const outputJson = parsed.options["output-json"] === true;

    // Validate required fields
    const nameValidation = validateRequired(name, "Story name");
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

    const epicValidation = validateRequired(epicRef, "Epic ID or URL");
    if (!epicValidation.valid) {
        exitWithError(epicValidation.error!);
    }

    // Determine if epicRef is an ID or URL
    let epicUrl = epicRef;
    if (epicRef.startsWith("SPWE-") || /^SPWE-\d+$/i.test(epicRef)) {
        // It's an ID - agent needs to search and get URL
        const epicId = normalizeId(epicRef);
        const validation = validateEpicId(epicId);
        if (!validation.valid) {
            exitWithError(validation.error!);
        }
        console.log(`Note: Epic ID ${epicId} provided. Agent should search Notion to get the URL.`);
        epicUrl = `{{EPIC_URL_FOR_${epicId}}}`;
    }

    // Build Story data structure
    const storyData: StoryData = {
        name,
        priority: priority as MoscowPriority,
        epicId: epicRef,
        userStory: {
            asA: asA || "[USER ROLE - To be filled by agent]",
            iWant: iWant || "[USER ACTION - To be filled by agent]",
            soThat: soThat || "[USER BENEFIT - To be filled by agent]",
        },
        acceptanceCriteria: ["[ACCEPTANCE CRITERIA - To be filled by agent]"],
        technicalRefinement: {
            components: [{ name: "[ComponentName]", purpose: "[purpose description]" }],
            systems: [{ name: "[SystemName]", responsibility: "[responsibility description]" }],
            dataFlow: "[DATA FLOW DIAGRAM - To be filled by agent]",
            architecturalDecisions: [
                {
                    decision: "[DECISION - To be filled by agent]",
                    rationale: "[RATIONALE - To be filled by agent]",
                },
            ],
            integrationPoints: ["[INTEGRATION POINTS - To be filled by agent]"],
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
    const content = generateStoryContent(storyData);

    // Build properties
    const properties = buildStoryProperties(name, priority, epicUrl);

    // Build MCP call parameters
    const mcpParams = {
        parent: {
            data_source_id: getDataSourceId("stories"),
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
        printHeader("Story Created Successfully");
        printInfo("Name", name);
        printInfo("Priority", priority);
        printInfo("Epic", epicRef);
        printInfo("Data Source", getDataSourceId("stories"));

        console.log("\n--- Generated Content Preview ---\n");
        console.log(content.substring(0, 500) + "...\n");

        console.log("--- MCP Call ---");
        console.log(`Server: ${NOTION_SERVER}`);
        console.log(`Tool: ${NOTION_TOOLS.createPages}`);
        console.log("\nUse --output-json for full parameters.");
    }
}

main();
