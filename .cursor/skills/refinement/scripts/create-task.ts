#!/usr/bin/env npx ts-node

/**
 * Create Task Script
 *
 * Generates Task content (Feature, Tech, or Bug) and outputs the MCP call parameters.
 * This script helps structure the data; the actual MCP call is made by the agent.
 *
 * Usage:
 *   npx ts-node create-task.ts --name "Task Name" --type feature --priority High --story SPWS-1
 *
 * The script outputs JSON that can be used with the Notion MCP.
 */

import { parseArgs, exitWithError, exitWithUsage, printHeader, printJson, printInfo } from "./lib/cli";
import { isMoscowPriority, isBugSeverity, isTaskType, validateRequired, validateStoryId, normalizeId } from "./lib/validation";
import { buildTaskProperties, getDataSourceId, NOTION_SERVER, NOTION_TOOLS } from "./lib/notion";
import { generateTaskContent } from "./lib/templates";
import { TaskData, TaskType, Priority, TASK_TYPES, MOSCOW_PRIORITIES, BUG_SEVERITIES } from "./lib/types";

const USAGE = `
Create Task - Generate Task for Notion

Usage: create-task.ts --name <name> --type <type> --priority <priority> --story <story-id> [options]

Required:
  -n, --name <name>         Task name/title
  -t, --type <type>         Task type: ${TASK_TYPES.join(", ")}
  -p, --priority <priority> Priority: ${MOSCOW_PRIORITIES.join(", ")} (or Bug Severity for bugs)
  -s, --story <story-id>    Parent Story ID (SPWS-X) or Notion URL

Optional:
  --description <text>      Task description (for Feature tasks)
  --what <text>             Technical objective - what (for Tech tasks)
  --why <text>              Technical objective - why (for Tech tasks)
  --output-json             Output full JSON for MCP call
  -h, --help                Show this help

Examples:
  # Feature Task
  create-task.ts --name "Implement Login UI" --type feature --priority High --story SPWS-1
  
  # Tech Task
  create-task.ts --name "Refactor Auth Service" --type tech --priority Medium --story SPWS-1
  
  # Bug Task
  create-task.ts --name "Fix Login Crash" --type bug --priority Critical --story SPWS-1
`;

