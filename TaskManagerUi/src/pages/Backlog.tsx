import { useEffect, useState } from "react"
import { useOrganizationApi, useProjectApi } from "../api/taskManagerApi"
import type { AccountDetails, Task } from "../components/Types"
import { Button, Container, Flex, Title } from "@mantine/core"
import { TaskDialog } from "../components/TicketView/TaskDialog"
import { TaskBacklog } from "../components/Backlog/TaskBacklog"
import { useParams } from "react-router-dom"
import { TaskTable } from "../components/TicketView/TableTickets"
import { useIdentityServerApi } from "../api/IdentityServerApi"

const Backlog = () => {
    const { getProjectWithTasksById } = useProjectApi();
    const { id } = useParams<{ id: string }>();
    const [tasks, setTasks] = useState<Task[]>([])
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
    const [accountsLoading, setAccountsLoading] = useState(false);
    const { getOrganizationAccounts } = useOrganizationApi();
    const { getAllAccountDetails } = useIdentityServerApi();
    const [accounts, setAccounts ] = useState<AccountDetails[]>([]);
    const [dialogOpen, setDialogOpen] = useState(false);
    const openTaskDialog = (task: Task) => {
        setSelectedTask(task);
        setDialogOpen(true);
        };

    const closeTaskDialog = () => {
        setSelectedTask(null);
        setDialogOpen(false);
        };

    useEffect(() => {
        let intervalId: NodeJS.Timeout
    
        const fetchOrganizationProjects = async () => {
            try {
                // const data = await getProjectWithTasksById(id || '')
                const data = await getProjectWithTasksById(localStorage.getItem('projectId') || '')
                setTasks(data.tasks || [])
            } catch (error) {
                console.error('Error fetching projects:', error)
            }
        }
    
        fetchOrganizationProjects()
        intervalId = setInterval(fetchOrganizationProjects, 60 * 1000)
        
        return () => clearInterval(intervalId)
        }, [])

        useEffect(() => {
            const handleBacklogUpdate = (event: Event) => {
                const customEvent = event as CustomEvent;
                const { projectId } = customEvent.detail;
                if (projectId) {
                    const fetchData = async () => {
                        const data = await getProjectWithTasksById(projectId);
                        setTasks(data.tasks || []);
                    };
                    fetchData();
                }
            };

            window.addEventListener('updateBacklog', handleBacklogUpdate as EventListener);

            return () => {
                window.removeEventListener('updateBacklog', handleBacklogUpdate as EventListener);
            };
        }, []);

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

        return (
            <Container fluid>
                <Flex justify="space-between" align="center" mb="md">
                    <Flex justify="flex-start" align="center" mb="md">
                        <Title order={1} mb="md">
                            Backlog
                        </Title>
                    </Flex>
                    <Flex align="flex-end" justify={"flex-end"}>
                        <Button
                            variant="outline"
                            onClick={() => { console.log('Add Task Clicked') }}
                            style={{ marginBottom: '20px' }}
                        >
                            Add Task
                        </Button>
                    </Flex>
                </Flex>
                <Flex gap="md" align="flex-start" wrap="wrap">
                    <TaskTable tasks={tasks} accounts={accounts} onTaskClick={openTaskDialog} />
                </Flex>
                {selectedTask && (
                    <TaskDialog task={selectedTask} opened={dialogOpen} onClose={closeTaskDialog} />
                )}
            </Container>
        )
}
export default Backlog