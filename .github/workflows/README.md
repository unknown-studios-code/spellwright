# GitHub Actions Workflows for Spellwright

This directory contains the CI/CD workflows configured for the Spellwright repository, ensuring code quality, compilation validation, and automated testing for Unity DOTS/ECS project.

## 📁 Structure

```
.github/workflows/
├── pr-lint.yml              # PR title validation
├── dotnet-lint.yml          # C# code formatting
├── unity-build.yml          # Unity compilation validation
├── unity-tests-editmode.yml # Unit tests (Edit Mode)
├── unity-tests-playmode.yml # Integration tests (Play Mode)
└── README.md                # This file
```

## 🎯 Configured Workflows

### 1. **PR Title Validation** (`pr-lint.yml`)

Validates Pull Request titles to ensure they follow Conventional Commits specification.

**Triggers:**

-   `pull_request_target`: opened, edited, synchronize
-   Runs on: All branches

**Requirements:**

-   Format: `type(scope): subject`
-   Types: `feat`, `fix`, `docs`, `refactor`, `perf`, `test`, `chore`, `ci`, `build`, `revert`
-   Scope: Required
-   Subject: Must start with lowercase letter, max 72 characters

**Status Check Generated:**

-   `PR Lint / Validate PR title`

**Example:**

```
✅ feat(components): implement velocity component
❌ Feat(components): Implement velocity component
❌ feat: implement velocity component (missing scope)
```

**Bypass:**

-   Add label `skip-title-check` to PR

---

### 2. **C# Code Formatting** (`dotnet-lint.yml`)

Validates C# code formatting using `dotnet format` with `.editorconfig` rules.

**Triggers:**

-   `pull_request`: Changes to `**.cs`, `**.csproj`, or `.editorconfig`
-   `push`: To `master` or `develop` branches
-   Runs on: `master`, `develop`

**What it checks:**

-   ✅ Code style consistency
-   ✅ Naming conventions
-   ✅ Indentation and spacing
-   ✅ EditorConfig compliance

**Status Check Generated:**

-   `C# Lint / Check C# Formatting`

**To fix locally:**

```bash
dotnet format
```

---

### 3. **Unity Build Validation** (`unity-build.yml`)

Validates Unity project compilation by building for Windows platform.

**Triggers:**

-   `pull_request`: Changes to Unity files
    -   `Assets/**`
    -   `Packages/**`
    -   `ProjectSettings/**`
    -   `**.cs`, `**.csproj`
-   Runs on: `master`, `develop`, `feature/develop/SPWS-*`

**Features:**

-   ✅ LFS support with caching
-   ✅ Library caching for faster builds
-   ✅ Build logs uploaded as artifacts
-   ✅ Windows 64-bit target platform

**Status Check Generated:**

-   `Unity Build Validation / Build Unity Project (Windows)`

**Required Secrets:**

-   `UNITY_LICENSE`
-   `UNITY_EMAIL`
-   `UNITY_PASSWORD`

---

### 4. **Unity Edit Mode Tests** (`unity-tests-editmode.yml`)

Executes Unity Edit Mode tests (unit tests without Play Mode).

**Triggers:**

-   `pull_request`: Changes to Unity files
    -   `Assets/**`
    -   `Packages/**`
    -   `**.cs`
-   Runs on: `master`, `develop`

**Features:**

-   ✅ LFS support with caching
-   ✅ Library caching for faster execution
-   ✅ Code coverage reports
-   ✅ Test results uploaded as artifacts
-   ✅ Coverage reports with badges

**Status Check Generated:**

-   `Edit Mode Test Results`

**Required Secrets:**

-   `UNITY_LICENSE`
-   `UNITY_EMAIL`
-   `UNITY_PASSWORD`

**What it tests:**

-   Unit tests (logic validation)
-   Component validation
-   Data structures
-   Pure functions
-   Non-runtime code

---

### 5. **Unity Play Mode Tests** (`unity-tests-playmode.yml`)

