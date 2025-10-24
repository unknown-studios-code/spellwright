# GitHub Rulesets for Spellwright

This directory contains the rulesets (rule sets) configured for the Spellwright repository, optimized for **solo development** following best practices.

## 📁 Structure

```
.github/rulesets/
├── branch-rulesets/      # Rules for branch protection
├── tag-rulesets/         # Rules for tag protection
├── push-rulesets/        # Rules for push validation
└── README.md             # This file
```

## 🎯 Configured Rulesets

### Branch Rulesets

#### 1. **Master Branch Protection** (`01-master-protection.json`)

Protection for the main production branch (adapted for solo development).

- ✅ Status checks must pass:
    - `Validate PR title`
    - `Validate EditorConfig Rules`
    - `Build Unity Project (Windows)`
    - `Unity Edit Mode Tests`
    - `Unity Play Mode Tests`
- ✅ Branches must be up to date before merging
- ✅ Require linear history
- ✅ Require signed commits
- ✅ Block force push
- ✅ Block deletion
- ✅ Block direct updates
- ⚠️ **No approval required** (solo development workflow)

**Target:** `master`

---

#### 2. **Develop Branch Protection** (`02-develop-protection.json`)

Protection for the main development branch.

- ✅ Status checks must pass:
    - `Validate PR title`
    - `Validate EditorConfig Rules`
    - `Build Unity Project (Windows)`
    - `Unity Edit Mode Tests`
- ✅ Branches must be up to date before merging
- ✅ Require linear history
- ✅ Block force push
- ✅ Block deletion
- ⚠️ **No approval required** (solo development workflow)

**Target:** `develop`

---

#### 3. **Story Branches Protection** (`03-story-branches-protection.json`)

Moderate protection for User Story branches.

- ✅ Block deletion (after merge)
- ⚠️ Allow force push (during development)
- ⚠️ Optional status checks:
    - `Validate PR title`
    - `Build Unity Project (Windows)`
- ⚠️ **No approval required** (solo development workflow)

**Target:** `feature/develop/SPWS-*`

---

#### 4. **Task Branches Protection** (`04-task-branches-protection.json`)

Basic protection for Task branches (Feature/Tech/Bug).

- ✅ Optional status checks:
    - `Validate PR title`
- ✅ Allow force push (flexibility during development)
- ✅ Allow deletion
- ⚠️ **No approval required** (solo development workflow)

**Targets:**

- `feature/SPWS-*/SPWT-*`
- `tech/SPWS-*/SPWT-*`
- `bug/SPWS-*/SPWT-*`

---

### Tag Rulesets

#### 5. **Release Tags Protection** (`01-release-tags-protection.json`)

Protection for release tags to ensure immutability.

- ✅ Restrict creation (admins only)
- ✅ Restrict deletion
- ✅ Restrict updates
- ✅ Require signed tags

**Target:** `v*` (e.g., v1.0.0, v1.2.3-beta)

---

### Push Rulesets

#### 6. **Sensitive Files Protection** (`01-sensitive-files-protection.json`)

Protection against committing sensitive files and validation of size/path.

**Blocked files:**

- ✅ Private keys: `*.key`, `*.pem`, `*.p12`, `*.pfx`
- ✅ Environment variables: `*.env`, `.env*`
- ✅ SSH keys: `**/id_rsa`, `**/id_dsa`, `**/.ssh/*`
- ✅ Credentials: `**/credentials.json`, `**/secrets.json`

**Limits:**

- ✅ Maximum file size: **100 MB**
- ✅ Maximum path length: **255 characters** (Windows compatibility)

**Target:** Entire repository (including forks)

---

## 📥 How to Import Rulesets

### Method 1: Via GitHub Web Interface (Recommended)

1. Access the repository: https://github.com/unknown-studios-code/spellwright
2. Navigate to **Settings** → **Rules** → **Rulesets**
3. Click on **New ruleset** → **Import a ruleset**
4. Select the desired JSON file from this folder
5. Review the settings and click **Create**

### Method 2: Via GitHub CLI

```bash
# Branch rulesets
gh api repos/unknown-studios-code/spellwright/rulesets \
  --method POST --input .github/rulesets/branch-rulesets/01-master-protection.json

gh api repos/unknown-studios-code/spellwright/rulesets \
  --method POST --input .github/rulesets/branch-rulesets/02-develop-protection.json

gh api repos/unknown-studios-code/spellwright/rulesets \
  --method POST --input .github/rulesets/branch-rulesets/03-story-branches-protection.json

gh api repos/unknown-studios-code/spellwright/rulesets \
  --method POST --input .github/rulesets/branch-rulesets/04-task-branches-protection.json

# Tag rulesets
gh api repos/unknown-studios-code/spellwright/rulesets \
  --method POST --input .github/rulesets/tag-rulesets/01-release-tags-protection.json

# Push rulesets (requires GitHub Team/Enterprise)
gh api repos/unknown-studios-code/spellwright/rulesets \
  --method POST --input .github/rulesets/push-rulesets/01-sensitive-files-protection.json
```

### Method 3: PowerShell Script (Import All)

```powershell
# Import all branch rulesets
Get-ChildItem .github/rulesets/branch-rulesets/*.json | ForEach-Object {
  Write-Host "Importing $($_.Name)..."
  gh api repos/unknown-studios-code/spellwright/rulesets `
    --method POST --input $_.FullName
}

