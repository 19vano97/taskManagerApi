import { useEffect, useState } from "react"
import { useOrganizationApi, useProjectApi } from "../../api/taskManagerApi"
import type { AccountDetails, Project, Task } from "../../components/Types"
import { Button, Container, Flex, Title } from "@mantine/core"
import { TaskDialog } from "../../components/TicketView/TaskDialog"
import { TaskBacklog } from "../../components/Backlog/TaskBacklog"
import { useParams } from "react-router-dom"
import { TableTickets } from "../../components/TicketView/TableTickets"
import { useIdentityServerApi } from "../../api/IdentityServerApi"
import { CreateTicket } from "../../components/TicketView/CreateTicket"
import NotFoundPage from "../NotFoundPage"
import { useOrgLocalStorage } from "../../hooks/useOrgLocalStorage"
import { LoaderMain } from "../../components/LoaderMain"

const Backlog = () => {
    const { getProjectWithTasksById } = useProjectApi();
    const params = useParams<{ id?: string }>();
    const id = params?.id;
    const [project, setProject] = useState<Project>();
    const [tasks, setTasks] = useState<Task[]>([])
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
    const [accountsLoading, setAccountsLoading] = useState(false);
    const { getOrganizationAccounts } = useOrganizationApi();
    const { getAllAccountDetails } = useIdentityServerApi();
    const [accounts, setAccounts] = useState<AccountDetails[]>([]);
    const [dialogOpen, setDialogOpen] = useState(false);
    const [loading, setLoading] = useState(false);
    const [createTicketDialogOpen, setCreateTicketDialogOpen] = useState(false);
    const openTaskDialog = (task: Task) => {
        setSelectedTask(task);
        setDialogOpen(true);
    };
    const closeTaskDialog = () => {
        setSelectedTask(null);
        setDialogOpen(false);
        fetchOrganizationProjects();
    };
    const openCreateTicketDialog = () => {
        setCreateTicketDialogOpen(true);
    };
    const closeCreateTicketDialog = () => {
        setCreateTicketDialogOpen(false);
        fetchOrganizationProjects();
    };

    const fetchOrganizationProjects = async () => {
        setLoading(true);
        try {
            const data = await getProjectWithTasksById(id!)
            const orgAccounts = await getOrganizationAccounts(data.data.project.organizationId);
            const dataAccountDetails = await getAllAccountDetails(orgAccounts.data.accounts);
            setAccounts(dataAccountDetails.data);
            setProject(data.data.project)
            setTasks(data.data.tasks || [])
        } catch (error) {
            console.error('Error fetching projects:', error)
        }
        finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        let intervalId: NodeJS.Timeout

        if (!id) return;

        fetchOrganizationProjects()
        intervalId = setInterval(fetchOrganizationProjects, 60 * 1000)

        return () => clearInterval(intervalId)
    }, [])

    useEffect(() => {
        if (project?.organizationId) {
          localStorage.setItem('organizationId', project?.organizationId);
        }
      }, [project?.organizationId]);
    
      if (!id || loading || !project) return <LoaderMain />;
      if (!id || id === 'undefined') return <NotFoundPage />

    // useEffect(() => {
    //     const handleBacklogUpdate = (event: Event) => {
    //         const customEvent = event as CustomEvent;
    //         const { projectId } = customEvent.detail;
    //         if (projectId) {
    //             const fetchData = async () => {
    //                 const data = await getProjectWithTasksById(projectId);
    //                 setTasks(data.data.tasks || []);
    //             };
    //             fetchData();
    //         }
    //     };

    //     window.addEventListener('updateBacklog', handleBacklogUpdate as EventListener);

    //     return () => {
    //         window.removeEventListener('updateBacklog', handleBacklogUpdate as EventListener);
    //     };
    // }, []);



    return (
        <Container fluid style={{ height: '100vh', padding: 0 }}>
            <Flex direction="column" style={{ height: '100%' }}>
                <Flex justify="space-between" align="center" p="md">
                    <Title order={1}>Backlog</Title>
                    <Button variant="outline" onClick={openCreateTicketDialog}>
                        Add Task
                    </Button>
                </Flex>

                <Flex style={{ flex: 1, overflow: 'hidden' }}>
                    <TableTickets tasks={tasks} accounts={accounts} onTaskClick={openTaskDialog} />
                </Flex>

                {selectedTask && (
                    <TaskDialog
                        task={selectedTask}
                        opened={dialogOpen}
                        onClose={closeTaskDialog}
                        organizationId={project!.organizationId}
                    />
                )}
                {createTicketDialogOpen && (
                    <CreateTicket
                        opened={createTicketDialogOpen}
                        onClose={closeCreateTicketDialog}
                        organizationId={project!.organizationId}
                    />
                )}
            </Flex>
        </Container>
    );

}
export default Backlog