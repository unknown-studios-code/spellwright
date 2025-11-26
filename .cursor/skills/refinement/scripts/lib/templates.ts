import { EpicData, StoryData, TaskData, RiskItem, ReferenceSection, RISK_CATEGORIES, ComponentInfo, SystemInfo, ArchitecturalDecision, ImplementationStep, ScriptInfo, AssetInfo } from "./types";

/**
 * Generate Epic content in Notion Markdown format
 */
export function generateEpicContent(data: EpicData): string {
    const sections: string[] = [];

    // Strategic Goal
    sections.push(`### 🎯 Strategic Goal\n${data.strategicGoal}\n---`);

    // Problem Statement
    sections.push(`### 🤔 Problem Statement
**What problem are we solving?**
${data.problemStatement.problem}

**Who is it for?**
${data.problemStatement.targetUser}
---`);

    // Value Hypothesis
    sections.push(`### 💡 Value Hypothesis
- **We believe that by** ${data.valueHypothesis.action}
- **We will achieve** ${data.valueHypothesis.outcome}
- **This will be measured by** ${data.valueHypothesis.measurement}
---`);

    // Scope
    const inScopeItems = data.scope.inScope.map((item, i) => `${i + 1}. ${item}`).join("\n");
    const outOfScopeItems = data.scope.outOfScope.map((item) => `- ${item}`).join("\n");
    sections.push(`### 📦 Scope
**In Scope:**
${inScopeItems}

**Out of Scope:**
${outOfScopeItems}
---`);

    // Success Criteria
    const quantitative = data.successCriteria.quantitative.map((m) => `- ${m}`).join("\n");
    const qualitative = data.successCriteria.qualitative.map((c, i) => `${i + 1}. ${c}`).join("\n");
    sections.push(`### 📈 Success Criteria
**Quantitative Metrics:**
${quantitative}

**Qualitative Acceptance Criteria:**
${qualitative}
---`);

    // Risks
    sections.push(`### ⚠️ Potential Risks\n${formatRisks(data.risks)}\n---`);

    // References
    sections.push(`### 🔗 References\n${formatReferences(data.references)}`);

    return sections.join("\n");
}

/**
 * Generate Story content in Notion Markdown format
 */
export function generateStoryContent(data: StoryData): string {
    const sections: string[] = [];

    // User Story
    sections.push(`### 👤 User Story
- **As a** ${data.userStory.asA}
- **I want to** ${data.userStory.iWant}
- **So that** ${data.userStory.soThat}
---`);

    // Acceptance Criteria
    const criteria = data.acceptanceCriteria.map((c, i) => `${i + 1}. ${c}`).join("\n");
    sections.push(`### ✅ Acceptance Criteria\n${criteria}\n---`);

    // Technical Refinement
    sections.push(`### ⚙️ Technical Refinement
**Components Needed:**
${formatComponents(data.technicalRefinement.components)}

**Systems Needed:**
${formatSystems(data.technicalRefinement.systems)}

**Data Flow:**
\`\`\`javascript
${data.technicalRefinement.dataFlow}
\`\`\`

**Key Architectural Decisions:**
${formatArchitecturalDecisions(data.technicalRefinement.architecturalDecisions)}

**Integration Points:**
${data.technicalRefinement.integrationPoints.map((p) => `- ${p}`).join("\n")}
---`);

    // Risks
    sections.push(`### ⚠️ Potential Risks\n${formatRisks(data.risks)}\n---`);

    // References
    sections.push(`### 🔗 References\n${formatReferences(data.references)}`);

    return sections.join("\n");
}

/**
 * Generate Feature Task content in Notion Markdown format
 */
export function generateFeatureTaskContent(data: TaskData): string {
    const sections: string[] = [];

    // Description
    sections.push(`### 📝 Description\n${data.description || ""}\n---`);

    // Definition of Done
    const dod = data.definitionOfDone.map((item, i) => `${i + 1}. ${item}`).join("\n");
    sections.push(`### ✅ Definition of Done\n${dod}\n---`);

    // Technical Refinement
    sections.push(`### ⚙️ Technical Refinement
${formatTaskTechnicalRefinement(data)}
---`);

    // Risks
    sections.push(`### ⚠️ Potential Risks\n${formatRisks(data.risks)}\n---`);

    // References
    sections.push(`### 🔗 References\n${formatReferences(data.references)}`);

    return sections.join("\n");
}

