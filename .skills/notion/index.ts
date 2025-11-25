import { AppendBlockChildrenUseCase } from "./src/application/append-block-children";
import { CreateCommentUseCase } from "./src/application/create-comment";
import { CreateDatabaseUseCase } from "./src/application/create-database";
import { CreatePageUseCase } from "./src/application/create-page";
import { DeleteBlockUseCase } from "./src/application/delete-block";
import { DuplicatePageUseCase } from "./src/application/duplicate-page";
import { GetBlockUseCase } from "./src/application/get-block";
import { GetBotInfoUseCase } from "./src/application/get-bot-info";
import { GetDatabaseUseCase } from "./src/application/get-database";
import { GetPageUseCase } from "./src/application/get-page";
import { GetPageByTaskIdUseCase } from "./src/application/get-page-by-task-id";
import { GetPageContentUseCase } from "./src/application/get-page-content";
import { GetUserUseCase } from "./src/application/get-user";
import { ListCommentsUseCase } from "./src/application/list-comments";
import { ListTeamspacesUseCase } from "./src/application/list-teams";
import { ListUsersUseCase } from "./src/application/list-users";
import { MovePageUseCase } from "./src/application/move-page";
import { SearchUseCase } from "./src/application/search";
import { UpdateBlockUseCase } from "./src/application/update-block";
import { UpdateDatabaseUseCase } from "./src/application/update-database";
import { UpdatePageUseCase } from "./src/application/update-page";
import { NotionAdapter } from "./src/infrastructure/notion-adapter";

const adapter = new NotionAdapter();

export async function appendBlockChildren(blockId: string, children: { type: string; content: string }[]) {
    const useCase = new AppendBlockChildrenUseCase(adapter);
    return await useCase.execute({ blockId, children });
}

export async function createComment(pageId: string, content: string) {
    const useCase = new CreateCommentUseCase(adapter);
    return await useCase.execute({ pageId, content });
}

export async function createDatabase(parentId: string, title: string, properties: Record<string, any>) {
    const useCase = new CreateDatabaseUseCase(adapter);
    return await useCase.execute({ parentId, title, properties });
}

export async function createPage(parentId: string, parentType: "page" | "database", title: string, content?: string) {
    const useCase = new CreatePageUseCase(adapter);
    return await useCase.execute({ parentId, parentType, title, content });
}

export async function deleteBlock(blockId: string) {
    const useCase = new DeleteBlockUseCase(adapter);
    return await useCase.execute({ blockId });
}

export async function duplicatePage(pageId: string) {
    const useCase = new DuplicatePageUseCase(adapter);
    return await useCase.execute({ pageId });
}

export async function getBlock(blockId: string) {
    const useCase = new GetBlockUseCase(adapter);
    return await useCase.execute({ blockId });
}

export async function getBotInfo() {
    const useCase = new GetBotInfoUseCase(adapter);
    return await useCase.execute({});
}

export async function getDatabase(databaseId: string) {
    const useCase = new GetDatabaseUseCase(adapter);
    return await useCase.execute({ databaseId });
}

export async function getPage(pageId: string) {
    const useCase = new GetPageUseCase(adapter);
    return await useCase.execute({ pageId });
}

export async function getPageByTaskId(taskId: string) {
    const useCase = new GetPageByTaskIdUseCase(adapter);
    return await useCase.execute({ taskId });
}

export async function getPageContent(blockId: string) {
    const useCase = new GetPageContentUseCase(adapter);
    return await useCase.execute({ blockId });
}

export async function getUser(userId: string) {
    const useCase = new GetUserUseCase(adapter);
    return await useCase.execute({ userId });
}

export async function listComments(blockId: string) {
    const useCase = new ListCommentsUseCase(adapter);
    return await useCase.execute({ blockId });
}

export async function listTeams() {
    const useCase = new ListTeamspacesUseCase(adapter);
    return await useCase.execute({});
}

export async function listUsers() {
    const useCase = new ListUsersUseCase(adapter);
    return await useCase.execute({});
}

export async function movePage(pageId: string, newParentId: string) {
    const useCase = new MovePageUseCase(adapter);
    return await useCase.execute({ pageId, newParentId });
}

export async function search(query: string, sort?: "last_edited_time" | "relevance") {
    const useCase = new SearchUseCase(adapter);
    return await useCase.execute({ query, sort });
}

export async function updateBlock(blockId: string, content: string) {
    const useCase = new UpdateBlockUseCase(adapter);
    return await useCase.execute({ blockId, content });
}

export async function updateDatabase(databaseId: string, updates: { title?: string; properties?: Record<string, any> }) {
    const useCase = new UpdateDatabaseUseCase(adapter);
    return await useCase.execute({ databaseId, ...updates });
}

export async function updatePage(pageId: string, properties?: Record<string, any>, archived?: boolean) {
    const useCase = new UpdatePageUseCase(adapter);
    return await useCase.execute({ pageId, properties, archived });
}
