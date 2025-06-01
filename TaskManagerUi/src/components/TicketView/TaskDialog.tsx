import { useEffect, useState } from 'react';
import { useOrganizationApi, useProjectApi, useTaskApi } from '../../api/taskManagerApi';
import type { AccountDetails, Project, Status, Task, TaskStatus, TaskType } from '../Types';
import { Card, Text, Group, Badge, Dialog, Modal, Flex, Fieldset, Select, Input, Button } from '@mantine/core';
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

type TaskDialogProps = {
    task: Task;
    opened: boolean;
    onClose: () => void;
};

export const TaskDialog = ({ task, opened, onClose }: TaskDialogProps) => {
    const { getTaskById, getAllTasks } = useTaskApi();
    const { getAllProjects } = useProjectApi();
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
    const [accounts, setAccounts ] = useState<AccountDetails[]>([]);
    const [reporterId, setReporterId] = useState<AccountDetails | null>(null);
    const [assigneeId, setAssigneeId] = useState<AccountDetails | null>(null);
    const [taskStatus, setTaskStatus] = useState<Status | null>(null);
    const [tasks, setTasks] = useState<Task[] | null>(null);
    const [accountsLoading, setAccountsLoading] = useState(false);
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

    useEffect(() => {
        if (!task) return;

        if (taskDetails && taskDetails.id === task.id) {
            return;
        }

        setLoading(true);
        setError(null);
        setTaskDetails(null);

        const fetchTaskDetails = async () => {
            try {
                const data = await getTaskById(task.id);
                setTaskDetails(data);
                setTaskType(
                    taskTypes.find((type) => type.id === task.type) || null
                );
                setSelectedProjectId(data.projectId || null);
                setSelectedParentTaskId(data.parentId || null);
                setTaskTitle(data.title || '');
                setTaskDescription(data.description || '');
            } catch (err) {
                setError('Failed to load task details');
            } finally {
                setLoading(false);
            }
        };

        fetchTaskDetails();
    }, [task?.id, getTaskById, taskDetails]);

    useEffect(() => {
        const fetchProjects = async () => {
            try {
                const data = await getAllProjects();
                setProjects(data);
                setTaskStatus(
                    data.find((project: Project) => project.id === task.projectId)?.statuses
                        .find((status: Status) => status.statusId === task.statusId) || null
                );
                setTaskType(
                    taskTypes.find((type) => type.id === task.type) || null
                );
                setSelectedProjectId(task.projectId || null);
            } catch (error) {
                console.error('Error fetching projects:', error);
            }
        };
        fetchProjects();
    }, []);

    useEffect(() => {
        const fetchOrganizationAccounts = async () => {
            setAccountsLoading(true); // Start loading
            try {
                const data = await getOrganizationAccounts();
                const dataAccountDetails = await getAllAccountDetails(data.accounts);
                setAccounts(dataAccountDetails);
                setReporterId(dataAccountDetails.find((account: AccountDetails) => account.id === task.reporterId) || null);
                setAssigneeId(dataAccountDetails.find((account: AccountDetails) => account.id === task.assigneeId) || null);
            } catch (error) {
                console.error('Error fetching accounts:', error);
            } finally {
                setAccountsLoading(false); // End loading
            }
        };
        fetchOrganizationAccounts();
    }, [task?.reporterId, task?.assigneeId]);

    useEffect(() => {
        const fetchTasks = async () => {
            try {
                const data = await getAllTasks();
                setTasks(data);
            } catch (error) {
                console.error('Error fetching tasks:', error);
            }
        };
        fetchTasks();
    }, []);

    const handleTaskTitleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setTaskTitle(event.target.value);
        taskDetails!.title = taskTitle;
    };

    const handleTaskDescriptionChange = (description: string) => {
        setTaskDescription(description);
        taskDetails!.description = taskDescription;
    };

    const handleProjectChange = (projectId: string | null) => {
        setSelectedProjectId(projectId);
        taskDetails!.projectId = selectedProjectId!;
    };

    const handleParentTaskChange = (taskId: string | null) => {
        setSelectedParentTaskId(taskId);
        taskDetails!.parentId = taskId!;
    };

    const handleTaskTypeChange = (selectedType: TaskType | null) => {
        setTaskType(selectedType);
        if (taskDetails && selectedType) {
            taskDetails.type = selectedType.id;
            taskDetails.typeName = selectedType.name || '';
        }
    };

    const handleStatusChange = (status: Status | null) => {
        setTaskStatus(status);
        if (taskDetails && status) {
            taskDetails.statusId = status.statusId;
            taskDetails.statusName = status.statusName;
        }
    };

    const handleReporterChange = (value: AccountDetails | null) => {
        setReporterId(value);
        taskDetails!.reporterId = value?.id!;
    }

    const handleAssigneeChange = (value: AccountDetails | null) => {
        setAssigneeId(value);
        taskDetails!.assigneeId = assigneeId!.id;
    }

    const handleEditTask = async () => {
        const taskData = {
            id: taskDetails!.id,
            title: taskDetails!.title,
            description: editor ? editor.getHTML() : '',
            type: taskDetails!.type,
            reporterId: taskDetails!.reporterId,
            assigneeId: taskDetails!.assigneeId,
            projectId: taskDetails?.projectId || null,
            parentId: taskDetails?.parentId || null,
            statusId: taskDetails!.statusId,
            statusName: taskDetails!.statusName,
        };

        try {
            const response = await editTask(taskDetails!.id, taskData);
        } catch (error) {
            console.error('Error creating task:', error);
        }
        onClose();
    }

    return (
        <Modal
            opened={opened}
            onClose={onClose}
            title={task.id}
            size="xxl"
            withCloseButton
            style={{ maxHeight: '600px' }}
            transitionProps={{ transition: 'fade', duration: 200 }}
        >
            {loading ? (
                <LoaderMain />
            ) : error ? (
                <Text c="red">{error}</Text>
            ) : taskDetails ? (
                <Card shadow="sm" padding="lg" radius="md" withBorder>
                    <Flex justify="space-between" align="center" mb="md">
                        <Fieldset legend="Summary" mb="md" style={{ width: '100%' }}>
                            <Input
                                placeholder="Task title"
                                value={taskDetails.title}
                                onChange={handleTaskTitleChange}
                                style={{ width: '100%' }}
                            />
                        </Fieldset>
                    </Flex>
                    <Flex justify={'space-between'} align="center" mb="md">
                        <Fieldset legend="Description" mb="md" style={{ width: '100%', minHeight: '300px' }}>
                            <TicketDesciption editor={editor} content={taskDetails.description} onChange={handleTaskDescriptionChange}/>
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
                    <TaskAdditionalInfo taskId={taskDetails.id} />
                    <Flex justify="space-between" align="center" mt="md">
                        <Button variant="outline" onClick={onClose}>
                            Close
                        </Button>
                        <Button color="blue" onClick={handleEditTask}>
                            Save Changes
                        </Button>
                    </Flex>
                </Card>
            ) : (
                <Text c="dimmed">No task selected</Text>
            )}
        </Modal>
    );
};
