import { Button, Fieldset, Flex, Input, Modal, Select, Title } from "@mantine/core";
import { useEditor } from '@tiptap/react';
import { Link } from '@mantine/tiptap';
import Highlight from '@tiptap/extension-highlight';
import StarterKit from '@tiptap/starter-kit';
import Underline from '@tiptap/extension-underline';
import TextAlign from '@tiptap/extension-text-align';
import Superscript from '@tiptap/extension-superscript';
import SubScript from '@tiptap/extension-subscript';
import { TicketDesciption } from "./TicketDesciption";
import { useOrganizationApi, useProjectApi, useTaskApi } from "../../api/taskManagerApi";
import type { AccountDetails, Project, Task } from "../Types";
import { useEffect, useState } from "react";
import { useSafeAuth } from "../../hooks/useSafeAuth";
import { AccountDropdown } from "../DropdownData/AccountDropdown";
import { useIdentityServerApi } from "../../api/IdentityServerApi";

type CreateTicketProps = {
    opened: boolean;
    onClose: () => void;
}

export const CreateTicket = ({opened, onClose}: CreateTicketProps) => {
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
    const { getAllProjects } = useProjectApi();
    const { getAllTasks, createTask } = useTaskApi();
    const { getOrganizationAccounts } = useOrganizationApi();
    const { getAllAccountDetails } = useIdentityServerApi();
    const [projects, setProjects] = useState<Project[] | null>(null);
    const [tasks, setTasks] = useState<Task[] | null>(null);
    const [titleTask, setTitleTask] = useState<string>('');
    const [selectedProjectId, setSelectedProjectId] = useState<string | null>(null);
    const [selectedParentTaskId, setSelectedParentTaskId] = useState<string | null>(null);
    const [accounts, setAccounts ] = useState<AccountDetails[]>([]);
    const [reporterId, setReporterId] = useState<AccountDetails | null>(null);
    const [assigneeId, setAssigneeId] = useState<AccountDetails | null>(null);
    const [accountsLoading, setAccountsLoading] = useState(false);
    const auth = useSafeAuth();
    

    useEffect(() => {
        const fetchProjects = async () => {
            try {
                const data = await getAllProjects();
                setProjects(data);
            } catch (error) {
                console.error('Error fetching projects:', error);
            }
        };
        fetchProjects();
    }, []);

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
    }
    , []);

    useEffect(() => {
            const fetchOrganizationAccounts = async () => {
                setAccountsLoading(true); // Start loading
                try {
                    const data = await getOrganizationAccounts();
                    const dataAccountDetails = await getAllAccountDetails(data.accounts);
                    setAccounts(dataAccountDetails);
                } catch (error) {
                    console.error('Error fetching accounts:', error);
                } finally {
                    setAccountsLoading(false); // End loading
                }
            };
            fetchOrganizationAccounts();
        }, []);

    const handleProjectChange = (projectId: string | null) => {
        setSelectedProjectId(projectId);
    };

    const handleTaskChange = (taskId: string | null) => {
        setSelectedParentTaskId(taskId);
    };

    const handleReporterChange = (value: AccountDetails | null) => {
        setReporterId(value);
    }
    
    const handleAssigneeChange = (value: AccountDetails | null) => {
        setAssigneeId(value);
    }

    const handleCreateTask = async () => {
        if (!selectedProjectId) {
            console.error('No project selected');
            return;
        }
        const taskData = {
            title: titleTask,
            description: editor ? editor.getHTML() : '',
            type: null, // Set the type as needed
            reporterId: auth.user?.profile.sub || '', 
            assigneeId: auth.user?.profile.sub || '', 
            projectId: selectedProjectId,
            parentId: selectedParentTaskId || null,
        }

        try {
            const responce = await createTask(taskData);
            console.log('Task created successfully:', responce);
        } catch (error) {
            console.error('Error creating task:', error);
        }
        onClose();
    }

    return (
        <Modal
            opened={opened}
            onClose={onClose}
            title="Create New Ticket"
            size="xxl"
            withCloseButton
            transitionProps={{ transition: 'fade', duration: 200 }}
        >
            {/* <Modal.Header>
                <Title order={3} style={{ width: '100%' }}>
                    Create New Ticket
                </Title>
            </Modal.Header> */}

            <Modal.Body>
                <Flex direction="column" gap="md" style={{ width: '100%' }}>
                    <div>
                        <Fieldset legend="Ticket Title" style={{ width: '100%' }}>
                            <Input
                                placeholder="Enter ticket title"
                                style={{ width: '100%' }}
                                onChange={(event) => { setTitleTask(event.currentTarget.value) }}
                            />
                        </Fieldset>
                    </div>
                    <div>
                        <Fieldset legend="Ticket Description" style={{ width: '100%' }}>
                            <Flex style={{ width: '100%', height: '300px' }}>
                                <TicketDesciption
                                    editor={editor}
                                    content=""
                                    onChange={() => {}}
                                />
                            </Flex>
                        </Fieldset>
                    </div>
                    <div>
                        <Fieldset legend="Select Project" style={{ width: '100%' }}>
                            <Select
                            placeholder="Select Project"
                            data={
                            projects?.map((project) => ({
                                value: project.id,
                                label: project.title,
                            })) || []
                            }
                            value={selectedProjectId}
                            onChange={handleProjectChange}
                            searchable
                            style={{ width: '100%' }}
                            />
                        </Fieldset>
                    </div>
                    <div>
                        <Fieldset legend="Parent Task" style={{ width: '100%' }}>
                            <Select
                                placeholder="Select parent task"
                                data={
                                tasks?.map((task) => ({
                                    value: task.id,
                                    label: task.title,
                                })) || []
                                }
                                value={selectedParentTaskId}
                                onChange={handleTaskChange}
                                searchable
                                style={{ width: '100%' }}
                            />
                        </Fieldset>
                    </div>
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
                </Flex>
            </Modal.Body>
            <Flex justify="flex-end" gap="md" mt="md">
                <Button variant="outline" onClick={onClose}>Cancel</Button>
                <Button onClick={handleCreateTask} color="blue">Create Task</Button>
            </Flex>
        </Modal>
    );
}