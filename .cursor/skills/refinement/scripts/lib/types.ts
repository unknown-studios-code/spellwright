// Artifact Types
export const ARTIFACT_TYPES = ["epic", "story", "task"] as const;
export type ArtifactType = (typeof ARTIFACT_TYPES)[number];

export const TASK_TYPES = ["feature", "tech", "bug"] as const;
export type TaskType = (typeof TASK_TYPES)[number];

// Priority Levels (MoSCoW for Epic/Story/Task, Bug Severity for Bug Tasks)
export const MOSCOW_PRIORITIES = ["Critical", "High", "Medium", "Low"] as const;
export type MoscowPriority = (typeof MOSCOW_PRIORITIES)[number];

export const BUG_SEVERITIES = ["Critical", "High", "Medium", "Low"] as const;
export type BugSeverity = (typeof BUG_SEVERITIES)[number];

export type Priority = MoscowPriority | BugSeverity;

// Risk Categories
export const RISK_CATEGORIES = {
    technical: "🔴",
    dependency: "🟡",
    knowledge: "🟠",
    scope: "🟠",
    timeline: "🔵",
    integration: "🔵",
    testing: "🔵",
    migration: "🟡",
    performance: "🟠",
    side_effect: "🟡",
    regression: "🟠",
    platform: "🔵",
} as const;

export type RiskCategory = keyof typeof RISK_CATEGORIES;

// Notion Data Source IDs
export const DATA_SOURCE_IDS = {
    epics: "28e56d55-129b-8189-827c-000b7bda32ac",
    stories: "28e56d55-129b-8156-85c3-000bea866839",
    tasks: "28e56d55-129b-8181-b82b-000b7a52fcfb",
} as const;

export type DataSourceType = keyof typeof DATA_SOURCE_IDS;

// ID Patterns
export const ID_PATTERNS = {
    epic: /^SPWE-\d+$/i,
    story: /^SPWS-\d+$/i,
    task: /^SPWT-\d+$/i,
} as const;

// Notion Property Names
export const NOTION_PROPERTIES = {
    epic: {
        id: "userDefined:ID",
        name: "Name",
        priority: "Priority",
        status: "Status",
        assignee: "Assignee",
        stories: "Stories",
        documentation: "Documentation",
    },
    story: {
        id: "userDefined:ID",
        name: "Name",
        priority: "Priority",
        status: "Status",
        assignee: "Assignee",
        epic: "Epic",
        tasks: "Tasks",
        branch: "Branch",
        pullRequest: "Pull Request",
    },
    task: {
        id: "userDefined:ID",
        name: "Name",
        priority: "Priority",
        status: "Status",
        assignee: "Assignee",
        type: "Type",
        story: "Story",
        branch: "Branch",
        pullRequest: "Pull Request",
    },
} as const;

// Interfaces for Artifact Data
export interface EpicData {
    name: string;
    priority: MoscowPriority;
    strategicGoal: string;
    problemStatement: {
        problem: string;
        targetUser: string;
    };
    valueHypothesis: {
        action: string;
        outcome: string;
        measurement: string;
    };
    scope: {
        inScope: string[];
        outOfScope: string[];
    };
    successCriteria: {
        quantitative: string[];
        qualitative: string[];
    };
    risks: RiskItem[];
    references: ReferenceSection;
}

export interface StoryData {
    name: string;
    priority: MoscowPriority;
    epicId: string;
    userStory: {
        asA: string;
        iWant: string;
        soThat: string;
    };
    acceptanceCriteria: string[];
    technicalRefinement: {
        components: ComponentInfo[];
        systems: SystemInfo[];
        dataFlow: string;
        architecturalDecisions: ArchitecturalDecision[];
        integrationPoints: string[];
    };
    risks: RiskItem[];
    references: ReferenceSection;
}

export interface TaskData {
    name: string;
    priority: Priority;
    storyId: string;
    type: TaskType;
    description?: string;
    technicalObjective?: TechnicalObjective;
    bugReport?: BugReport;
    definitionOfDone: string[];
    technicalRefinement: TaskTechnicalRefinement;
    risks: RiskItem[];
    references: ReferenceSection;
}

// Supporting Interfaces
export interface RiskItem {
    category: RiskCategory;
    description: string;
    mitigation?: string;
}

export interface ReferenceSection {
    designFiles?: ReferenceLink[];
    documentation?: ReferenceLink[];
    externalResources?: ReferenceLink[];
}

export interface ReferenceLink {
    title: string;
    url?: string;
}

export interface ComponentInfo {
    name: string;
    purpose: string;
}

export interface SystemInfo {
    name: string;
    responsibility: string;
}

export interface ArchitecturalDecision {
    decision: string;
    rationale: string;
}

export interface TechnicalObjective {
    what: string;
    why: string;
}

export interface BugReport {
    stepsToReproduce: string[];
    expectedBehavior: string;
    actualBehavior: string;
    environment: EnvironmentInfo;
    rootCauseAnalysis?: string;
}

export interface EnvironmentInfo {
    unityVersion: string;
    dotsPackages?: string[];
    buildTarget: string;
    deviceSpecs?: string;
    commitHash?: string;
    reproducibility: string;
}

export interface TaskTechnicalRefinement {
    coreScripts?: ScriptInfo[];
    keyAssets?: AssetInfo[];
    inspectorValues?: string[];
    implementationNotes?: string[];
    dependencies?: string[];
    steps?: ImplementationStep[];
    performanceImpact?: string;
}

export interface ScriptInfo {
    path: string;
    description: string;
}

export interface AssetInfo {
    path: string;
    description: string;
}

export interface ImplementationStep {
    title: string;
    details: string[];
}

// Notion API Types
export interface NotionPageProperties {
    [key: string]: string | number | null;
}

export interface NotionCreatePageParams {
    dataSourceId: string;
    properties: NotionPageProperties;
    content: string;
}

export interface NotionUpdatePageParams {
    pageId: string;
    properties?: NotionPageProperties;
    content?: string;
}

export interface NotionSearchResult {
    id: string;
    title: string;
    url: string;
    type: string;
}

export interface NotionPageResult {
    id: string;
    title: string;
    url: string;
    properties: Record<string, unknown>;
    content: string;
}

// Validation Result
export interface ValidationResult {
    valid: boolean;
    error?: string;
}

// CLI Types
export interface ParsedArgs {
    options: Record<string, string | boolean>;
    positional: string[];
}

export interface ArgDefinition {
    flags: string[];
    hasValue?: boolean;
    isBoolean?: boolean;
}
