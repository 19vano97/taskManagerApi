import { Button, Card, Container, Divider, Fieldset, Flex, Input, Paper, Text } from "@mantine/core";
import { Navigate, useParams, useNavigate } from "react-router-dom"
import { DatePicker, DatePickerInput } from '@mantine/dates';
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
import { TimeOnlyInput, type TimeOnly } from "../hooks/useTimeOnly";
import { Divide } from "lucide-react";

const TaskPage = () => {
    const params = useParams<{ id?: string }>();
    const id = params?.id;
    const navigate = useNavigate();
    const { getTaskById } = useTaskApi();
    const { getOrganizationProjectsById } = useOrganizationApi();
    const { editTask } = useTaskApi();
    const [taskDetails, setTaskDetails] = useState<Task | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [taskTitle, setTaskTitle] = useState<string>('');
    const [taskDescription, setTaskDescription] = useState<string>('');
    const [selectedProjectId, setSelectedProjectId] = useState<string | null>(null);
    const [selectedParentTaskId, setSelectedParentTaskId] = useState<Task | null>(null);
    const [taskType, setTaskType] = useState<TaskType | null>(null);
    const [projects, setProjects] = useState<Project[] | null>(null);
    const [accounts, setAccounts] = useState<AccountDetails[]>([]);
    const [reporterId, setReporterId] = useState<AccountDetails | null>(null);
    const [assigneeId, setAssigneeId] = useState<AccountDetails | null>(null);
    const [taskStatus, setTaskStatus] = useState<Status | null>(null);
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
    const [dialogOpen, setDialogOpen] = useState(false);
    const [childTasks, setChildTasks] = useState<Task[] | null>(null);
    const [accountsLoading, setAccountsLoading] = useState(false);
    const [isEditingTitle, setIsEditingTitle] = useState(false);
    const [originalTitle, setOriginalTitle] = useState('');
    const [isEditingDescription, setIsEditingDescription] = useState(false);
    const [originalDescription, setOriginalDescription] = useState('');
    const [startDate, setStartDate] = useState<Date | null>(null);
    const [dueDate, setDueDate] = useState<Date | null>(null);
    const [estimatedTime, setEstimatedTime] = useState<TimeOnly | null>(null);
    const [spentTime, setSpentTime] = useState<TimeOnly | null>(null);
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
                setProjects(data.data.projects || []);
                const project = data.data.projects?.find((p: Project) => p.id === taskDetails.projectId);

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
        if (taskDetails) {
            setTaskTitle(taskDetails.title || '');
            setTaskDescription(taskDetails.description || '');
            setOriginalTitle(taskDetails.title || '');
            setOriginalDescription(taskDetails.description || '');
            setChildTasks(taskDetails.childIssues || null)
            setSelectedProjectId(taskDetails.projectId || null);
            setSelectedParentTaskId(taskDetails.parentTicket || null);
            setReporterId(taskDetails.reporter || null);
            setAssigneeId(taskDetails.assignee || null);
            setStartDate(taskDetails.startDate ? new Date(taskDetails.startDate) : null);
            setDueDate(taskDetails.dueDate ? new Date(taskDetails.dueDate) : null);
            setEstimatedTime(taskDetails.estimate ? taskDetails.estimate : null);
            setSpentTime(taskDetails.spentTime ? taskDetails.spentTime : null);
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
        console.log('Saving description:', taskDescription);
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
        setSelectedParentTaskId({id: taskId} as Task);
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

    const handleStartDateChange = (date: string | null) => {
        setStartDate(new Date(date!));
        if (date && taskDetails) {
            taskDetails.startDate = new Date(date!);
            handleEditTask();
        }
    }

    const handleDueDateChange = (date: string | null) => {
        setDueDate(new Date(date!));
        if (date && taskDetails) {
            taskDetails.dueDate = new Date(date!);
            handleEditTask();
        }
    }

    const handleEstimatedTimeChange = (time: TimeOnly | null) => {
        setEstimatedTime(time);
        if (time && taskDetails) {
            taskDetails.estimate = time;
            handleEditTask();
        }
    }

    const handleSpentTimeChange = (time: TimeOnly | null) => {
        setSpentTime(time);
        if (time && taskDetails) {
            taskDetails.spentTime = time;
            console.log('Spent time updated:', taskDetails.spentTime);
            handleEditTask();
        }
    }

    const handleEditTask = async () => {
        const taskData = {
            id: taskDetails!.id,
            title: taskTitle|| taskDetails?.title || null,
            description: taskDescription || taskDetails?.description || null,
            type: taskDetails?.typeId || taskType?.id || null,
            reporterId: taskDetails?.reporterId || reporterId?.id || null,
            assigneeId: taskDetails?.assigneeId || assigneeId?.id || null,
            projectId: taskDetails?.projectId || selectedProjectId || null,
            parentId: taskDetails?.parentId || selectedParentTaskId || null,
            statusId: taskDetails?.statusId || taskStatus?.statusId || null,
            statusName: taskDetails?.statusName || taskStatus?.statusName || null,
            startDate: taskDetails?.startDate ? taskDetails?.startDate: null,
            dueDate: taskDetails?.dueDate ? taskDetails?.dueDate : null,
            estimate: taskDetails?.estimate ?? null,
            spentTime: taskDetails?.spentTime ?? null,
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
            ) : taskDetails && taskType && taskStatus ? (
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

                            {taskDetails.childIssues ? (
                                <Fieldset legend="Child Tasks" mb="md" style={{ width: '100%' }}>
                                    <Paper withBorder shadow="xs" radius="md" p="md">
                                        <TableTickets tasks={taskDetails.childIssues ?? []} accounts={accounts} onTaskClick={openTaskDialog} />
                                    </Paper></Fieldset>
                            ) : (null)}
                            {selectedTask && (
                                <TaskDialog
                                    task={selectedTask}
                                    opened={dialogOpen}
                                    onClose={closeTaskDialog}
                                    organizationId={taskDetails.organizationId!}
                                />
                            )}

                            <Fieldset legend="Additional Info" mb="md" style={{ width: '100%' }}>
                                {taskDetails && <TaskAdditionalInfo taskId={taskDetails.id} organizationId={taskDetails.organizationId!} />}
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
                                    organizationId={taskDetails.organizationId!}
                                    onTaskChange={handleParentTaskChange}
                                />
                            </Fieldset>
                            <Fieldset legend="Reporter" style={{ width: '100%' }}>
                                <AccountDropdown
                                    selectedAccount={reporterId}
                                    organizationId={taskDetails.organizationId!}
                                    placeholder={reporterId?.firstName + " " + reporterId?.lastName || "Select reporter"}
                                    onAccountChange={handleReporterChange}
                                />
                            </Fieldset>
                            <Fieldset legend="Assignee" style={{ width: '100%' }}>
                                <AccountDropdown
                                    selectedAccount={assigneeId}
                                    organizationId={taskDetails.organizationId!}
                                    placeholder={assigneeId?.firstName + " " + reporterId?.lastName || "Select reporter"}
                                    onAccountChange={handleAssigneeChange}
                                />
                            </Fieldset>
                            <Fieldset legend="Time" w={'100%'}>

                                <TimeOnlyInput
                                    label="Estimated Time (HH:mm)"
                                    value={taskDetails.estimate ?? ''}
                                    onChange={handleEstimatedTimeChange}
                                />

                                <TimeOnlyInput
                                    label="Spent Time (HH:mm)"
                                    value={taskDetails.spentTime ?? ''}
                                    onChange={handleSpentTimeChange}
                                />

                                <Divider my="sm" />

                                <DatePickerInput
                                    valueFormat="YYYY-MM-DD"
                                    label="Start date"
                                    placeholder="Pick date"
                                    value={taskDetails?.startDate ? taskDetails?.startDate : null}
                                    onChange={handleStartDateChange}
                                />

                                <Divider my="sm" />
                                
                                <DatePickerInput
                                    valueFormat="YYYY-MM-DD"
                                    label="Due Date"
                                    placeholder="Pick date"
                                    value={taskDetails?.dueDate ? taskDetails?.dueDate : null}
                                    onChange={handleDueDateChange}
                                />
                                
                            </Fieldset>
                        </Flex>
                    </Flex>
                </Container>
            ) : null}
        </Container>
    );
}

export default TaskPage;