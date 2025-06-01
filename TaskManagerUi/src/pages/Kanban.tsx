import { useEffect, useState } from 'react'
import { useProjectApi } from '../api/taskManagerApi'
import { Container, Flex, Button, Title } from '@mantine/core';
import { Column } from '../components/Kanban/Column';
import type { Project, Status, Task } from '../components/Types';
import { TaskDialog } from "../components/TicketView/TaskDialog"
import { CreateTicket } from '../components/TicketView/CreateTicket';
import { useProject } from '../context/ProjectContext';


const Kanban = () => {
  const { getProjectWithTasksById } = useProjectApi();
  const [project, setProject] = useState<Project | null>(null);
  const [statuses, setStatuses] = useState<Status[]>([]);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [taskDialogOpen, setTaskDialogOpen] = useState(false); //
  const [createTicketDialogOpen, setCreateTicketDialogOpen] = useState(false);

  const openTaskDialog = (task: Task) => {
    setSelectedTask(task);
    setTaskDialogOpen(true);
  };

  const closeTaskDialog = () => {
    setSelectedTask(null);
    setTaskDialogOpen(false);
  };

  const openCreateTicketDialog = () => {
    setCreateTicketDialogOpen(true);
  };

  const closeCreateTicketDialog = () => {
    setCreateTicketDialogOpen(false);
  };

  const { selectedProjectId } = useProject();

  useEffect(() => {
    const fetchData = async () => {
      const projectId = selectedProjectId;
      if (!projectId) return;

      const data = await getProjectWithTasksById(projectId);
      setTasks(data.tasks || []);
    };

    fetchData();
  }, [selectedProjectId]);

  useEffect(() => {
    let intervalId: NodeJS.Timeout;

    if (localStorage.getItem('organizationId') !== null && localStorage.getItem('projectId') !== null) {
      const fetchOrganizationProjects = async () => {
        try {
          const data = await getProjectWithTasksById(localStorage.getItem('projectId') || '');
          setStatuses(data.project.statuses || []);
          setProject(data.project);
          setTasks(data.tasks || []);
        } catch (error) {
          console.error('Error fetching projects:', error);
        }
      };
      fetchOrganizationProjects();
      intervalId = setInterval(fetchOrganizationProjects, 60 * 1000);
    }

    return () => clearInterval(intervalId);
  }, []);

  useEffect(() => {
    const handleKanbanUpdate = (event: Event) => {
      const customEvent = event as CustomEvent;
      const { projectId } = customEvent.detail;
      if (projectId) {
        const fetchData = async () => {
          const data = await getProjectWithTasksById(projectId);
          setTasks(data.tasks || []);
          setStatuses(data.project.statuses || []);
          setProject(data.project);
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
      {localStorage.getItem('organizationId') !== null && localStorage.getItem('projectId') !== null ? (
        <Container fluid p="md">
          <Flex justify="space-between" align="center" mb="md">
            <Title order={2}>Kanban Board</Title>
            <Button variant="outline" onClick={openCreateTicketDialog}>
              Add Task
            </Button>
          </Flex>
          <Flex gap="md" align="flex-start">
            {statuses.map((status) => (
              <Column
                key={status.statusId}
                status={status}
                tasks={tasks.filter((task) => task.statusId === status.statusId)}
                onTaskClick={openTaskDialog}
              />
            ))}
          </Flex>

          {selectedTask && (
            <TaskDialog task={selectedTask} opened={taskDialogOpen} onClose={closeTaskDialog} />
          )}
          {createTicketDialogOpen && (
            <CreateTicket opened={createTicketDialogOpen} onClose={closeCreateTicketDialog} />
          )}
        </Container>
      ) : (
        <Flex justify="center" align="center" style={{ height: '100vh' }}>
          <h1>Please select organization and project</h1>
        </Flex>
      )}
    </Container>
  );
};

export default Kanban;