Executes Unity Play Mode tests (integration and system tests).

**Triggers:**

-   `pull_request`: Changes to Unity files
    -   `Assets/**`
    -   `Packages/**`
    -   `**.cs`
-   Runs on: **`master` only** (final validation)

**Features:**

-   ✅ LFS support with caching
-   ✅ Library caching for faster execution
-   ✅ Code coverage reports
-   ✅ Test results uploaded as artifacts
-   ✅ Coverage reports with badges

**Status Check Generated:**

-   `Play Mode Test Results`

**Required Secrets:**

-   `UNITY_LICENSE`
-   `UNITY_EMAIL`
-   `UNITY_PASSWORD`

**What it tests:**

-   Integration tests (systems working together)
-   Runtime behavior
-   DOTS systems execution
-   Physics and collisions
-   Scene-based tests

---

## 🔗 Integration with Rulesets

Workflows are integrated with Branch Rulesets to enforce quality gates:

| Branch                   | Required Workflows                  |
| ------------------------ | ----------------------------------- |
| `master`                 | All 5 workflows                     |
| `develop`                | All except Play Mode (4 workflows)  |
| `feature/develop/SPWS-*` | PR Lint + Unity Build (2 workflows) |
| `feature/SPWS-*/SPWT-*`  | PR Lint only (1 workflow)           |
| `tech/SPWS-*/SPWT-*`     | PR Lint only (1 workflow)           |
| `bug/SPWS-*/SPWT-*`      | PR Lint only (1 workflow)           |

See [Branch Rulesets Documentation](../rulesets/README.md) for details.

---

## 🔑 Required Secrets

All Unity workflows require these GitHub Secrets to be configured:

### **UNITY_LICENSE**

Your Unity license file content (`.ulf` format).

**How to obtain:**

1. Activate Unity locally:

    ```bash
    # Windows
    "C:\Program Files\Unity\Hub\Editor\<VERSION>\Editor\Unity.exe" ^
      -quit -batchmode -serial <SERIAL_KEY> ^
      -username <EMAIL> -password <PASSWORD>
    ```

2. Get license file:

    ```bash
    # Windows
    C:\ProgramData\Unity\Unity_lic.ulf
    ```

3. Copy entire file contents to GitHub Secret

