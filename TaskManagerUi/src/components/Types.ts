export interface Organization {
  id?: string;
  name: string;
  description?: string;
  abbreviation?: string;
  projects?: Project[];
  accounts?: AccountDetails[];
  ownerId: string;
  owner?: AccountDetails
  createDate?: string;
  modifyDate?: string;
}

export interface Project {
  id?: string;
  title: string;
  description: string;
  ownerId: string;
  owner?: AccountDetails;
  organizationId: string;
  statuses?: Status[];
  tickets?: Task[];
  createDate?: string;
  modidyDate?: string;
}

export interface Status {
  typeId: number;
  typeName: string;
  statusId: number;
  statusName: string;
  order: number;
}

export interface ProjectSingleStatusDto {
  projectId: string;
  status: Status
}

export interface Task {
  id: string;
  title: string;
  description: string;
  statusId?: TaskStatus["statusId"];
  statusName?: TaskStatus["statusName"];
  typeId?: number;
  typeName?: string;
  reporterId?: string;
  assigneeId?: string;
  reporter?: AccountDetails;
  assignee?: AccountDetails;
  projectId?: string;
  parentId?: string;
  startDate?: Date;
  dueDate?: Date;
  spentTime?: string;
  estimate?: string;
  organizationId?:string;
  childIssues?: Task[]
  parentTicket?: Task;
  isCreatedByAi?: boolean;
  createDate?: string;
  modifyDate?: string;
}

export interface TicketAiView {
  title: string;
  description: string;
  type: number;
  reporterId?: string;
  assigneeId?: string;
  isCreatedByAi?: boolean;
  parentName?: string;
  projectId?: string;
}

export interface TaskStatus {
  statusId: number;
  statusName: string;
}

export interface TaskHistory {
  id: string;
  taskId: string;
  eventName: string;
  previousState: string;
  newState: string;
  author: string;
  createDate: Date;
  modifyDate: Date;
}

export interface TaskComment {
  id: string,
  ticketId: string,
  accountId: string,
  account?: AccountDetails,
  message: string,
  createDate?: Date
  modifyDate?: Date
}

export interface CreateTask {
  title: string;
  description: string;
  type: number | null;
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
  id?: string;
  email?: string;
  firstName: string;
  lastName: string;
  createDate?: Date;
  modifyDate?: Date;
}

export interface ChatMessage {
  role: string;
  content: string;
  IsAutomatedTicketCreationFlag: boolean;
  createDate?: Date;
}

export interface AiThreadDetails {
  id?: string;
  name?: string;
  organziationId?: string;
  accountId?: string;
  createDate?: Date;
}