import { useEffect, useState } from 'react';
import { useOrganizationApi, useProjectApi, useTaskApi } from '../../api/taskManagerApi';
import type { AccountDetails, OrganizationDetails, Project, Status, Task, TaskStatus, TaskType } from '../Types';
import { Card, Text, Group, Badge, Dialog, Modal, Flex, Fieldset, Select, Input, Button, ScrollArea } from '@mantine/core';
import { LoaderMain } from '../LoaderMain';
import { useEditor } from '@tiptap/react';
import Highlight from '@tiptap/extension-highlight';
import StarterKit from '@tiptap/starter-kit';
import Underline from '@tiptap/extension-underline';
import TextAlign from '@tiptap/extension-text-align';
import Superscript from '@tiptap/extension-superscript';
import SubScript from '@tiptap/extension-subscript';
import Link from '@tiptap/extension-link';

import { TicketDesciption } from '../TicketView/TicketDesciption';
import { TaskTypesBadge, taskTypesConst } from './TaskTypeBadge';
import { useIdentityServerApi } from '../../api/IdentityServerApi';
import { ProjectDropdownData } from '../DropdownData/ProjectDropdownData';
import { ProjectTaskStatusesDdData } from '../DropdownData/ProjectTaskStatusesDdData';
import { TaskTypeDropdown } from '../DropdownData/TaskTypeDropdown';
import { TaskDropdown } from '../DropdownData/TaskDropdown';
import { AccountDropdown } from '../DropdownData/AccountDropdown';
import { TaskAdditionalInfo } from './TaskAdditionalInfo';
import { TableTickets } from './TableTickets';

type TaskDialogProps = {
    organizationId: string;
    task: Task;
    opened: boolean;
    onClose: () => void;
};