**More info:** [GameCI Activation Guide](https://game.ci/docs/github/activation)

### **UNITY_EMAIL**

The email address associated with your Unity account.

### **UNITY_PASSWORD**

The password for your Unity account.

---

## 🚀 Setup Instructions

### 1. Configure GitHub Secrets

1. Go to: **Settings** → **Secrets and variables** → **Actions**
2. Click: **New repository secret**
3. Add each secret:
    - Name: `UNITY_LICENSE`, Value: [entire .ulf file content]
    - Name: `UNITY_EMAIL`, Value: your-email@example.com
    - Name: `UNITY_PASSWORD`, Value: your-password

### 2. Import Branch Rulesets

Follow instructions in [Rulesets README](../rulesets/README.md) to import branch protection rules.

### 3. First Run

1. Create a PR to `develop`
2. All configured workflows will run automatically
3. Check workflow status in PR **Checks** tab
4. Fix any issues reported by workflows

---

## 📊 Workflow Execution Flow

```
┌─────────────────────────────────────┐
│ Developer creates Pull Request      │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ PR Lint validates title             │ ← Runs first
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ Parallel execution:                 │
│ - C# Lint (formatting)              │
│ - Unity Build (compilation)         │
│ - Unity Edit Mode Tests (unit)      │
│ - Unity Play Mode Tests (if master) │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ All checks pass → Ready to merge    │
│ Any check fails → Fix & push        │
└─────────────────────────────────────┘
```

---

## 🎨 Status Check Names Reference

Use these exact names when configuring branch protection rules:

| Workflow File              | Status Check Name                                        |
| -------------------------- | -------------------------------------------------------- |
| `pr-lint.yml`              | `PR Lint / Validate PR title`                            |
| `dotnet-lint.yml`          | `C# Lint / Check C# Formatting`                          |
| `unity-build.yml`          | `Unity Build Validation / Build Unity Project (Windows)` |
| `unity-tests-editmode.yml` | `Edit Mode Test Results`                                 |
| `unity-tests-playmode.yml` | `Play Mode Test Results`                                 |

---

## 🐛 Troubleshooting

### **Unity License Issues**

**Error:** `License activation failed`

**Solution:**

1. Verify `UNITY_LICENSE` secret is correctly set
2. Check license is not expired
3. Ensure license matches Unity version in project
4. For Personal license, follow [manual activation](https://game.ci/docs/github/activation)

### **Build Failures**

**Error:** `Compilation errors`

**Solution:**

1. Check uploaded build logs in workflow artifacts
2. Verify project builds locally
3. Check DOTS package versions compatibility
4. Review compiler errors in logs

### **Test Failures**

**Error:** `Tests failed`

**Solution:**

1. Download test results artifact from workflow
2. Run tests locally: Unity → Window → General → Test Runner
3. Check test coverage report for details
4. Fix failing tests and push again

### **Formatting Issues**

**Error:** `Code formatting issues detected`

**Solution:**

```bash
# Auto-fix formatting
dotnet format

# Commit and push
git add .
git commit -m "style: apply code formatting"
git push
```

### **LFS Issues**

**Error:** `Git LFS pull failed`

**Solution:**

1. Verify Git LFS is installed locally
2. Check `.gitattributes` is correct
3. Run `git lfs pull` to sync LFS files
4. Ensure LFS quota is not exceeded

---

## 📈 Performance Optimization

### **Caching Strategy**

All workflows implement aggressive caching:

1. **LFS Cache:** Cached by file hash
    - Key: `${{ runner.os }}-lfs-${{ hashFiles('.lfs-assets-id') }}`
2. **Library Cache:** Cached by Unity project files
    - Key: `Library-${{ runner.os }}-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}`

**Expected Times:**

-   First run: ~15-20 minutes (no cache)
-   Subsequent runs: ~5-8 minutes (with cache)
-   PR Lint: ~30 seconds
-   C# Lint: ~1-2 minutes

---

## 🔄 Updating Workflows

### **To update Unity version:**

1. Edit workflow file
2. Change `unityVersion: auto` to specific version (if needed)
3. Update cached Library key pattern if breaking changes

### **To add new status checks:**

1. Add workflow file to `.github/workflows/`
2. Define job name carefully (affects status check name)
3. Update branch rulesets with new status check name
4. Update this README with new workflow documentation

### **To modify triggers:**

1. Edit `on:` section in workflow file
2. Verify branch patterns match your naming convention
3. Test with draft PR to verify triggers work

---

## 📚 References

### **Official Documentation**

-   [GitHub Actions](https://docs.github.com/en/actions)
-   [GameCI Unity Builder](https://game.ci/docs/github/builder)
-   [GameCI Unity Test Runner](https://game.ci/docs/github/test-runner)
-   [actions/checkout](https://github.com/actions/checkout)
-   [actions/cache](https://github.com/actions/cache)

### **Project Documentation**

-   [Branch Rulesets](../rulesets/README.md)
-   [Commit Standards](../../.cursor/rules/general/commit-creation.mdc)
-   [PR Standards](../../.cursor/rules/general/pr-creation.mdc)
-   [Branch Patterns](../../.cursor/rules/general/shell-commands.mdc)

### **Unity DOTS Resources**

-   [Unity DOTS Documentation](https://docs.unity3d.com/Packages/com.unity.entities@latest)
-   [DOTS Best Practices](https://docs.unity3d.com/Packages/com.unity.entities@latest/manual/ecs_best_practices.html)

---

## 📝 Changelog

-   **2025-10-24** - Documentation created
    -   Documented all 5 workflows
    -   Added troubleshooting guide
    -   Included setup instructions
    -   Referenced integration with rulesets
    -   Added performance optimization tips
