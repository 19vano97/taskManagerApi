import {
  Container,
  Title,
  Grid,
  Card,
  Divider,
  Text,
  Paper,
  Group,
  Avatar,
  Button,
  Flex,
  ActionIcon
} from '@mantine/core';
import { useParams, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { useOrganizationApi } from '../../api/taskManagerApi';
import { useIdentityServerApi } from '../../api/IdentityServerApi';
import { LoaderMain } from '../../components/LoaderMain';
import type { OrganizationDetails, AccountDetails } from '../../components/Types';
import NotFoundPage from '../NotFoundPage';
import { useSafeAuth } from '../../hooks/useSafeAuth';
import CreateProject from '../../components/project/CreateProject';
import SuccessAlert from '../../components/alerts/SuccessAlert';
import { Trash } from 'lucide-react';
import DeleteProject from '../../components/project/DeleteProject';
import { useOrgLocalStorage as checkOrgLocalStorage } from '../../hooks/useOrgLocalStorage';

export default function OrganizationDashboard() {
  const params = useParams<{ id?: string }>();
  const id = params?.id;
  const navigate = useNavigate();
  const { getOrganizationProjectsById } = useOrganizationApi();
  const { getAllAccountDetails } = useIdentityServerApi();
  const auth = useSafeAuth();
  const [organization, setOrganization] = useState<OrganizationDetails | null>(null);
  const [accounts, setAccounts] = useState<AccountDetails[]>([]);
  const [isOwner, setIsOwner] = useState(false);
  const [loading, setLoading] = useState(true);
  const [projectToDelete, setProjectToDelete] = useState<string>();
  const [createProjectModalOpen, setCreateProjectModalOpen] = useState(false);
  const [deleteProjectModalOpen, setDeleteProjectModalOpen] = useState(false);
  const [showSuccessProjectCreation, setShowSuccessProjectCreation] = useState(false);
  const [showSuccessProjectDeletion, setShowSuccessProjectDeletion] = useState(false);
  const openCreateProjectDialog = () => {
    setCreateProjectModalOpen(true);
  };
  const closeCreateProjectDialog = () => {
    setCreateProjectModalOpen(false);
  };
  const openDeleteProjectDialog = (projectId: string) => {
    setProjectToDelete(projectId);
    setDeleteProjectModalOpen(true);
  };
  const closeDeleteProjectDialog = () => {
    setProjectToDelete(null!);
    setDeleteProjectModalOpen(false);
  };

  if (!id) return <NotFoundPage />;

  const fetchOrganization = async () => {
    if (!id) return;

    const data = await getOrganizationProjectsById(id);
    setOrganization(data.data);

    const accDetails = await getAllAccountDetails(data.data.accounts);
    setAccounts(accDetails.data);

    if (auth?.user?.profile.sub === data.data.owner) {
      setIsOwner(true);
    }
  };

  useEffect(() => {
        if (organization?.id) {
      localStorage.setItem('organizationId', organization.id);
    }
  }, [organization?.id]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        await fetchOrganization();
      } catch (err) {
        console.error('Failed to load organization dashboard:', err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [id]);

  const handleProjectCreationSuccess = async () => {
    await fetchOrganization();
    setShowSuccessProjectCreation(true);
    setTimeout(() => setShowSuccessProjectCreation(false), 4000);
  };

  const handleProjectDeletionSuccess = async () => {
    await fetchOrganization();
    setShowSuccessProjectDeletion(true);
    setTimeout(() => setShowSuccessProjectDeletion(false), 4000);
  };

  const getInitials = (first: string, last: string) => `${first?.[0] ?? ''}${last?.[0] ?? ''}`;

  if (loading || !organization) return <LoaderMain />;

  return (
    <>
      {showSuccessProjectCreation && (
        <SuccessAlert title="Project successfully created!" />
      )}
      {showSuccessProjectDeletion && (
        <SuccessAlert title="Project successfully deleted!" />
      )}
      <Container fluid size="xl" py="xl">
        <Title order={2} mb="lg">{organization.name} Overview</Title>

        <Grid gutter="xl">
          <Grid.Col span={{ base: 12 }}>
            <Card withBorder shadow="sm" radius="md" p="lg">
              <Flex justify="space-between" align="center" mb="md">
                <Title order={4}>Available Projects</Title>
                {isOwner && (
                  <Button
                    variant="outline"
                    size="xs"
                    onClick={openCreateProjectDialog}
                  >
                    Create Project
                  </Button>
                )}
              </Flex>
              <Divider my="sm" />
              <Grid>
                {organization.projects.map((project) => (
                  <Grid.Col span={{ base: 12, sm: 6, md: 4 }} key={project.id}>
                    <Card withBorder shadow="xs" radius="md" p="md">
                      <Flex justify={"space-between"}>
                        <Text fw={600}>{project.title}</Text>
                        <ActionIcon
                          color="red"
                          onClick={() => openDeleteProjectDialog(String(project.id))}
                          mt={22}
                        >
                          <Trash size={16} />
                        </ActionIcon>
                      </Flex>
                      <Text size="sm" c="dimmed" lineClamp={2}>{project.description}</Text>
                      <Button
                        size="xs"
                        mt="sm"
                        variant="light"
                        fullWidth
                        onClick={() => navigate(`/project/${project.id}`)}
                      >
                        Open Project
                      </Button>
                    </Card>
                  </Grid.Col>
                ))}
              </Grid>
            </Card>
          </Grid.Col>

          <Grid.Col span={{ base: 12, sm: 6, md: 4 }}>
            <Card withBorder shadow="sm" radius="md" p="lg">
              <Title order={4}>Total Members</Title>
              <Divider my="sm" />
              <Text size="xl">{accounts.length}</Text>
            </Card>
          </Grid.Col>

          <Grid.Col span={{ base: 12, sm: 6, md: 4 }}>
            <Card withBorder shadow="sm" radius="md" p="lg">
              <Title order={4}>Total Projects</Title>
              <Divider my="sm" />
              <Text size="xl">{organization.projects.length}</Text>
            </Card>
          </Grid.Col>

          <Grid.Col span={{ base: 12 }}>
            <Card withBorder shadow="sm" radius="md" p="lg">
              <Title order={4}>Recent Members</Title>
              <Divider my="sm" />
              <Paper>
                {accounts.slice(0, 5).map(acc => (
                  <Group key={acc.id} py="xs">
                    <Avatar color="blue" radius="xl">{getInitials(acc.firstName, acc.lastName)}</Avatar>
                    <div>
                      <Text fw={500}>{acc.firstName} {acc.lastName}</Text>
                      <Text size="xs" c="dimmed">{acc.email}</Text>
                    </div>
                  </Group>
                ))}
              </Paper>
            </Card>
          </Grid.Col>
        </Grid>
        {createProjectModalOpen && (
          <CreateProject
            organizationId={id}
            onClose={closeCreateProjectDialog}
            opened={createProjectModalOpen}
            onSuccess={handleProjectCreationSuccess}
          />
        )}
        {deleteProjectModalOpen && (
          <DeleteProject
            projectId={projectToDelete!}
            opened={deleteProjectModalOpen}
            onClose={closeDeleteProjectDialog}
            onSuccess={handleProjectDeletionSuccess}
          />
        )}
      </Container>
    </>
  );
}
