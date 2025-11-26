import { exec } from "./git";

export interface PRInfo {
    hasMergedPR: boolean;
    prNumber?: number;
    prUrl?: string;
}

export function getMergedPRForBranch(branchName: string): PRInfo {
    const result = exec(`gh pr list --state merged --head "${branchName}" --json number,url --limit 1`, { silent: true });

    if (!result.success || !result.output) {
        return { hasMergedPR: false };
    }

    try {
        const prs = JSON.parse(result.output);
        if (prs.length > 0) {
            return {
                hasMergedPR: true,
                prNumber: prs[0].number,
                prUrl: prs[0].url,
            };
        }
    } catch {
        // JSON parse error, treat as no PR
    }

    return { hasMergedPR: false };
}
