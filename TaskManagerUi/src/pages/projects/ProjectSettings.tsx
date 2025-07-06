import { Button, Container, Flex, TextInput, Title, Text, Paper, Group, Divider, Card, Grid, Badge } from "@mantine/core";
import { useParams } from "react-router-dom";
import { useProjectApi } from "../../api/taskManagerApi";
import { useEffect, useState } from "react";
import type { AccountDetails, Project, Status, Task } from "../../components/Types";
import NotFoundPage from "../NotFoundPage";
import { useSafeAuth } from "../../hooks/useSafeAuth";
import { useIdentityServerApi } from "../../api/IdentityServerApi";
import { LoaderMain } from "../../components/LoaderMain";
import { ProjectStatusEditor } from "./components/ProjectStatusEditor";
import { useOrgLocalStorage } from "../../hooks/useOrgLocalStorage";

const ProjectSettings = () => {
  const params = useParams<{ id?: string }>();
  const id = params?.id;
  const { getProjectById, editProject, getProjectWithTasksById } = useProjectApi();
  const [project, setProject] = useState<Project | null>(null);
  const { getAllAccountDetails } = useIdentityServerApi();
  const auth = useSafeAuth();
  const [projectOwner, setProjectOwner] = useState<AccountDetails>();
  const [loading, setLoading] = useState(true);
  const [editMode, setEditMode] = useState(false);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [statuses, setStatuses] = useState<Status[]>([]);
  const [tasks, setTasks] = useState<Task[]>([]);

  if (!id || id === 'undefined') return <NotFoundPage />;

  useEffect(() => {
    const fetchProject = async () => {
      try {
        const data = await getProjectById(id!);
        const accountDetails = await getAllAccountDetails([data.data.ownerId]);
        setProject(data.data);
        setTitle(data.data.title);
        setDescription(data.data.description);
        setStatuses(data.data.statuses || []);
        setProjectOwner(accountDetails.data.find(acc => acc.id === data.data.ownerId));
      } catch (error) {
        console.error("Failed to fetch project:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchProject();
  }, []);

  useEffect(() => {
    if (project?.organizationId) {
      localStorage.setItem('organizationId', project?.organizationId);
    }
  }, [project?.organizationId]);

  useEffect(() => {
    let intervalId: NodeJS.Timeout;

    if (id) {
      const fetchOrganizationProjects = async () => {
        setIsLoading(true);
        try {
          const data = await getProjectWithTasksById(id!);
          setStatuses(data.data.project.statuses || []);
          setProject(data.data.project);
          setTasks(data.data.tasks || []);
        } catch (error) {
          console.error('Error fetching projects:', error);
        } finally {
          setIsLoading(false);
        }
      };
      fetchOrganizationProjects();
      intervalId = setInterval(fetchOrganizationProjects, 60 * 1000);
    }

    return () => {
      if (intervalId) clearInterval(intervalId);
    };
  }, []);

  const isOwner = auth?.user?.profile.sub === project?.ownerId;

  if (id === undefined) {
    return <NotFoundPage />;
  }

  const handleSave = async () => {
    if (!project) return;

    try {
      const updatedProject = {
        id: project.id,
        title,
        description,
        ownerId: project.ownerId,
        organizationId: project.organizationId,
        statuses,
      };
      console.log(updatedProject)

      await editProject(updatedProject, project!.id!);
      setProject({ ...project, title, description, statuses });
      setEditMode(false);
    } catch (error) {
      console.error("Failed to update project:", error);
    }
  };

  const totalTasks = tasks.length;
  const doneTasks = tasks.filter((task) => {
    const status = statuses.find((s) => s.statusId === task.statusId);
    return status?.typeId === 4;
  }).length;

  return (
    <Container fluid size="lg" py="xl">
      <Paper withBorder shadow="sm" radius="md" p="lg">
        <Group justify="space-between" mb="md">
          <Title order={2}>Project Details</Title>
          {isOwner && !editMode && (
            <Button variant="light" onClick={() => setEditMode(true)}>Edit</Button>
          )}
        </Group>

        <Divider my="sm" />

        {loading || !project ? (
          <LoaderMain />
        ) : editMode && isOwner ? (
          <Flex direction="column" gap="sm">
            <TextInput
              label="Title"
              value={title}
              onChange={(e) => setTitle(e.currentTarget.value)}
            />
            <TextInput
              label="Description"
              value={description}
              onChange={(e) => setDescription(e.currentTarget.value)}
            />
            <ProjectStatusEditor
              statuses={statuses}
              onChange={(newStatuses) => setStatuses(newStatuses)}
              isEditable={isOwner}
            />
            <Group mt="md">
              <Button onClick={handleSave}>Save</Button>
              <Button variant="default" onClick={() => setEditMode(false)}>Cancel</Button>
            </Group>
          </Flex>
        ) : (
          <Flex direction="column" gap="xs">
            <Text><strong>Title:</strong> {project.title}</Text>
            <Text><strong>Description:</strong> {project.description}</Text>
            <Text>
              <strong>Owner:</strong>{' '}
              {projectOwner ? `${projectOwner.firstName} ${projectOwner.lastName} (${projectOwner.email})` : 'Unknown'}
            </Text>
            <ProjectStatusEditor statuses={statuses} onChange={() => { }} isEditable={false} />
          </Flex>
        )}
      </Paper>

      <Grid mt="xl">
        <Grid.Col span={{ base: 12, sm: 6, md: 4 }}>
          <Card withBorder shadow="sm" radius="md" p="lg">
            <Title order={4}>Task Statistics</Title>
            <Divider my="sm" />
            <Text component="div">
              Total Tasks: <Badge color="gray">{totalTasks}</Badge>
            </Text>
            <Text component="div">
              Completed Tasks: <Badge color="green">{doneTasks}</Badge>
            </Text>
            <Text component="div">
              In Progress: <Badge color="blue">{totalTasks - doneTasks}</Badge>
            </Text>
          </Card>
        </Grid.Col>
      </Grid>
    </Container>
  );
};

export default ProjectSettings;