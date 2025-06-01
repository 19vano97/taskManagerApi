import { useEffect, useState } from "react"
import { useProjectApi } from "../api/taskManagerApi"
import type { Task } from "../components/Types"
import { Button, Container, Flex, Title } from "@mantine/core"
import { TaskDialog } from "../components/TicketView/TaskDialog"
import { TaskBacklog } from "../components/Backlog/TaskBacklog"

const Backlog = () => {
    const { getProjectWithTasksById } = useProjectApi()

    const [tasks, setTasks] = useState<Task[]>([])
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
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
            const handleKanbanUpdate = (event: Event) => {
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

            window.addEventListener('updateKanban', handleKanbanUpdate as EventListener);

            return () => {
                window.removeEventListener('updateKanban', handleKanbanUpdate as EventListener);
            };
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
                    {tasks.map((task) => (
                        <TaskBacklog
                            key={task.id}
                            task={task}
                            onTaskClick = {openTaskDialog}  
                        />
                    ))}
                </Flex>
                {selectedTask && (
                    <TaskDialog task={selectedTask} opened={dialogOpen} onClose={closeTaskDialog} />
                )}
            </Container>
        )
}
export default Backlog