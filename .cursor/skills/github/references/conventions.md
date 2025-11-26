# Conventions

Standards for branches, commits, and pull requests.

## Branch Naming

### Story Branch

```
feature/develop/SPWS-X
```

### Task Branch

```
<type>/<story-id>/<task-id>
```

Types: `feature`, `tech`, `bug`, `hotfix`, `docs`

Examples: `tech/SPWS-4/SPWT-13`, `bug/SPWS-5/SPWT-42`

### Base Branch

| Type   | Base      |
| ------ | --------- |
| Story  | `develop` |
| Task   | `develop` |
| Hotfix | `main`    |

## Commit Format

```
<type>(<scope>): <subject>

[body]

[footer(s)]
```

### Types

| Type       | Impact | Usage                        |
| ---------- | ------ | ---------------------------- |
| `feat`     | MINOR  | New feature                  |
| `fix`      | PATCH  | Bug fix                      |
| `docs`     | None   | Documentation only           |
| `style`    | None   | Formatting (no logic change) |
| `refactor` | None   | Code change (no feature/fix) |
| `perf`     | PATCH  | Performance improvement      |
| `test`     | None   | Tests                        |
| `build`    | None   | Build system, dependencies   |
| `ci`       | None   | CI configuration             |
| `chore`    | None   | Other maintenance            |
| `revert`   | None   | Revert previous commit       |

### Subject Rules

- Imperative mood: "add", "fix", "change"
- Lowercase, no period
- Under 72 characters

### Footers

```
Implements: SPWT-XX
Part of: SPWS-X
Related: SPWT-XX, SPWS-X
```

### Body Sections (for complex commits)

| Section           | When to Use                  |
| ----------------- | ---------------------------- |
| `Implementation:` | Main code changes            |
| `Tests:`          | Test additions/modifications |
| `Performance:`    | Optimizations                |
| `Fixes:`          | Bug fixes within feature     |
| `Configuration:`  | Config changes               |

## PR Format

### Title

```
<type>(<scope>): <subject>
```

Same rules as commit subject.

### Labels (3-5 required)

**Type (1):** `type: feature`, `type: bug`, `type: tech`, `type: docs`, `type: test`, `type: ci/cd`, `type: breaking`

**Priority (1):** `priority: critical`, `priority: high`, `priority: medium`, `priority: low`

**Context (1-2):**

- Spell: `spell: generator`, `spell: modifier`, `spell: payload`, `spell: component`
- DOTS: `dots: system`, `dots: component`, `dots: job`, `dots: burst`, `dots: entities`
- Area: `area: architecture`, `area: ui`, `area: physics`, `area: networking`, `area: input`, `area: rendering`, `area: audio`

**Status (optional):** `status: blocked`, `status: in progress`, `status: needs review`, `status: needs testing`

### Body Template

```markdown
## 📋 Task: SPWT-X

**Story:** SPWS-X - Story Title
**Type:** Feature/Tech/Bug
**Priority:** Critical/High/Medium/Low

---

## 🎯 What Changed

Brief summary (2-4 sentences).

---

## 📁 Files Created/Modified

Directory/
├── File1.cs (description)
└── File2.cs (description)

---

## ✅ Definition of Done

- [x] DoD item from Notion

---

## 🔑 Key Technical Decisions

1. **Decision** → Rationale

---

## 🔗 References

- **Notion Task:** [SPWT-X](notion_url)
- **Story:** [SPWS-X](notion_url)
```