export const TaskDialog = ({ organizationId, task, opened, onClose }: TaskDialogProps) => {
    const { getTaskById, getAllTasksByOrganization } = useTaskApi();
    const { getOrganizationProjectsById } = useOrganizationApi();
    const { editTask } = useTaskApi();
    const { getAllAccountDetails } = useIdentityServerApi();
    const [organization, setOrganization] = useState<OrganizationDetails>();
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
    const [tasks, setTasks] = useState<Task[] | null>(null);
    const [accountsLoading, setAccountsLoading] = useState(false);
    const [isEditingTitle, setIsEditingTitle] = useState(false);
    const [originalTitle, setOriginalTitle] = useState(task.description || '');
    const [isEditingDescription, setIsEditingDescription] = useState(false);
    const [originalDescription, setOriginalDescription] = useState(task.title || '');
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
    const [dialogOpen, setDialogOpen] = useState(false);
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

    useEffect(() => {
        if (!task.id) return;

        const fetchTaskDetails = async () => {
            setLoading(true);
            try {
                const data = await getTaskById(task.id);
                setTaskDetails(data.data);
            } catch {
                setError('Failed to load task details');
            } finally {
                setLoading(false);
            }
        };

        fetchTaskDetails();
    }, [task.id]);


    useEffect(() => {
        if (!taskDetails) return;

        const fetchProjects = async () => {
            try {
                const data = await getOrganizationProjectsById(organizationId);
                setOrganization(data.data);
                setProjects(data.data.projects);
                const project = data.data.projects.find((p: Project) => p.id !== undefined && p.id === taskDetails.projectId);

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

    const fetchOrganizationAccounts = async () => {
        if (!taskDetails) return;
        if (!organization) return;
        setAccountsLoading(true);
        try {
            const accountDetails = await getAllAccountDetails(organization?.accounts);
            setAccounts(accountDetails.data);
            console.log(accountDetails.data)

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

    useEffect(() => {
        fetchOrganizationAccounts();
    }, [taskDetails, organization]);


    useEffect(() => {
        const fetchTasks = async () => {
            try {
                const data = await getAllTasksByOrganization(organizationId);
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
        if (value && taskDetails && value.id) {
            taskDetails.reporterId = value.id ?? '';
            handleEditTask();
        }
    }

    const handleAssigneeChange = (value: AccountDetails | null) => {
        setAssigneeId(value);
        if (value && taskDetails) {
            taskDetails.assigneeId = value.id ?? '';
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

    return (
        <Modal
            opened={opened}
            onClose={onClose}
            title={task.id}
            size="xxl"
            withCloseButton
            transitionProps={{ transition: 'fade', duration: 200 }}
        >
            {loading ? (
                <LoaderMain />
            ) : error ? (
                <Text c="red">{error}</Text>
            ) : taskDetails ? (
                <Card
                    shadow="sm"
                    padding="lg"
                    radius="md"
                    withBorder
                    style={{ maxWidth: '1200px' }}
                >
                    <Flex justify="space-between" align="center" mb="md">
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
                    </Flex>
                    <Flex justify="space-between" align="center" mb="md">
                        <Fieldset legend="Description" mb="md" style={{ width: '100%', minHeight: '300px' }}>
                            {!isEditingDescription ? (
                                <div
                                    style={{ minHeight: '150px', cursor: 'pointer' }}
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
                    <Flex justify="space-between" align="center" mb="md">
                        <Fieldset legend="Status" style={{ width: '50%' }}>
                            <ProjectTaskStatusesDdData
                                selectedProjectId={selectedProjectId}
                                taskStatus={taskStatus}
                                projects={projects}
                                onStatusChange={handleStatusChange}
                            />
                        </Fieldset>
                        <Fieldset legend="Task Type" style={{ width: '50%' }}>
                            <TaskTypeDropdown
                                taskType={taskType}
                                taskTypes={taskTypes}
                                onTaskTypeChange={handleTaskTypeChange}
                            />
                        </Fieldset>
                    </Flex>
                    <Flex justify="space-between" align="center" mb="md">
                        <Fieldset legend="Select Project" style={{ width: '100%' }}>
                            <ProjectDropdownData
                                selectedProjectId={selectedProjectId}
                                onProjectChange={handleProjectChange}
                                organizationId={organizationId}
                            />
                        </Fieldset>
                    </Flex>
                    <Flex justify="space-between" align="center" mb="md">
                        <Fieldset legend="Parent Task" style={{ width: '100%' }}>
                            <TaskDropdown
                                selectedTaskId={selectedParentTaskId}
                                tasks={tasks}
                                onTaskChange={handleParentTaskChange}
                            />
                        </Fieldset>
                    </Flex>
                    <Flex justify="space-between" align="center" mb="md">
                        <Fieldset legend="Reporter" style={{ width: '50%' }}>
                            <AccountDropdown
                                selectedAccount={reporterId}
                                accounts={accounts}
                                placeholder="Select Reporter"
                                onAccountChange={handleReporterChange}
                            />
                        </Fieldset>
                        <Fieldset legend="Assignee" style={{ width: '50%' }}>
                            <AccountDropdown
                                selectedAccount={assigneeId}
                                accounts={accounts}
                                placeholder="Select Assignee"
                                onAccountChange={handleAssigneeChange}
                            />
                        </Fieldset>
                    </Flex>
                    <Flex justify="space-between" align="center" mb="md">
                        <Fieldset legend="Child issues" style={{ width: '100%' }}>
                            {taskDetails.childIssues ? (
                                <TableTickets tasks={taskDetails.childIssues ?? []} accounts={accounts} onTaskClick={openTaskDialog} />
                            ) : (null)}
                            {selectedTask && (
                                <TaskDialog
                                    task={selectedTask}
                                    opened={dialogOpen}
                                    onClose={closeTaskDialog}
                                    organizationId={organizationId}
                                />
                            )}
                        </Fieldset>
                    </Flex>
                    <Flex justify="space-between" align="center" mb="md">
                        <TaskAdditionalInfo taskId={taskDetails.id} />
                    </Flex>
                </Card>
            ) : (
                <Text c="dimmed">No task selected</Text>
            )}
        </Modal>
    );
};
