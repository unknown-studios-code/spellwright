# Template Reference

Quick reference for artifact template structures.

## Template Files

| Artifact     | Template File                                             |
| ------------ | --------------------------------------------------------- |
| Epic         | [templates/epic.md](../templates/epic.md)                 |
| Story        | [templates/story.md](../templates/story.md)               |
| Feature Task | [templates/feature-task.md](../templates/feature-task.md) |
| Tech Task    | [templates/tech-task.md](../templates/tech-task.md)       |
| Bug Task     | [templates/bug-task.md](../templates/bug-task.md)         |

---

## Epic Structure

```
# [Epic Name]

### 🎯 Strategic Goal
[2-3 sentence elevator pitch]

### 🤔 Problem Statement
**What problem are we solving?**
**Who is it for?**

### 💡 Value Hypothesis
- We believe that by [ACTION]
- We will achieve [OUTCOME]
- This will be measured by [METRIC]

### 📦 Scope
**In Scope:** (numbered list)
**Out of Scope:** (bullet list)

### 📈 Success Criteria
**Quantitative Metrics:** (bullets)
**Qualitative Acceptance Criteria:** (numbered)

### ⚠️ Potential Risks
(categorized with 🔴🟡🟠🔵)

### 🔗 References
(design files, docs, external resources)
```

---

## Story Structure

```
# [Story Title]

### 👤 User Story
- As a [role]
- I want to [action]
- So that [benefit]

### ✅ Acceptance Criteria
(numbered, testable behaviors)

### ⚙️ Technical Refinement
**Components Needed:** (name + purpose)
**Systems Needed:** (name + responsibility)
**Data Flow:** (diagram)
**Key Architectural Decisions:** (decision + rationale)
**Integration Points:** (dependencies)

### ⚠️ Potential Risks
(categorized with 🔴🟡🟠🔵)

### 🔗 References
(parent Epic, design files, docs)
```

---

## Feature Task Structure

```
# [Feature Title]

### 📝 Description
[User-facing behavior description]

### ✅ Definition of Done
(numbered, with exact file paths, 10+ items)

### ⚙️ Technical Refinement
**Core Scripts:** (path + description)
**Key Prefabs/Assets:** (path + description)
**Inspector Values:** (field + default + description)
**Implementation Notes:** (algorithms, patterns)
**Dependencies:** (SPWT-X references)

### ⚠️ Potential Risks
(categorized with 🔴🟡🟠🔵)

### 🔗 References
(design files, Unity docs, external resources)
```

---

## Tech Task Structure

```
# [Tech Title]

### 🎯 Technical Objective
- **What:** [change description]
- **Why:** [benefit with metrics]

### ✅ Definition of Done
(numbered, with performance targets)

### ⚙️ Technical Refinement
**Step 1: [Title]**
(specific changes)

**Step 2: [Title]**
(specific changes)

**Performance Impact:**
- Before: [metric]
- After: [metric]
- **[X]x improvement**

### ⚠️ Potential Risks
(categorized with 🔴🟡🟠🔵)

### 🔗 References
(Unity docs, project rules)
```

---

## Bug Task Structure

```
# [Bug Title]

### 🐛 Bug Report
**Steps to Reproduce:** (numbered, exact)
**Expected Behavior:**
**Actual Behavior:**
**Environment:** (versions, platform, reproducibility)

### Root Cause Analysis
[Why the bug occurs]

### ✅ Definition of Done
(numbered, includes regression test)

### 🔧 Implementation Plan
**Step 1: [Fix Title]**
(specific changes)

### ⚠️ Potential Risks
(categorized with 🔴🟡🟠🔵)

### 🔗 References
(bug tracking, docs)
```

---

## Universal Rules

### Content Rules

1. **No Placeholders** - Every section must have real content
2. **Specific Details** - Use concrete numbers, file paths, system names
3. **Unity Terminology** - Use correct Unity/DOTS terms
4. **Performance Targets** - Include quantifiable metrics
5. **English Only** - All artifacts in English

### Formatting Rules

1. Use **numbered lists** for sequential/ordered items
2. Use **bullets** for unordered items
3. **No checkboxes** in artifact descriptions
4. Keep all **emojis** in section headers
5. Preserve **markdown formatting**

### Risk Format

```markdown
- 🔴 **[Category] Risk:** [Description]
    - Mitigation: [How to address]
```