const ARG_DEFINITIONS = [
    { flags: ["-n", "--name"], hasValue: true },
    { flags: ["-t", "--type"], hasValue: true },
    { flags: ["-p", "--priority"], hasValue: true },
    { flags: ["-s", "--story"], hasValue: true },
    { flags: ["--description"], hasValue: true },
    { flags: ["--what"], hasValue: true },
    { flags: ["--why"], hasValue: true },
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
    const typeValue = parsed.options["type"] as string;
    const priority = parsed.options["priority"] as string;
    const storyRef = parsed.options["story"] as string;
    const description = parsed.options["description"] as string;
    const what = parsed.options["what"] as string;
    const why = parsed.options["why"] as string;
    const outputJson = parsed.options["output-json"] === true;

    // Validate required fields
    const nameValidation = validateRequired(name, "Task name");
    if (!nameValidation.valid) {
        exitWithError(nameValidation.error!);
    }

    const typeValidation = validateRequired(typeValue, "Task type");
    if (!typeValidation.valid) {
        exitWithError(typeValidation.error!);
    }

    if (!isTaskType(typeValue)) {
        exitWithError(`Invalid task type. Must be one of: ${TASK_TYPES.join(", ")}`);
    }

    const taskType = typeValue.toLowerCase() as TaskType;

    const priorityValidation = validateRequired(priority, "Priority");
    if (!priorityValidation.valid) {
        exitWithError(priorityValidation.error!);
    }

    // Validate priority based on task type
    if (taskType === "bug") {
        if (!isBugSeverity(priority)) {
            exitWithError(`Invalid bug severity. Must be one of: ${BUG_SEVERITIES.join(", ")}`);
        }
    } else {
        if (!isMoscowPriority(priority)) {
            exitWithError(`Invalid priority. Must be one of: ${MOSCOW_PRIORITIES.join(", ")}`);
        }
    }

    const storyValidation = validateRequired(storyRef, "Story ID or URL");
    if (!storyValidation.valid) {
        exitWithError(storyValidation.error!);
    }

    // Determine if storyRef is an ID or URL
    let storyUrl = storyRef;
    if (storyRef.startsWith("SPWS-") || /^SPWS-\d+$/i.test(storyRef)) {
        const storyId = normalizeId(storyRef);
        const validation = validateStoryId(storyId);
        if (!validation.valid) {
            exitWithError(validation.error!);
        }
        console.log(`Note: Story ID ${storyId} provided. Agent should search Notion to get the URL.`);
        storyUrl = `{{STORY_URL_FOR_${storyId}}}`;
    }

    // Build Task data structure based on type
    const taskData: TaskData = {
        name,
        priority: priority as Priority,
        storyId: storyRef,
        type: taskType,
        definitionOfDone: [
            "[DOD ITEM 1 - To be filled by agent]",
            "[DOD ITEM 2 - To be filled by agent]",
            "Branch created and link added to Notion task property 'Branch'",
            "PR created and link added to Notion task property 'Pull Request'",
            "Pull Request reviewed, approved, and merged",
        ],
        technicalRefinement: {
            coreScripts: [{ path: "[PATH/TO/SCRIPT.cs]", description: "[Script description]" }],
            keyAssets: [{ path: "[PATH/TO/ASSET]", description: "[Asset description]" }],
            implementationNotes: ["[IMPLEMENTATION NOTE - To be filled by agent]"],
            dependencies: ["[DEPENDENCY - To be filled by agent]"],
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

    // Add type-specific fields
    if (taskType === "feature") {
        taskData.description = description || "[FEATURE DESCRIPTION - To be filled by agent]";
    } else if (taskType === "tech") {
        taskData.technicalObjective = {
            what: what || "[WHAT - To be filled by agent]",
            why: why || "[WHY - To be filled by agent]",
        };
        taskData.technicalRefinement.steps = [
            {
                title: "[STEP TITLE]",
                details: ["[Step detail 1]", "[Step detail 2]"],
            },
        ];
        taskData.technicalRefinement.performanceImpact = "[PERFORMANCE IMPACT - To be filled by agent]";
    } else if (taskType === "bug") {
        taskData.bugReport = {
            stepsToReproduce: ["[STEP 1 - To be filled by agent]", "[STEP 2 - To be filled by agent]"],
            expectedBehavior: "[EXPECTED BEHAVIOR - To be filled by agent]",
            actualBehavior: "[ACTUAL BEHAVIOR - To be filled by agent]",
            environment: {
                unityVersion: "[UNITY VERSION]",
                dotsPackages: ["[PACKAGE VERSION]"],
                buildTarget: "[BUILD TARGET]",
                reproducibility: "[REPRODUCIBILITY %]",
            },
        };
        taskData.technicalRefinement.steps = [
            {
                title: "[FIX STEP TITLE]",
                details: ["[Fix step detail 1]", "[Fix step detail 2]"],
            },
        ];
    }

    // Generate content
    const content = generateTaskContent(taskData);

    // Build properties - capitalize task type for Notion
    const taskTypeFormatted = taskType.charAt(0).toUpperCase() + taskType.slice(1);
    const properties = buildTaskProperties(name, priority, taskTypeFormatted, storyUrl);

    // Build MCP call parameters
    const mcpParams = {
        parent: {
            data_source_id: getDataSourceId("tasks"),
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
        printHeader(`${taskTypeFormatted} Task Created Successfully`);
        printInfo("Name", name);
        printInfo("Type", taskTypeFormatted);
        printInfo("Priority", priority);
        printInfo("Story", storyRef);
        printInfo("Data Source", getDataSourceId("tasks"));

        console.log("\n--- Generated Content Preview ---\n");
        console.log(content.substring(0, 500) + "...\n");

        console.log("--- MCP Call ---");
        console.log(`Server: ${NOTION_SERVER}`);
        console.log(`Tool: ${NOTION_TOOLS.createPages}`);
        console.log("\nUse --output-json for full parameters.");
    }
}

main();
