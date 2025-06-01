import { taskManagerApiClient } from './httpClient'
import type { CreateTask } from '../components/Types'
import { useSafeAuth } from '../hooks/useSafeAuth';

export const useOrganizationApi = () => {
  const { user } = useSafeAuth();
  const defaultPath = '/api/organization'
  const apiPromise = taskManagerApiClient(
    () => user?.access_token,
    () => {
      const orgId = localStorage.getItem('organizationId');
      return orgId === null ? undefined : orgId;
    },
    () => {
      const projectId = localStorage.getItem('projectId');
      return projectId === null ? undefined : projectId;
    }
  )

  return {
    getOrganizationProjects: async () => (await apiPromise).get(`${defaultPath}/account/default`),
    createOrganizationProjects: async (data: any) => (await apiPromise).post(`${defaultPath}/create`, data),
    editOrganization: async (data: any) => (await apiPromise).post(`${defaultPath}/edit`, data),
    getOrganizationAccounts: async () => (await apiPromise).get(`${defaultPath}/info/accounts`),
  }
}

export const useProjectApi = () => {
  const { user } = useSafeAuth();
  const defaultPath = '/api/project';
  const apiPromise = taskManagerApiClient(
    () => user?.access_token,
    () => {
      const orgId = localStorage.getItem('organizationId');
      return orgId === null ? undefined : orgId;
    },
    () => {
      const projectId = localStorage.getItem('projectId');
      return projectId === null ? undefined : projectId;
    }
  );

  return {
    // Fetch all projects for the organization
    getAllProjects: async () => (await apiPromise).get(`${defaultPath}/all`),

    // Fetch all projects with their tasks
    getAllProjectsWithTasks: async () => (await apiPromise).get(`${defaultPath}/all/tasks`),

    // Fetch a specific project by its ID
    getProjectById: async (projectId: string) => (await apiPromise).get(`${defaultPath}/${projectId}`),

    // Fetch a specific project with its tasks by project ID
    getProjectWithTasksById: async (projectId: string) =>
      (await apiPromise).get(`${defaultPath}/${projectId}/tasks`),

    // Fetch accounts associated with a specific project
    getAccountsByProjectId: async (projectId: string) =>
      (await apiPromise).get(`${defaultPath}/${projectId}/accounts`),

    // Create a new project
    createProject: async (data: any) => (await apiPromise).post(`${defaultPath}/create`, data),

    // Edit an existing project
    editProject: async (data: any) => (await apiPromise).put(`${defaultPath}/edit/`, data),

    // Delete a project by its ID
    deleteProject: async (projectId: string) =>
      (await apiPromise).delete(`${defaultPath}/delete/${projectId}`),
  };
};

export const useTaskApi = () => {
  const { user } = useSafeAuth();
  const defaultPath = '/api/task';
  const apiPromise = taskManagerApiClient(
    () => user?.access_token,
    () => {
      const orgId = localStorage.getItem('organizationId');
      return orgId === null ? undefined : orgId;
    },
    () => {
      const projectId = localStorage.getItem('projectId');
      return projectId === null ? undefined : projectId;
    }
  );

  return {
    // Fetch all tasks in the organization
    getAllTasks: async () => (await apiPromise).get(`${defaultPath}/all`),

    // Fetch all tasks in a specific project
    getTasksByProjectId: async (projectId: string) =>
      (await apiPromise).get(`${defaultPath}/all/${projectId}`),

    // Fetch task details by task ID
    getTaskById: async (taskId: string) =>
      (await apiPromise).get(`${defaultPath}/details/${taskId}`),

    // Create a new task
    createTask: async (data: CreateTask) =>
      (await apiPromise).post(`${defaultPath}/create`, data),

    // Edit an existing task by task ID
    editTask: async (taskId: string, data: any) =>
      (await apiPromise).post(`${defaultPath}/edit/${taskId}`, data),

    // // Change the status of a task
    // changeTaskStatus: async (taskId: string, statusId: number) =>
    //   (await apiPromise).put(`${defaultPath}/edit/${taskId}/task/${statusId}`),

    // Add a parent task to a task
    addParentToTask: async (data: { parentId: string; taskId: string }) =>
      (await apiPromise).post(`${defaultPath}/parent`, data),

    getTaskHistory: async ( taskId: string) =>
      (await apiPromise).get(`${defaultPath}/history/${taskId}`),

    // Delete a task by task ID
    deleteTask: async (taskId: string) =>
      (await apiPromise).delete(`${defaultPath}/delete/${taskId}`),
  };
};