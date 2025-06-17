export interface Organization {
  id: string;
  name: string;
  projects: Project[];
  accounts: string[];
  createDate: string;
  modifyDate: string;
  ownerId: string;
}

export interface OrganizationAccounts {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  organizationId: string;
  createDate: string;
  modifyDate: string;
}

export interface Project {
  id: string;
  title: string;
  description: string;
  ownerId: string;
  organizationId: string;
  statuses: Status[];
  createDate: string;
  modidyDate: string;
}

export interface Status {
  typeId: number;
  typeName: string;
  statusId: number;
  statusName: string;
  order: number;
}

export interface Task {
  id: string;
  title: string;
  description: string;
  statusId: TaskStatus["statusId"];
  statusName: TaskStatus["statusName"];
  type: number;
  typeName: string;
  reporterId: string;
  assigneeId: string;
  projectId: string;
  parentId: string;
  childIssues?: Task[]
  createDate: string;
  modifyDate: string;
}

export interface TaskStatus {
  statusId: number;
  statusName: string;
}

export interface TaskEdit {
  id: string;
  title: string | null;
  description: string | null;
  statusId: number | null;
  statusName: string | null;
  type: number | null;
  typeName: string | null;
  reporterId: string | null;
  assigneeId: string | null;
  projectId: string | null;
  parentId: string | null;
}

export interface TaskHistory {
  id: string;
  taskId: string;
  eventName: string;
  previousState:string;
  newState: string;
  author: string;
  createDate: Date;
  modifyDate: string;
}

export interface ProjectResponse {
  project: Project;
  tasks: Task[];
}

export interface CreateTask {
  title: string;
  description: string;
  type: number  | null;
  reporterId: string;
  assigneeId: string;
  projectId: string;
  parentId?: string | null;
};

export interface TaskType {
  id: number;
  name: string;
};

export interface TaskHistoryType {
  id: number;
  name: string;
}


export interface AccountDetails {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
}