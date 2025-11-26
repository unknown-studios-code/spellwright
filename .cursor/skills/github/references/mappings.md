# Inference Mappings

Tables for automated inference of types, scopes, labels, and footers.

## Branch Type from Task Type

| Notion Task Type | Branch Type |
| ---------------- | ----------- |
| Feature Task     | `feature`   |
| Tech Task        | `tech`      |
| Bug Task         | `bug`       |
| Hotfix           | `hotfix`    |
| Documentation    | `docs`      |

## Commit Type from Changes

| Change Pattern             | Type       |
| -------------------------- | ---------- |
| New files/classes added    | `feat`     |
| Bug fix, error correction  | `fix`      |
| Code restructuring         | `refactor` |
| Performance optimization   | `perf`     |
| Tests only                 | `test`     |
| Documentation only         | `docs`     |
| Formatting, whitespace     | `style`    |
| Build system, dependencies | `build`    |
| CI/CD configuration        | `ci`       |
| Other maintenance          | `chore`    |

### Decision Logic

1. ALL files in `Tests/` or `*Tests.cs` → `test`
2. ALL files are `.md` or docs → `docs`
3. Files in `.github/workflows/` → `ci`
4. New public classes/methods → `feat`
5. Bug/issue fix → `fix`
6. Restructure without new features → `refactor`
7. Default → `feat` or ask user

## Scope from File Paths

| Path Pattern                    | Scope        |
| ------------------------------- | ------------ |
| `Systems/Collision/`            | `collision`  |
| `Systems/Movement/`             | `movement`   |
| `Systems/Lifetime/`             | `lifetime`   |
| `Systems/`                      | `systems`    |
| `Components/`                   | `components` |
| `Jobs/`                         | `jobs`       |
| `Generators/`                   | `generators` |
| `Modifiers/`                    | `modifiers`  |
| `Payloads/`                     | `payloads`   |
| `Baking/`                       | `baking`     |
| `Validation/`                   | `validation` |
| `LLM/` or `*LLM*`               | `llm`        |
| `API/` or `*Api*`               | `api`        |
| `UI/` or `*UI*`                 | `ui`         |
| `Editor/`                       | `editor`     |
| `Tests/`                        | `tests`      |
| `.github/workflows/`            | `ci`         |
| `package.json`, `tsconfig.json` | `build`      |

If files span multiple scopes: use common ancestor or omit.

## Labels from File Paths

| Path Pattern         | Label              |
| -------------------- | ------------------ |
| `Systems/`           | `dots: system`     |
| `Components/`        | `dots: component`  |
| `Jobs/`              | `dots: job`        |
| `*Burst*`            | `dots: burst`      |
| `Generators/`        | `spell: generator` |
| `Modifiers/`         | `spell: modifier`  |
| `Payloads/`          | `spell: payload`   |
| `*Collision*`        | `area: physics`    |
| `*Movement*`         | `area: physics`    |
| `UI/`                | `area: ui`         |
| `*Input*`            | `area: input`      |
| `*Network*`          | `area: networking` |
| `*Render*`, `*VFX*`  | `area: rendering`  |
| `*Audio*`            | `area: audio`      |
| `.github/workflows/` | `type: ci/cd`      |

## Type Labels from Branch/Commit

| Branch Type | Commit Type | Label           |
| ----------- | ----------- | --------------- |
| `feature`   | `feat:`     | `type: feature` |
| `bug`       | `fix:`      | `type: bug`     |
| `tech`      | `refactor:` | `type: tech`    |
| -           | `docs:`     | `type: docs`    |
| -           | `test:`     | `type: test`    |
| -           | `ci:`       | `type: ci/cd`   |

## Footers from Branch Name

| Branch Pattern           | Implements | Part of |
| ------------------------ | ---------- | ------- |
| `<type>/SPWS-X/SPWT-Y`   | SPWT-Y     | SPWS-X  |
| `feature/develop/SPWS-X` | -          | SPWS-X  |

### Regex Patterns

```
Task branch: ^(feature|tech|bug|hotfix|docs)/(SPWS-\d+)/(SPWT-\d+)$
  → Implements: group 3, Part of: group 2

Story branch: ^feature/develop/(SPWS-\d+)$
  → Part of: group 1
```
