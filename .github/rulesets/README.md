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

-   ✅ **1 approval** required on PRs
-   ✅ Dismiss stale reviews when new commits are pushed
-   ✅ Require review thread resolution
-   ✅ Status checks must pass:
    -   `PR Lint / Validate PR title`
    -   `C# Lint / Check C# Formatting`
    -   `Unity Build Validation / Build Unity Project (Windows)`
    -   `Edit Mode Test Results`
    -   `Play Mode Test Results`
-   ✅ Branches must be up to date before merging
-   ✅ Require linear history
-   ✅ Require signed commits
-   ✅ Block force push
-   ✅ Block deletion
-   ✅ Block direct updates

**Target:** `master`

---

#### 2. **Develop Branch Protection** (`02-develop-protection.json`)

Protection for the main development branch.

-   ✅ **1 approval** required on PRs
-   ✅ Dismiss stale reviews when new commits are pushed
-   ✅ Require review thread resolution
-   ✅ Status checks must pass:
    -   `PR Lint / Validate PR title`
    -   `C# Lint / Check C# Formatting`
    -   `Unity Build Validation / Build Unity Project (Windows)`
    -   `Edit Mode Test Results`
-   ✅ Branches must be up to date before merging
-   ✅ Require linear history
-   ✅ Block force push
-   ✅ Block deletion

**Target:** `develop`

---

#### 3. **Story Branches Protection** (`03-story-branches-protection.json`)

Moderate protection for User Story branches.

-   ✅ **1 approval** required on PRs
-   ✅ Block deletion (after merge)
-   ⚠️ Allow force push (during development)
-   ⚠️ Optional status checks:
    -   `PR Lint / Validate PR title`
    -   `Unity Build Validation / Build Unity Project (Windows)`

**Target:** `feature/develop/SPWS-*`

---

#### 4. **Task Branches Protection** (`04-task-branches-protection.json`)

Basic protection for Task branches (Feature/Tech/Bug).

-   ✅ **1 approval** required on PRs
-   ✅ Optional status checks:
    -   `PR Lint / Validate PR title`
-   ✅ Allow force push (flexibility during development)
-   ✅ Allow deletion

**Targets:**

-   `feature/SPWS-*/SPWT-*`
-   `tech/SPWS-*/SPWT-*`
-   `bug/SPWS-*/SPWT-*`

---

### Tag Rulesets

#### 5. **Release Tags Protection** (`01-release-tags-protection.json`)

Protection for release tags to ensure immutability.

-   ✅ Restrict creation (admins only)
-   ✅ Restrict deletion
-   ✅ Restrict updates
-   ✅ Require signed tags

**Target:** `v*` (e.g., v1.0.0, v1.2.3-beta)

---

### Push Rulesets

#### 6. **Sensitive Files Protection** (`01-sensitive-files-protection.json`)

Protection against committing sensitive files and validation of size/path.

**Blocked files:**

-   ✅ Private keys: `*.key`, `*.pem`, `*.p12`, `*.pfx`
-   ✅ Environment variables: `*.env`, `.env*`
-   ✅ SSH keys: `**/id_rsa`, `**/id_dsa`, `**/.ssh/*`
-   ✅ Credentials: `**/credentials.json`, `**/secrets.json`

**Limits:**

-   ✅ Maximum file size: **100 MB**
-   ✅ Maximum path length: **255 characters** (Windows compatibility)

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

-   **Repository Admins** (actor_id: 5, RepositoryRole)
    -   Bypass mode: `always`

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

-   ✅ Clean and organized history
-   ✅ Integration with Notion (PR links)
-   ✅ Automatic CI/CD validation
-   ✅ Visual code review before merge
-   ✅ Structured documentation (PR body)
-   ✅ Traceability (task → PR → commit)

---

## ⚠️ Important Notes

1. **Push Rulesets** require **GitHub Team** or **Enterprise** plan

    - If you are on Free/Pro, the `01-sensitive-files-protection.json` file will fail on import
    - Consider using local Git hooks or GitHub Actions as an alternative

2. **Status Checks**

    - Rulesets include empty `required_status_checks`
    - Configure your GitHub Actions CI/CD first
    - Then edit the rulesets to add specific checks

3. **Signed Commits**

    - Master branch requires signed commits/tags
    - Configure GPG keys: https://docs.github.com/en/authentication/managing-commit-signature-verification
    - **Optional**: Remove this rule if you prefer not to use it

4. **Conventional Commits**
    - Consider adding a GitHub Action to validate commit format
    - Example: https://github.com/amannn/action-semantic-pull-request

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

-   [GitHub Docs - About Rulesets](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/about-rulesets)
-   [GitHub Ruleset Recipes](https://github.com/github/ruleset-recipes)
-   [Spellwright - Commit Standards](.cursor/rules/general/commit-creation.mdc)
-   [Spellwright - PR Standards](.cursor/rules/general/pr-creation.mdc)
-   [Spellwright - Branch Patterns](.cursor/rules/general/shell-commands.mdc)

---

## 📝 Changelog

-   **2025-10-24** - Adjustment for solo development
    -   Reduced required_approving_review_count from 2 to 1 on master
    -   Removed require_last_push_approval (allows self-review)
    -   Maintained branch protection and linear history
-   **2025-10-24** - Initial creation of rulesets for Spellwright
    -   Branch protection for master, develop, story and task branches
    -   Tag protection for releases
    -   Push rulesets for sensitive files
