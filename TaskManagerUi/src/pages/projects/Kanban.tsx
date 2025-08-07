import { useEffect, useState } from 'react'
import { useOrganizationApi, useProjectApi, useTaskApi } from '../../api/taskManagerApi'
import { DragDropContext, type DropResult } from '@hello-pangea/dnd';
import { Container, Flex, Button, Title, Text, Loader } from '@mantine/core';
import { Column } from '../../components/Kanban/Column';
import type { AccountDetails, Project, Status, Task } from '../../components/Types';
import { TaskDialog } from "../../components/TicketView/TaskDialog"
import { CreateTicket } from '../../components/TicketView/CreateTicket';
import { useProject } from '../../context/ProjectContext';
import { useParams } from 'react-router-dom';
import { useIdentityServerApi } from '../../api/IdentityServerApi';
import NotFoundPage from '../NotFoundPage';
import { useOrgLocalStorage } from '../../hooks/useOrgLocalStorage';
import { LoaderMain } from '../../components/LoaderMain';
import SuccessAlert from '../../components/alerts/SuccessAlert';


const Kanban = () => {
  const params = useParams<{ id?: string }>();
  const id = params?.id;
  const { getProjectWithTasksById } = useProjectApi();
  const { editTask } = useTaskApi();
  const { getOrganizationAccounts } = useOrganizationApi();
  const { selectedProjectId } = useProject();
  const [project, setProject] = useState<Project | null>(null);
  const [accounts, setAccounts] = useState<AccountDetails[]>([]);
  const [statuses, setStatuses] = useState<Status[]>([]);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [taskDialogOpen, setTaskDialogOpen] = useState(false); //
  const [createTicketDialogOpen, setCreateTicketDialogOpen] = useState(false);
  const [showSuccessTicketCreation, setShowSuccessTicketCreation] = useState(false);
  const [showSuccessTicketEdit, setShowSuccessTicketEdit] = useState(false);
  const [loading, setLoading] = useState(true);
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

  const fetchData = async () => {
    const projectId = selectedProjectId;
    if (!projectId) return;

    const data = await getProjectWithTasksById(projectId);
    setTasks(data.data.tickets || []);
  };

  useEffect(() => {
    fetchData();
  }, [selectedProjectId]);

  const fetchProjectTasks = async () => {
    setLoading(true);
    try {
      const data = await getProjectWithTasksById(id!);
      const orgAccounts = await getOrganizationAccounts(data.data.organizationId);
      setAccounts(orgAccounts.data.accounts || []);
      setStatuses(data.data.statuses || []);
      setProject(data.data);
      setTasks(data.data.tickets || []);
    } catch (error) {
      console.error('Error fetching projects:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (createTicketDialogOpen || taskDialogOpen) return;

    let intervalId: NodeJS.Timeout;

    if (id) {
      fetchProjectTasks();
      intervalId = setInterval(fetchProjectTasks, 60 * 1000);
    }

    return () => {
      if (intervalId) clearInterval(intervalId);
    };
  }, []);

  useEffect(() => {
    if (project?.organizationId) {
      localStorage.setItem('organizationId', project?.organizationId);
    }
  }, [project?.organizationId]);

  if (!id || loading || !project) return <LoaderMain />;
  if (!id || id === 'undefined') return <NotFoundPage />

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
      if (response.status === 200) {
        handleTicketEditSuccess
      }
    } catch (error) {
      console.error('Error creating task:', error);
    }
  }

  const handleTicketCreationSuccess = async () => {
    await fetchData();
    setShowSuccessTicketCreation(true);
    setTimeout(() => setShowSuccessTicketCreation(false), 4000);
  }

  const handleTicketEditSuccess = async () => {
    await fetchData();
    setShowSuccessTicketEdit(true);
    setTimeout(() => setShowSuccessTicketEdit(false), 4000);
  }

  return (
    <Container fluid>
      {showSuccessTicketCreation && (
        <SuccessAlert title="Ticket successfully created!" />
      )}
      {showSuccessTicketEdit && (
        <SuccessAlert title="Ticket successfully edited!" />
      )}
      <Container fluid p="md">
        <Flex justify="space-between" align="center" mb="md">
          <Title order={2}>Kanban Board</Title>
          <Button variant="outline" onClick={openCreateTicketDialog}>
            Add Task
          </Button>
        </Flex>

        {loading ? (
          <LoaderMain />
        ) : (
          <Flex gap="md" wrap="wrap" justify="space-between">

            <DragDropContext onDragEnd={onDragEnd}>
              <Flex gap="md" align="flex-start">
                {statuses
                  .slice()
                  .sort((a, b) => a.order - b.order)
                  .map((status) => (
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

          </Flex>
        )}

        {selectedTask && (
          <TaskDialog
            task={selectedTask}
            opened={taskDialogOpen}
            onClose={closeTaskDialog}
            organizationId={project!.organizationId}
          />
        )}
        {createTicketDialogOpen && (
          <CreateTicket
            opened={createTicketDialogOpen}
            onClose={closeCreateTicketDialog}
            organizationId={project!.organizationId}
            onSuccess={handleTicketCreationSuccess}
            projectId={id}
          />
        )}
      </Container>
    </Container>
  );

};

export default Kanban;
