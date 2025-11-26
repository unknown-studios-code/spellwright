# Priority Frameworks

Reference for priority and severity classification.

## MoSCoW Priority Framework

Used for Epics, Stories, and Feature/Tech Tasks.

### Critical (Must Have)

**Definition:** Absolutely essential. Release cannot exist without this. Blocks all other work.

**Criteria:**

1. Without this, nothing else can be built → Critical
2. Is this an absolute prerequisite for ANY other work? → Critical
3. Does this define the fundamental architecture? → Critical

**Examples:**

- Core system architecture
- Foundational infrastructure
- Critical dependencies
- Core data structures

### High (Must Have)

**Definition:** Essential for release to function properly. Product/feature cannot ship without this.

**Criteria:**

1. Does this block other epics/stories/tasks? → High
2. Is this part of the primary user flow? → High
3. Can release demonstrate core value without this? If NO → High
4. Does this provide components/systems required by others? → High

**Examples:**

- Core features
- Key user workflows
- Critical integrations
- Primary systems implementation

### Medium (Should Have)

**Definition:** Important but not blocking. Release can function with workaround or simplified version.

**Criteria:**

1. Is there an acceptable workaround? If YES → Medium
2. Can this be partially delivered? If YES → Medium
3. Does this enhance but not enable core functionality? → Medium

**Examples:**

- Enhanced validation
- Additional content types
- Quality-of-life improvements
- Extended error handling

### Low (Could Have)

**Definition:** Desirable but can be deferred. Enhancement or polish.

**Criteria:**

1. Can this be added in future iteration? If YES → Low
2. Is this polish or optimization? → Low
3. Does release demonstrate value without this? If YES → Low

**Examples:**

- Advanced features
- Editor tooling
- Polish and optimization
- Extended logging

---

## Bug Severity Framework

Used ONLY for Bug Tasks.

### Critical

**Definition:** Showstopper that prevents use of feature or causes data loss/corruption.

**Examples:**

- Crashes on launch
- Complete feature broken
- Data corruption
- Security vulnerability

**Criteria:**

- Crashes
- Data loss
- Security issue
- No workaround possible

### High

**Definition:** Major bug that blocks core functionality with no reasonable workaround.

**Examples:**

- Crashes in common scenarios
- Core feature broken
- No workaround available

**Criteria:**

- Blocks main workflow
- High frequency
- No workaround

### Medium

**Definition:** Important bug that degrades experience but has a workaround.

**Examples:**

- Degraded UX
- Edge case behavior
- Non-blocking but noticeable

**Criteria:**

- Has workaround
- Impacts experience
- Moderate frequency

### Low

**Definition:** Minor cosmetic issue or rare edge case with minimal impact.

**Examples:**

- Visual glitch
- Rare scenario
- Polish issues

**Criteria:**

- Cosmetic
- Rare
- Minimal user impact

---

## Risk Categories

Use these emoji-based categories for ALL risk sections:

| Emoji | Category                  | When to Use                                                |
| ----- | ------------------------- | ---------------------------------------------------------- |
| 🔴    | Technical Risk            | Architecture, technology, performance challenges           |
| 🟡    | Dependency Risk           | Blocked by other work, external factors, team dependencies |
| 🟠    | Knowledge/Scope Risk      | Team expertise gaps, unclear boundaries, scope creep       |
| 🔵    | Timeline/Integration Risk | Schedule concerns, integration conflicts, coordination     |

### Risk Format

```markdown
- 🔴 **[Category] Risk:** [Brief description]
    - Mitigation: [How to address or reduce risk]
```

### Examples

```markdown
- 🔴 **Performance Risk:** System may not scale to 1000 entities
    - Mitigation: Early stress testing with 2000 entities, profile before implementation

- 🟡 **Dependency Risk:** Blocked if SPWS-1 (components) is delayed
    - Mitigation: Create mock components for parallel development

- 🟠 **Knowledge Risk:** Team unfamiliar with Unity DOTS patterns
    - Mitigation: Knowledge sharing session, pair programming

- 🔵 **Timeline Risk:** Integration complexity unknown
    - Mitigation: Early prototype to validate approach
```
