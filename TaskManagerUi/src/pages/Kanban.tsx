import { useEffect, useState } from 'react'
import { useOrganizationApi, useProjectApi, useTaskApi } from '../api/taskManagerApi'
import { DragDropContext, type DropResult } from '@hello-pangea/dnd';
import { Container, Flex, Button, Title } from '@mantine/core';
import { Column } from '../components/Kanban/Column';
import type { AccountDetails, Project, Status, Task } from '../components/Types';
import { TaskDialog } from "../components/TicketView/TaskDialog"
import { CreateTicket } from '../components/TicketView/CreateTicket';
import { useProject } from '../context/ProjectContext';
import { useParams } from 'react-router-dom';
import { useIdentityServerApi } from '../api/IdentityServerApi';


const Kanban = () => {
  const { getProjectWithTasksById } = useProjectApi();
  const { editTask } = useTaskApi();
  const { getAllAccountDetails } = useIdentityServerApi();
  const { getOrganizationAccounts } = useOrganizationApi();
  const { selectedProjectId } = useProject();
  const { id } = useParams<{ id: string }>();
  const [project, setProject] = useState<Project | null>(null);
  const [accountsLoading, setAccountsLoading] = useState(false);
  const [accounts, setAccounts ] = useState<AccountDetails[]>([]);
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

    // if (localStorage.getItem('organizationId') !== null && id !== null) {
    if (localStorage.getItem('organizationId') !== null && localStorage.getItem('projectId') !== null) {
      const fetchOrganizationProjects = async () => {
        try {
          // const data = await getProjectWithTasksById(id || '');
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

  useEffect(() => {
      const fetchOrganizationAccounts = async () => {
          setAccountsLoading(true);
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

  const onDragEnd = (result: DropResult) => {
    const { source, destination, draggableId } = result;
    if (!destination) return;

    if (source.droppableId !== destination.droppableId) {
      const updatedTasks = tasks.map((task) =>
        task.id === draggableId ? { ...task, statusId: parseInt(destination.droppableId) } : task
      );

      setTasks(updatedTasks);
      handleEditTask(draggableId, parseInt(destination.droppableId));
    }
  };

  const handleEditTask = async (id: string, statusId: number) => {
        const taskData = {
            id: id,
            statusId: statusId || 0,
        };

        try {
            const response = await editTask(id, taskData);
        } catch (error) {
            console.error('Error creating task:', error);
        }
    }

  return (
  <Container fluid>
    <Container fluid p="md">
      <Flex justify="space-between" align="center" mb="md">
        <Title order={2}>Kanban Board</Title>
        <Button variant="outline" onClick={openCreateTicketDialog}>
          Add Task
        </Button>
      </Flex>

      {localStorage.getItem('organizationId') !== null && localStorage.getItem('projectId') !== null ? (
        <DragDropContext onDragEnd={onDragEnd}>
          <Flex gap="md" align="flex-start">
            {statuses.map((status) => (
              <Column
                key={status.statusId}
                status={status}
                tasks={tasks.filter((task) => task.statusId === status.statusId)}
                accounts={accounts}
                onTaskClick={openTaskDialog}
              />
            ))}
          </Flex>
        </DragDropContext>
      ) : (
        <Flex justify="center" align="center" style={{ height: '40vh' }}>
          <h1>Please select organization and project</h1>
        </Flex>
      )}

      {selectedTask && (
        <TaskDialog task={selectedTask} opened={taskDialogOpen} onClose={closeTaskDialog} />
      )}
      {createTicketDialogOpen && (
        <CreateTicket opened={createTicketDialogOpen} onClose={closeCreateTicketDialog} />
      )}
    </Container>
  </Container>
);

};

export default Kanban;
