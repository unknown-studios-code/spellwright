# GitHub Skills

TypeScript-based GitHub integration skills implementing Clean Architecture and SOLID principles. These skills are designed to be executed by AI agents via `ts-node` (CLI) or imported as a library.

## Architecture

The project follows Clean Architecture patterns with strict separation of concerns:

- **Domain Layer** (`src/domain/`): Core business entities and interfaces with no external dependencies
- **Application Layer** (`src/application/`): Use cases implementing business logic
- **Infrastructure Layer** (`src/infrastructure/`): External API integration using Octokit
- **Presentation Layer**:
    - **CLI** (`cli/*.ts`): Entry points for manual execution via terminal
    - **Library** (`index.ts`): Programmatic API for agent composition

## Setup

### 1. Install Dependencies

```bash
cd .agent/skills
npm install
```

### 2. Configure GitHub Token

Create a `.env` file in `.agent/skills/`:

```properties
GITHUB_TOKEN=ghp_your_personal_access_token
```

Get your token from: https://github.com/settings/tokens

## Usage

### Programmatic API (Recommended for Agents)

Agents should import functions from `github/index.ts` to chain operations or process data programmatically.

```typescript
import { listLabels, createIssue } from "./.agent/skills/github/index";

async function task() {
    const labels = await listLabels("owner", "repo");
    // Use data directly
    console.log(labels.data);
}
```

### CLI (Manual Testing)

All skills output JSON with the following structure when run via CLI:

```json
{
  "success": boolean,
  "data": any,
  "error": string (if success is false)
}
```

**Example:**

```bash
npx ts-node github/cli/list_labels.ts owner repo
```

---

## Available Skills

### 🔍 Code & File Operations

- **Search Code**: `github/cli/search_code.ts` / `searchCode()`
- **Get File Content**: `github/cli/get_file_content.ts` / `getFileContent()`

### 👤 User Operations

- **Get Current User**: `github/cli/get_current_user.ts` / `getCurrentUser()`
- **Get User**: `github/cli/get_user.ts` / `getUser()`

### 📝 Issue Operations

- **List Issues**: `github/cli/list_issues.ts` / `listIssues()`
- **Search Issues**: `github/cli/search_issues.ts` / `searchIssues()`
- **Create Issue**: `github/cli/create_issue.ts` / `createIssue()`
- **Update Issue**: `github/cli/update_issue.ts` / `updateIssue()`

### 🏷️ Label Operations

- **List Labels**: `github/cli/list_labels.ts` / `listLabels()`
- **Add Labels**: `github/cli/add_labels.ts` / `addLabels()`
- **Remove Label**: `github/cli/remove_label.ts` / `removeLabel()`

### 🔀 Pull Request Operations

- **List Pull Requests**: `github/cli/list_pull_requests.ts` / `listPullRequests()`
- **Create Pull Request**: `github/cli/create_pull_request.ts` / `createPullRequest()`
- **Merge Pull Request**: `github/cli/merge_pull_request.ts` / `mergePullRequest()`

### 👁️ Pull Request Review Operations

- **List PR Reviews**: `github/cli/list_pr_reviews.ts` / `listPrReviews()`
- **Create PR Review**: `github/cli/create_pr_review.ts` / `createPrReview()`

### 🌿 Branch Operations

- **List Branches**: `github/cli/list_branches.ts` / `listBranches()`
- **Create Branch**: `github/cli/create_branch.ts` / `createBranch()`

### 📜 Commit Operations

- **List Commits**: `github/cli/list_commits.ts` / `listCommits()`
- **Get Commit**: `github/cli/get_commit.ts` / `getCommit()`

### ⚙️ GitHub Actions Operations

- **List Workflow Runs**: `github/cli/list_workflow_runs.ts` / `listWorkflowRuns()`

### 🔔 Notification Operations

- **List Notifications**: `github/cli/list_notifications.ts` / `listNotifications()`

### 🍴 Repository Operations

- **Fork Repository**: `github/cli/fork_repository.ts` / `forkRepository()`
- **Star Repository**: `github/cli/star_repository.ts` / `starRepository()`

---

## Development

### Build

Compile TypeScript to JavaScript:

```bash
cd .agent/skills
npm run build
```

Output will be in the `dist/` directory.