/**
 * Generate Tech Task content in Notion Markdown format
 */
export function generateTechTaskContent(data: TaskData): string {
    const sections: string[] = [];

    // Technical Objective
    if (data.technicalObjective) {
        sections.push(`### 🎯 Technical Objective
- **What:** ${data.technicalObjective.what}
- **Why:** ${data.technicalObjective.why}
---`);
    }

    // Definition of Done
    const dod = data.definitionOfDone.map((item, i) => `${i + 1}. ${item}`).join("\n");
    sections.push(`### ✅ Definition of Done\n${dod}\n---`);

    // Technical Refinement
    sections.push(`### ⚙️ Technical Refinement
${formatTechTaskRefinement(data)}
---`);

    // Risks
    sections.push(`### ⚠️ Potential Risks\n${formatRisks(data.risks)}\n---`);

    // References
    sections.push(`### 🔗 References\n${formatReferences(data.references)}`);

    return sections.join("\n");
}

/**
 * Generate Bug Task content in Notion Markdown format
 */
export function generateBugTaskContent(data: TaskData): string {
    const sections: string[] = [];

    // Bug Report
    if (data.bugReport) {
        const steps = data.bugReport.stepsToReproduce.map((s, i) => `${i + 1}. ${s}`).join("\n");
        sections.push(`### 🐛 Bug Report
**Steps to Reproduce:**
${steps}

**Expected Behavior:**
${data.bugReport.expectedBehavior}

**Actual Behavior:**
${data.bugReport.actualBehavior}

**Environment:**
- **Unity Version:** ${data.bugReport.environment.unityVersion}
${data.bugReport.environment.dotsPackages ? `- **DOTS Packages:**\n${data.bugReport.environment.dotsPackages.map((p) => `    - ${p}`).join("\n")}` : ""}
- **Build Target:** ${data.bugReport.environment.buildTarget}
${data.bugReport.environment.deviceSpecs ? `- **Device Specs:** ${data.bugReport.environment.deviceSpecs}` : ""}
${data.bugReport.environment.commitHash ? `- **Commit Hash:** \`${data.bugReport.environment.commitHash}\`` : ""}
- **Reproducibility:** ${data.bugReport.environment.reproducibility}
---`);
    }

    // Root Cause Analysis (if available)
    if (data.bugReport?.rootCauseAnalysis) {
        sections.push(`### Root Cause Analysis\n${data.bugReport.rootCauseAnalysis}\n---`);
    }

    // Definition of Done
    const dod = data.definitionOfDone.map((item, i) => `${i + 1}. ${item}`).join("\n");
    sections.push(`### ✅ Definition of Done\n${dod}\n---`);

    // Implementation Plan
    if (data.technicalRefinement.steps && data.technicalRefinement.steps.length > 0) {
        sections.push(`### 🔧 Implementation Plan
${formatImplementationSteps(data.technicalRefinement.steps)}
---`);
    }

    // Risks
    sections.push(`### ⚠️ Potential Risks\n${formatRisks(data.risks)}\n---`);

    // References
    sections.push(`### 🔗 References\n${formatReferences(data.references)}`);

    return sections.join("\n");
}

/**
 * Generate Task content based on type
 */
export function generateTaskContent(data: TaskData): string {
    switch (data.type) {
        case "feature":
            return generateFeatureTaskContent(data);
        case "tech":
            return generateTechTaskContent(data);
        case "bug":
            return generateBugTaskContent(data);
        default:
            return generateFeatureTaskContent(data);
    }
}

// Helper Functions

