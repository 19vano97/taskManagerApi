import { taskManagerAxios } from './httpClient';
import { useSafeAuth } from '../hooks/useSafeAuth';
import { useEffect } from 'react';
import { configureAxiosAuth } from './httpClient';
import type {
  AiThreadDetails,
  ChatMessage,
  CreateTask,
  Organization,
  Project,
  Task,
  TaskHistory,
  TicketAiView,
} from '../components/Types';

const getOrgId = () => localStorage.getItem('organizationId') || undefined;
const getProjId = () => localStorage.getItem('projectId') || undefined;

export const useOrganizationApi = () => {
  const auth = useSafeAuth();

  useEffect(() => {
    configureAxiosAuth(
      taskManagerAxios,
      () => auth.user?.access_token,
      getOrgId,
      () => { auth.signoutRedirect(); }
    );
  }, [auth.user]);

  const path = '/api/organization';

  return {
    getAllOrganizationProjects: async () => 
      (await taskManagerAxios.get<Organization[]>(`${path}/details/me`)),
    getOrganizationProjectsById: async (organizationId: string) => 
      (await taskManagerAxios.get<Organization>(`${path}/${organizationId}/details/`)),
    postCreateOrganization: async (data: Organization) => 
      (await taskManagerAxios.post<Organization>(`${path}/create`, data)),
    postEditOrganization: async (id: string, data: Organization) => 
      (await taskManagerAxios.post<Organization>(`${path}/${id}/edit`, data)),
    postAddAccountToOrganization: async (organizationId: string, accountId: string) => 
      (await taskManagerAxios.post<Organization>(`${path}/details/${organizationId}/new-member/${accountId}`)),
    getOrganizationAccounts: async (organizationId: string) => 
      (await taskManagerAxios.get<Organization>(`${path}/${organizationId}/accounts`))
  };
};

export const useProjectApi = () => {
  const path = '/api/project';
  const auth = useSafeAuth();

  useEffect(() => {
    configureAxiosAuth(
      taskManagerAxios,
      () => auth.user?.access_token,
      getOrgId,
      () => { auth.signoutRedirect(); }
    );
  }, [auth.user]);

  return {
    getAllProjectsWithTasks: async (organizationId: string) => 
      (await taskManagerAxios.get<Project>(`${path}/all/${organizationId}`)),
    getProjectById: async (id: string) => 
      (await taskManagerAxios.get<Project>(`${path}/${id}`)),
    getProjectWithTasksById: async (id: string) => 
      (await taskManagerAxios.get<Project>(`${path}/${id}/tasks`)),
    createProject: async (data: Project) => 
      (await taskManagerAxios.post<Project>(`${path}/create`, data)),
    editProject: async (data: Project, projectId: string) => 
      (await taskManagerAxios.post<Project>(`${path}/${projectId}/edit`, data)),
    deleteProject: async (projectId: string) => {
      await taskManagerAxios.delete(`${path}/${projectId}/delete`);
    }
  };
};

export const useTaskApi = () => {
  const path = '/api/task';

  return {
    getAllTasksByOrganization: async (organizationId: string) => 
      (await taskManagerAxios.get<Task[]>(`${path}/all/${organizationId}/organization`)),
    getTasksByProject: async (id: string) => 
      (await taskManagerAxios.get<Task[]>(`${path}/all/${id}/project`)),
    getTaskById: async (taskId: string) => 
      (await taskManagerAxios.get<Task>(`${path}/${taskId}/details`)),
    createTask: async (data: CreateTask) =>
      (await taskManagerAxios.post<Task>(`${path}/create`, data)),
    createTicketsForAi: async (data: TicketAiView[]) => 
        (await taskManagerAxios.post<Task>(`${path}/create/ai/list`, data)),
    editTask: async (taskId: string, data: any) =>
      (await taskManagerAxios.post(`${path}/${taskId}/edit`, data)),
    addParentToTask: async (data: { parentId: string; taskId: string }) =>
      (await taskManagerAxios.post(`${path}/parent`, data)),
    getTaskHistory: async (taskId: string) => 
      (await taskManagerAxios.get<TaskHistory[]>(`${path}/${taskId}/history`)),
    deleteTask: async (taskId: string) => 
      (await taskManagerAxios.delete<void>(`${path}${taskId}/delete`)),
  };
};

export const useAiChatApi = () => {
  const path = '/api/ai';
  const auth = useSafeAuth();

  useEffect(() => {
    configureAxiosAuth(
      taskManagerAxios,
      () => auth.user?.access_token,
      getOrgId,
      () => { auth.signoutRedirect(); }
    );
  }, [auth.user]);

  return {
    getChatHistoryByThreadId: async (threadId: string) =>
      (await taskManagerAxios.get<ChatMessage[]>(`${path}/chat/${threadId}/history`)),
    deleteThreadById: async (threadId: string) =>
      (await taskManagerAxios.delete<void>(`${path}/thread/${threadId}/delete`)),
    createNewThread: async (data: AiThreadDetails ) =>
      (await taskManagerAxios.post<AiThreadDetails>(`${path}/thread/create`, data)),
    postSendMessageToChat: async (data: ChatMessage, threadId: string) =>
      (await taskManagerAxios.post<ChatMessage>(`${path}/chat/${threadId}/message`, data)),
    getThreadInfo: async (threadId: string) =>
      (await taskManagerAxios.get<AiThreadDetails>(`${path}/thread/${threadId}/info`)),
    getAllThreads: async () =>
      (await taskManagerAxios.get<AiThreadDetails[]>(`${path}/thread/all`)),
  };
};