# Import tag rulesets
Get-ChildItem .github/rulesets/tag-rulesets/*.json | ForEach-Object {
  Write-Host "Importing $($_.Name)..."
  gh api repos/unknown-studios-code/spellwright/rulesets `
    --method POST --input $_.FullName
}

# Import push rulesets (if you have GitHub Team/Enterprise)
Get-ChildItem .github/rulesets/push-rulesets/*.json | ForEach-Object {
  Write-Host "Importing $($_.Name)..."
  gh api repos/unknown-studios-code/spellwright/rulesets `
    --method POST --input $_.FullName
}
```

---

## 🔑 Bypass Actors

All rulesets are configured with bypass for:

- **Repository Admins** (actor_id: 5, RepositoryRole)
    - Bypass mode: `always`

To add specific users/teams to the bypass:

1. After importing, go to **Settings** → **Rules** → **Rulesets**
2. Edit the desired ruleset
3. In **Bypass list**, add:
    - Specific users
    - Specific teams
    - GitHub Apps

---

## 📊 Protection Hierarchy (Solo Development)

```
┌─────────────────────────────────────┐
│ master (High Protection)            │
│ - 1 approval (self-review)          │
│ - Signed commits                    │
│ - Linear history                    │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│ develop (High Protection)           │
│ - 1 approval (self-review)          │
│ - Linear history                    │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│ Story Branches (Medium Protection)  │
│ - 1 approval                        │
│ - Allow force push                  │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│ Task Branches (Low Protection)      │
│ - 1 approval                        │
│ - Maximum flexibility               │
└─────────────────────────────────────┘
```

---

## 💡 Solo Development: Self-Review

As you are the sole developer, the PR process works as follows:

1. **Create PR** from task branch to story/develop
2. **Self-review**: You approve the PR yourself
3. **Merge**: System allows merge after your approval

**Advantages of maintaining PRs even when working alone:**

- ✅ Clean and organized history
- ✅ Integration with Notion (PR links)
- ✅ Automatic CI/CD validation
- ✅ Visual code review before merge
- ✅ Structured documentation (PR body)
- ✅ Traceability (task → PR → commit)

---

## ⚠️ Important Notes

1. **Solo Development Workflow**
    - All branch rulesets **do not require manual PR approvals**
    - CI checks validate code quality automatically
    - Merge directly after CI passes (no review needed)
    - Labels and assignees are assigned automatically
    - **Rationale**: Streamlined workflow for solo development without sacrificing quality

2. **Push Rulesets** require **GitHub Team** or **Enterprise** plan
    - If you are on Free/Pro, the `01-sensitive-files-protection.json` file will fail on import
    - Consider using local Git hooks or GitHub Actions as an alternative

3. **Status Checks**
    - Rulesets use the **job name** from workflows, not the "Workflow / Job" format
    - GitHub UI displays: `Workflow Name / Job Name`
    - GitHub API reports: `Job Name` (only)
    - Rulesets must use: `Job Name` (the API format)
    - See [Status Check Names Reference](.github/workflows/README.md#-status-check-names-reference) for exact names

4. **Signed Commits**
    - Master branch requires signed commits/tags
    - Configure GPG keys: https://docs.github.com/en/authentication/managing-commit-signature-verification
    - **Optional**: Remove this rule if you prefer not to use it

5. **Conventional Commits**
    - PR titles are validated via `pr-lint.yml` workflow
    - Follows Conventional Commits specification
    - Required types: feat, fix, docs, refactor, perf, test, chore, ci, build, revert

---

## 🔄 Updating Rulesets

To update an existing ruleset:

1. Edit the local JSON file
2. Get the ruleset ID:
    ```bash
    gh api repos/unknown-studios-code/spellwright/rulesets
    ```
3. Execute:
    ```bash
    gh api repos/unknown-studios-code/spellwright/rulesets/{ID} \
      --method PUT --input {file.json}
    ```

---

## 📚 References

- [GitHub Docs - About Rulesets](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/about-rulesets)
- [GitHub Ruleset Recipes](https://github.com/github/ruleset-recipes)
- [Spellwright - Commit Standards](.cursor/rules/general/commit-creation.mdc)
- [Spellwright - PR Standards](.cursor/rules/general/pr-creation.mdc)
- [Spellwright - Branch Patterns](.cursor/rules/general/shell-commands.mdc)

---

## 📝 Changelog

- **2025-10-24** - Removed approval requirement for solo development
    - Removed `required_approving_review_count` from all branch rulesets
    - Simplified workflow: merge directly after CI passes
    - No manual review/approval needed
    - Updated documentation to reflect solo development workflow
    - **Rationale**: Streamline process without sacrificing quality (CI validates everything)
- **2025-10-24** - Status check names corrected
    - Updated all rulesets to use job names (API format) instead of "Workflow / Job" format
    - Corrected check names: `Validate PR title`, `Validate EditorConfig Rules`, `Build Unity Project (Windows)`, `Unity Edit Mode Tests`, `Unity Play Mode Tests`
    - Added documentation explaining GitHub UI vs API naming differences
    - Fixed "Waiting for status" issue in Pull Requests
- **2025-10-24** - EditorConfig workflow integration
    - Added `Validate EditorConfig Rules` to master and develop rulesets
    - Replaced C# Lint with EditorConfig Lint for basic formatting validation
    - Updated documentation to reflect new status check
- **2025-10-24** - Initial rulesets configuration
    - Created branch rulesets for master, develop, story, and task branches
    - Created tag ruleset for release tags
    - Created push ruleset for sensitive files protection
    - Configured status checks and branch protection rules
    - Documented all rulesets and usage instructions