function formatRisks(risks: RiskItem[]): string {
    return risks
        .map((risk) => {
            const emoji = RISK_CATEGORIES[risk.category] || "🔴";
            const categoryName = risk.category.charAt(0).toUpperCase() + risk.category.slice(1);
            let line = `- ${emoji} **${categoryName} Risk:** ${risk.description}`;
            if (risk.mitigation) {
                line += `\n    - Mitigation: ${risk.mitigation}`;
            }
            return line;
        })
        .join("\n");
}

function formatReferences(refs: ReferenceSection): string {
    const sections: string[] = [];

    if (refs.designFiles && refs.designFiles.length > 0) {
        sections.push(`- **🎨 Design Files:**`);
        refs.designFiles.forEach((ref) => {
            sections.push(`    - [${ref.title}](${ref.url || "URL"})`);
        });
    }

    if (refs.documentation && refs.documentation.length > 0) {
        sections.push(`- **📚 Documentation:**`);
        refs.documentation.forEach((ref) => {
            sections.push(`    - [${ref.title}](${ref.url || "URL"})`);
        });
    }

    if (refs.externalResources && refs.externalResources.length > 0) {
        sections.push(`- **🌐 External Resources:**`);
        refs.externalResources.forEach((ref) => {
            sections.push(`    - [${ref.title}](${ref.url || "URL"})`);
        });
    }

    return sections.join("\n");
}

function formatComponents(components: ComponentInfo[]): string {
    return components.map((c) => `- \`${c.name}\` (purpose: ${c.purpose})`).join("\n");
}

function formatSystems(systems: SystemInfo[]): string {
    return systems.map((s) => `- \`${s.name}\` (responsibility: ${s.responsibility})`).join("\n");
}

function formatArchitecturalDecisions(decisions: ArchitecturalDecision[]): string {
    return decisions.map((d, i) => `- **Decision ${i + 1}:** ${d.decision}\n  - **Rationale:** ${d.rationale}`).join("\n");
}

function formatTaskTechnicalRefinement(data: TaskData): string {
    const sections: string[] = [];
    const ref = data.technicalRefinement;

    if (ref.coreScripts && ref.coreScripts.length > 0) {
        sections.push(`**Core Scripts:**\n${formatScripts(ref.coreScripts)}`);
    }

    if (ref.keyAssets && ref.keyAssets.length > 0) {
        sections.push(`**Key Prefabs/Assets:**\n${formatAssets(ref.keyAssets)}`);
    }

    if (ref.inspectorValues && ref.inspectorValues.length > 0) {
        sections.push(`**Inspector Values:**\n${ref.inspectorValues.map((v) => `- ${v}`).join("\n")}`);
    }

    if (ref.implementationNotes && ref.implementationNotes.length > 0) {
        sections.push(`**Implementation Notes:**\n${ref.implementationNotes.map((n) => `- ${n}`).join("\n")}`);
    }

    if (ref.dependencies && ref.dependencies.length > 0) {
        sections.push(`**Dependencies:**\n${ref.dependencies.map((d) => `- ${d}`).join("\n")}`);
    }

    return sections.join("\n\n");
}

function formatTechTaskRefinement(data: TaskData): string {
    const sections: string[] = [];
    const ref = data.technicalRefinement;

    if (ref.steps && ref.steps.length > 0) {
        sections.push(formatImplementationSteps(ref.steps));
    }

    if (ref.performanceImpact) {
        sections.push(`**Performance Impact:**\n${ref.performanceImpact}`);
    }

    return sections.join("\n\n");
}

function formatImplementationSteps(steps: ImplementationStep[]): string {
    return steps
        .map((step, i) => {
            const details = step.details.map((d) => `- ${d}`).join("\n");
            return `**Step ${i + 1}: ${step.title}**\n${details}`;
        })
        .join("\n\n");
}

function formatScripts(scripts: ScriptInfo[]): string {
    return scripts.map((s) => `- \`${s.path}\` - ${s.description}`).join("\n");
}

function formatAssets(assets: AssetInfo[]): string {
    return assets.map((a) => `- \`${a.path}\` - ${a.description}`).join("\n");
}
