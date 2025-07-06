import { Button, Card, Container, Fieldset, Flex, Input, Paper, Text } from "@mantine/core";
import { Navigate, useParams, useNavigate } from "react-router-dom"
import { TaskTypeDropdown } from "../components/DropdownData/TaskTypeDropdown";
import { ProjectTaskStatusesDdData } from "../components/DropdownData/ProjectTaskStatusesDdData";
import { ProjectDropdownData } from "../components/DropdownData/ProjectDropdownData";
import { AccountDropdown } from "../components/DropdownData/AccountDropdown";
import { TaskDropdown } from "../components/DropdownData/TaskDropdown";
import { TaskAdditionalInfo } from "../components/TicketView/TaskAdditionalInfo";
import type { AccountDetails, Project, Status, Task, TaskType } from "../components/Types";
import { useEffect, useState } from "react";
import { taskTypesConst } from "../components/TicketView/TaskTypeBadge";
import { useEditor } from '@tiptap/react';
import Highlight from '@tiptap/extension-highlight';
import StarterKit from '@tiptap/starter-kit';
import Underline from '@tiptap/extension-underline';
import TextAlign from '@tiptap/extension-text-align';
import Superscript from '@tiptap/extension-superscript';
import SubScript from '@tiptap/extension-subscript';
import Link from '@tiptap/extension-link';
import { useOrganizationApi, useTaskApi } from "../api/taskManagerApi";
import { useIdentityServerApi } from "../api/IdentityServerApi";
import { TicketDesciption } from "../components/TicketView/TicketDesciption";
import { LoaderMain } from "../components/LoaderMain";
import { TableTickets } from "../components/TicketView/TableTickets";
import { TaskDialog } from "../components/TicketView/TaskDialog";
import NotFoundPage from "./NotFoundPage";
import { useOrgLocalStorage } from "../hooks/useOrgLocalStorage";

const TaskPage = () => {
    const params = useParams<{ id?: string }>();
    const id = params?.id;
    const navigate = useNavigate();
    const { getTaskById, getAllTasksByOrganization } = useTaskApi();
    const { getOrganizationProjectsById } = useOrganizationApi();
    const { editTask } = useTaskApi();
    const { getOrganizationAccounts } = useOrganizationApi();
    const { getAllAccountDetails } = useIdentityServerApi();
    const [taskDetails, setTaskDetails] = useState<Task | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [taskTitle, setTaskTitle] = useState<string>('');
    const [taskDescription, setTaskDescription] = useState<string>('');
    const [selectedProjectId, setSelectedProjectId] = useState<string | null>(null);
    const [selectedParentTaskId, setSelectedParentTaskId] = useState<string | null>(null);
    const [taskType, setTaskType] = useState<TaskType | null>(null);
    const [projects, setProjects] = useState<Project[] | null>(null);
    const [accounts, setAccounts] = useState<AccountDetails[]>([]);
    const [reporterId, setReporterId] = useState<AccountDetails | null>(null);
    const [assigneeId, setAssigneeId] = useState<AccountDetails | null>(null);
    const [taskStatus, setTaskStatus] = useState<Status | null>(null);
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
    const [dialogOpen, setDialogOpen] = useState(false);
    const [childTasks, setChildTasks] = useState<Task[] | null>(null);
    const [tasks, setTasks] = useState<Task[] | null>(null);
    const [accountsLoading, setAccountsLoading] = useState(false);
    const [isEditingTitle, setIsEditingTitle] = useState(false);
    const [originalTitle, setOriginalTitle] = useState('');
    const [isEditingDescription, setIsEditingDescription] = useState(false);
    const [originalDescription, setOriginalDescription] = useState('');
    const taskTypes = taskTypesConst;
    const editor = useEditor({
        extensions: [
            StarterKit,
            Underline,
            Link,
            Superscript,
            SubScript,
            Highlight,
            TextAlign.configure({ types: ['heading', 'paragraph'] }),
        ],
    });
    const openTaskDialog = (task: Task) => {
        setSelectedTask(task);
        setDialogOpen(true);
    };

    const closeTaskDialog = () => {
        setSelectedTask(null);
        setDialogOpen(false);
    };

    if (!id) return <NotFoundPage />;

    useEffect(() => {
        if (!id) return;

        const fetchTaskDetails = async () => {
            setLoading(true);
            try {
                const data = await getTaskById(id);
                setTaskDetails(data.data);
            } catch {
                setError('Failed to load task details');
            } finally {
                setLoading(false);
            }
        };

        fetchTaskDetails();
    }, [id]);


    useEffect(() => {
        if (!taskDetails) return;

        const fetchProjects = async () => {
            try {
                const data = await getOrganizationProjectsById(taskDetails.organizationId!);
                setProjects(data.data.projects);
                const project = data.data.projects.find((p: Project) => p.id === taskDetails.projectId);

                if (project) {
                    const status = project.statuses?.find((s: { statusId: number; }) => s.statusId === taskDetails.statusId) || null;
                    setTaskStatus(status);
                }

                const type = taskTypes.find((t) => t.id === taskDetails.typeId) || null;
                setTaskType(type);
            } catch (error) {
                console.error('Error fetching projects:', error);
            }
        };

        fetchProjects();
    }, [taskDetails]);

    useEffect(() => {
        if (taskDetails?.organizationId) {
            localStorage.setItem('organizationId', taskDetails?.organizationId);
        }
    }, [taskDetails?.organizationId]);

    useEffect(() => {
        if (!taskDetails) return;

        const fetchOrganizationAccounts = async () => {
            setAccountsLoading(true);
            try {
                const data = await getOrganizationAccounts(taskDetails.organizationId!);
                const accountDetails = await getAllAccountDetails(data.data.accounts);
                setAccounts(accountDetails.data);

                const reporter = accountDetails.data.find((a) => a.id !== undefined && a.id === taskDetails.reporterId) || null;
                const assignee = accountDetails.data.find((a) => a.id !== undefined && a.id === taskDetails.assigneeId) || null;

                setReporterId(reporter);
                setAssigneeId(assignee);
            } catch (error) {
                console.error('Error fetching accounts:', error);
            } finally {
                setAccountsLoading(false);
            }
        };

        fetchOrganizationAccounts();
    }, [taskDetails]);


    useEffect(() => {
        const fetchTasks = async () => {
            try {
                const data = await getAllTasksByOrganization(taskDetails?.organizationId!);
                setTasks(data.data);
            } catch (error) {
                console.error('Error fetching tasks:', error);
            }
        };
        fetchTasks();
    }, []);

    useEffect(() => {
        if (taskDetails) {
            setTaskTitle(taskDetails.title || '');
            setTaskDescription(taskDetails.description || '');
            setOriginalTitle(taskDetails.title || '');
            setOriginalDescription(taskDetails.description || '');
            setChildTasks(taskDetails.childIssues || null)
            setSelectedProjectId(taskDetails.projectId || null);
            setSelectedParentTaskId(taskDetails.parentId || null);
            setTaskType(taskTypes.find(type => type.id === taskDetails.typeId) || null);
        }
    }, [taskDetails]);

    const handleStartEditingTitle = () => {
        setOriginalTitle(taskTitle);
        setIsEditingTitle(true);
    };

    const handleCancelEditingTitle = () => {
        setTaskTitle(originalTitle);
        setIsEditingTitle(false);
    };

    const handleSaveEditingTitle = () => {
        if (taskDetails) {
            taskDetails.title = taskTitle;
        }
        setIsEditingTitle(false);
    };

    const handleTaskTitleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setTaskTitle(event.target.value);
        if (event.target.value && taskDetails) {
            taskDetails!.title = event.target.value
            handleEditTask();
        }
    };

    const handleTaskDescriptionChange = (description: string) => {
        setTaskDescription(description);
    };

    const handleStartEditingDescription = () => {
        setOriginalDescription(taskDescription);
        setIsEditingDescription(true);
    };

    const handleCancelEditingDescription = () => {
        setTaskDescription(originalDescription);
        editor?.commands.setContent(originalDescription);
        setIsEditingDescription(false);
    };

    const handleSaveEditingDescription = () => {
        setIsEditingDescription(false);
        handleEditTask();
    };

    const handleProjectChange = (projectId: string | null) => {
        setSelectedProjectId(projectId);
        if (projectId && taskDetails) {
            taskDetails!.projectId = projectId
            handleEditTask();
        }
    };

    const handleParentTaskChange = (taskId: string | null) => {
        setSelectedParentTaskId(taskId);
        if (taskId && taskDetails) {
            taskDetails!.parentId = taskId
            handleEditTask();
        }
    };

    const handleTaskTypeChange = (selectedType: TaskType | null) => {
        setTaskType(selectedType);
        if (selectedType && taskDetails) {
            taskDetails!.typeId = selectedType.id
            handleEditTask();
        }
    };

    const handleStatusChange = (status: Status | null) => {
        setTaskStatus(status);
        if (status && taskDetails) {
            taskDetails.statusId = status.statusId;
            taskDetails.statusName = status.statusName;
            handleEditTask();
        }
    };

    const handleReporterChange = (value: AccountDetails | null) => {
        setReporterId(value);
        if (value && taskDetails && value.id !== undefined) {
            taskDetails.reporterId = value.id;
            handleEditTask();
        }
    }

    const handleAssigneeChange = (value: AccountDetails | null) => {
        setAssigneeId(value);
        if (value && taskDetails && value.id !== undefined) {
            taskDetails.assigneeId = value.id;
            handleEditTask();
        }
    }

    const handleEditTask = async () => {
        const taskData = {
            id: taskDetails!.id,
            title: taskDetails?.title || taskTitle || null,
            description: taskDetails?.description || taskDescription || null,
            type: taskDetails?.typeId || taskType?.id || null,
            reporterId: taskDetails?.reporterId || reporterId?.id || null,
            assigneeId: taskDetails?.assigneeId || assigneeId?.id || null,
            projectId: taskDetails?.projectId || selectedProjectId || null,
            parentId: taskDetails?.parentId || selectedParentTaskId || null,
            statusId: taskDetails?.statusId || taskStatus?.statusId || null,
            statusName: taskDetails?.statusName || taskStatus?.statusName || null,
        };

        try {
            const response = await editTask(taskDetails!.id, taskData);
        } catch (error) {
            console.error('Error creating task:', error);
        }
    }

    if (id === undefined) {
        return <NotFoundPage />;
    }


    return (
        <Container fluid>
            {loading ? (
                <LoaderMain />
            ) : error ? (
                <Text c="red">{error}</Text>
            ) : taskDetails && taskType && taskStatus && reporterId ? (
                <Container fluid>
                    <Flex
                        mih={50}
                        gap="sm"
                        justify="center"
                        align="flex-start"
                        direction="row"
                        wrap="wrap"

                    >
                        <Flex
                            justify="center"
                            align="center"
                            direction="column"
                            wrap="wrap"
                            w="70%"
                            miw={350}
                        >
                            <Fieldset legend="Summary" mb="md" style={{ width: '100%' }}>
                                {!isEditingTitle ? (
                                    <div onClick={handleStartEditingTitle} style={{ cursor: 'pointer', padding: '6px 8px' }}>
                                        <Text size="sm" fw={500}>
                                            {taskTitle || <i style={{ color: '#aaa' }}>Click to add title</i>}
                                        </Text>
                                    </div>
                                ) : (
                                    <>
                                        <Input
                                            value={taskTitle}
                                            onChange={handleTaskTitleChange}
                                            placeholder="Enter task title"
                                            style={{ width: '100%' }}
                                        />
                                        <Flex mt="sm" gap="sm" justify="flex-end">
                                            <Button size="xs" color="gray" onClick={handleCancelEditingTitle}>
                                                Cancel
                                            </Button>
                                            <Button size="xs" color="blue" onClick={handleSaveEditingTitle}>
                                                Save
                                            </Button>
                                        </Flex>
                                    </>
                                )}
                            </Fieldset>
                            <Fieldset legend="Description" mb="md" style={{ width: '100%', minHeight: '350px' }}>
                                {!isEditingDescription ? (
                                    <div
                                        style={{ minHeight: '350px', cursor: 'pointer' }}
                                        onClick={handleStartEditingDescription}
                                        dangerouslySetInnerHTML={{ __html: taskDescription || '<i>Click to add description</i>' }}
                                    />
                                ) : (
                                    <>
                                        <TicketDesciption editor={editor} content={taskDescription} onChange={handleTaskDescriptionChange} />
                                        <Flex mt="sm" gap="sm" justify="flex-end">
                                            <Button size="xs" color="gray" onClick={handleCancelEditingDescription}>
                                                Cancel
                                            </Button>
                                            <Button size="xs" color="blue" onClick={handleSaveEditingDescription}>
                                                Save
                                            </Button>
                                        </Flex>
                                    </>
                                )}
                            </Fieldset>
                        </Flex>
                        <Flex
                            justify="center"
                            align="center"
                            direction="column"
                            wrap="wrap"
                            gap="sm"
                            w="20%"
                            miw={250}
                        >
                            <Fieldset legend="Status" style={{ width: '100%' }}>
                                <ProjectTaskStatusesDdData
                                    selectedProjectId={selectedProjectId}
                                    taskStatus={taskStatus}
                                    projects={projects}
                                    onStatusChange={handleStatusChange}
                                />
                            </Fieldset>
                            <Fieldset legend="Task Type" style={{ width: '100%' }}>
                                <TaskTypeDropdown
                                    taskType={taskType}
                                    taskTypes={taskTypes}
                                    onTaskTypeChange={handleTaskTypeChange}
                                />
                            </Fieldset>

                            <Fieldset legend="Select Project" style={{ width: '100%' }}>
                                <ProjectDropdownData
                                    selectedProjectId={selectedProjectId}
                                    onProjectChange={handleProjectChange}
                                    organizationId={taskDetails.organizationId!}
                                />
                            </Fieldset>

                            <Fieldset legend="Parent Task" style={{ width: '100%' }}>
                                <TaskDropdown
                                    selectedTaskId={selectedParentTaskId}
                                    tasks={tasks}
                                    onTaskChange={handleParentTaskChange}
                                />
                            </Fieldset>
                            <Fieldset legend="Reporter" style={{ width: '100%' }}>
                                <AccountDropdown
                                    selectedAccount={reporterId}
                                    accounts={accounts}
                                    placeholder={reporterId?.firstName || "Select reporter"}
                                    onAccountChange={handleReporterChange}
                                />
                            </Fieldset>
                            <Fieldset legend="Assignee" style={{ width: '100%' }}>
                                <AccountDropdown
                                    selectedAccount={assigneeId}
                                    accounts={accounts}
                                    placeholder="Select Assignee"
                                    onAccountChange={handleAssigneeChange}
                                />
                            </Fieldset>
                        </Flex>

                        <Container fluid w={"93%"} mt="md">
                            {taskDetails.childIssues ? (
                                <Paper withBorder shadow="xs" radius="md" p="md">
                                    <TableTickets tasks={taskDetails.childIssues ?? []} accounts={accounts} onTaskClick={openTaskDialog} />
                                </Paper>
                            ) : (null)}
                            {selectedTask && (
                                <TaskDialog
                                    task={selectedTask}
                                    opened={dialogOpen}
                                    onClose={closeTaskDialog}
                                    organizationId={taskDetails.organizationId!}
                                />
                            )}
                        </Container>

                        <Container fluid w={"93%"} mt="md">
                            {taskDetails && <TaskAdditionalInfo taskId={taskDetails.id} />}
                        </Container>
                    </Flex>
                </Container>
            ) : null}
        </Container>
    );
}

export default TaskPage;