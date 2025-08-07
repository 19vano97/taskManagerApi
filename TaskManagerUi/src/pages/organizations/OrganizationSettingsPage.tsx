import {
  Container,
  TextInput,
  Button,
  Title,
  Grid,
  Card,
  Divider,
  Paper,
  Group,
  Text,
  Avatar,
  Tabs,
  Flex,
  Table,
} from '@mantine/core';
import { useNavigate, useParams } from 'react-router-dom';
import { useOrganizationApi } from '../../api/taskManagerApi';
import { useEffect, useState } from 'react';
import type { AccountDetails, Organization } from '../../components/Types';
import { useIdentityServerApi } from '../../api/IdentityServerApi';
import { useSafeAuth } from '../../hooks/useSafeAuth';
import AddMemberToOrganization from '../../components/Account/AddMemberToOrganization';
import NotFoundPage from '../NotFoundPage';
import { LoaderMain } from '../../components/LoaderMain';
import { useOrgLocalStorage } from '../../hooks/useOrgLocalStorage';
import DeleteProject from '../../components/project/DeleteProject';
import SuccessAlert from '../../components/alerts/SuccessAlert';

const OrganizationSettingsPage = () => {
  const params = useParams<{ id?: string }>();
  const id = params?.id;
  const { getOrganizationProjectsById, postEditOrganization: editOrganization } = useOrganizationApi();
  const auth = useSafeAuth();
  const navigate = useNavigate();

  const [organization, setOrganization] = useState<Organization>();
  const [accounts, setAccounts] = useState<AccountDetails[]>([]);
  const [loading, setLoading] = useState(true);
  const [editMode, setEditMode] = useState(false);
  const [name, setName] = useState('');
  const [abbreviation, setAbbreviation] = useState('');
  const [projectToDelete, setProjectToDelete] = useState<string>();
  const [deleteProjectModalOpen, setDeleteProjectModalOpen] = useState(false);
  const [AddMemberModalOpen, setAddMemberModalOpen] = useState(false);
  const [showSuccessMemeberAdding, setShowSuccessMemeberAdding] = useState(false);
  const [showSuccessProjectDeletion, setShowSuccessProjectDeletion] = useState(false);
  const openDeleteProjectDialog = (projectId: string) => {
    setProjectToDelete(projectId);
    setDeleteProjectModalOpen(true);
  };
  const closeDeleteProjectDialog = () => {
    setProjectToDelete(null!);
    setDeleteProjectModalOpen(false);
  };
  const openAddMemberDialog = () => {
    setAddMemberModalOpen(true);
  };
  const closeAddMemberDialog = () => {
    setAddMemberModalOpen(false);
  };

  if (!id || id === 'undefined') {
    return <NotFoundPage />;
  }

  const fetchOrganization = async () => {
    try {
      const data = await getOrganizationProjectsById(id!);

      setOrganization(data.data);
      setName(data.data.name);
      setAbbreviation(data.data.abbreviation || '');
      setAccounts(data.data.accounts || []);
    } catch (error) {
      console.error('Failed to fetch organization:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrganization();
  }, []);

  useEffect(() => {
    if (organization?.id) {
      localStorage.setItem('organizationId', organization.id);
    }
  }, [organization?.id]);

  const isOwner = auth?.user?.profile.sub === organization?.ownerId;

  const handleSave = async () => {
    if (!organization) return;

    try {
      const updatedOrg = {
        ...organization,
        name,
        abbreviation,
        modifyDate: new Date().toISOString(),
      };
      await editOrganization(organization.id!, updatedOrg);
      setOrganization(updatedOrg);
      setEditMode(false);
    } catch (error) {
      console.error('Failed to update organization:', error);
    }
  };

  const handleProjectDeletionSuccess = async () => {
    await fetchOrganization();
    setShowSuccessProjectDeletion(true);
    setTimeout(() => setShowSuccessProjectDeletion(false), 4000);
  };

  const handleAddingMemberSuccess = async () => {
    await fetchOrganization();
    setShowSuccessMemeberAdding(true);
    setTimeout(() => setShowSuccessMemeberAdding(false), 4000);
  };

  const owner = accounts.find((acc) => acc.id === organization?.ownerId);

  if (loading || !organization) {
    return (
      <Container fluid>
        <LoaderMain />
      </Container>
    );
  }

  if (!id) {
    return <NotFoundPage />;
  }
  const getInitials = (firstName: string, lastName: string) =>
    `${firstName?.[0] ?? ""}${lastName?.[0] ?? ""}`.toUpperCase();

  return (
    <Container fluid size="xl" py="xl">
      {showSuccessProjectDeletion && (
        <SuccessAlert title="Project successfully deleted!" />
      )}
      {showSuccessMemeberAdding && (
        <SuccessAlert title="Membet has been successfully added" />
      )}
      <Tabs defaultValue="info">
        <Tabs.List>
          <Tabs.Tab value="info">Organization Info</Tabs.Tab>
          <Tabs.Tab value="members">Members</Tabs.Tab>
          <Tabs.Tab value="projects">Projects</Tabs.Tab>
        </Tabs.List>

        <Tabs.Panel value="info" pt="md">
          <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Group justify="space-between">
              <Title order={3}>Organization Info</Title>
              {isOwner && !editMode && (
                <Button variant="light" onClick={() => setEditMode(true)}>
                  Edit
                </Button>
              )}
            </Group>
            <Divider my="sm" />
            {editMode ? (
              <Flex direction="column" gap="sm">
                <TextInput label="Name" value={name} onChange={(e) => setName(e.currentTarget.value)} />
                <TextInput label="Abbreviation" value={abbreviation} onChange={(e) => setAbbreviation(e.currentTarget.value)} />
                <Group>
                  <Button onClick={handleSave}>Save</Button>
                  <Button variant="default" onClick={() => setEditMode(false)}>
                    Cancel
                  </Button>
                </Group>
              </Flex>
            ) : (
              <Flex direction="column" gap="xs">
                <Text><strong>Name:</strong> {organization.name}</Text>
                <Text><strong>Abbreviation:</strong> {organization.abbreviation}</Text>
                <Text>
                  <strong>Owner:</strong>{' '}
                  {owner ? `${owner.firstName} ${owner.lastName} (${owner.email})` : 'Unknown'}
                </Text>
              </Flex>
            )}
          </Card>
        </Tabs.Panel>

        <Tabs.Panel value="members" pt="md">
          <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Group justify="space-between">
              <Title order={3}>Organization Members</Title>
              {isOwner && (
                <Button variant="outline" onClick={openAddMemberDialog}>
                  Add Member
                </Button>
              )}
            </Group>
            <Divider my="sm" />
            <Paper withBorder shadow="xs" radius="md" p="md">
              <Table
                striped
                highlightOnHover
                withTableBorder
                withColumnBorders
                verticalSpacing="md"
              >
                <Table.Thead>
                  <Table.Tr>
                    <Table.Th>Avatar</Table.Th>
                    <Table.Th>First Name</Table.Th>
                    <Table.Th>Last Name</Table.Th>
                    <Table.Th>Email</Table.Th>
                    <Table.Th>Created</Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {accounts.map((user) => (
                    <Table.Tr key={user.id}>
                      <Table.Td>
                        <Avatar color="blue" radius="xl">
                          {getInitials(user.firstName, user.lastName)}
                        </Avatar>
                      </Table.Td>
                      <Table.Td>{user.firstName}</Table.Td>
                      <Table.Td>{user.lastName}</Table.Td>
                      <Table.Td>{user.email}</Table.Td>
                      <Table.Td>
                        {user.createDate ? new Date(user.createDate).toLocaleDateString() : "N/A"}
                      </Table.Td>
                    </Table.Tr>
                  ))}
                </Table.Tbody>
              </Table>
            </Paper>
          </Card>
        </Tabs.Panel>

        <Tabs.Panel value="projects" pt="md">
          <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Title order={3} mb="sm">Projects</Title>
            <Grid>
              {organization.projects?.map((project) => {
                const projectOwner = accounts.find((acc) => acc.id === project.ownerId);
                return (
                  <Grid.Col span={{ base: 12, sm: 6, md: 4 }} key={project.id}>
                    <Card withBorder shadow="xs" radius="md" p="md">
                      <Text fw={600}>{project.title}</Text>
                      <Text size="sm" c="dimmed" lineClamp={2}>{project.description}</Text>
                      <Text size="xs" mt="xs">
                        Owner: {projectOwner ? `${projectOwner.firstName} ${projectOwner.lastName}` : 'Unknown'}
                      </Text>
                      <Button
                        size="xs"
                        mt="sm"
                        variant="light"
                        fullWidth
                        onClick={() => navigate(`/project/${project.id}`)}
                      >
                        Open Project
                      </Button>
                      {isOwner ? (<Button
                        color="red.6"
                        onClick={() => openDeleteProjectDialog(String(project.id))}
                        size="xs"
                        mt="sm"
                        variant="light"
                        fullWidth
                      >
                        Delete project
                      </Button>) : (<></>)}
                    </Card>
                  </Grid.Col>
                );
              })}
            </Grid>
          </Card>
        </Tabs.Panel>
      </Tabs>
      {deleteProjectModalOpen && (
        <DeleteProject
          projectId={projectToDelete!}
          opened={deleteProjectModalOpen}
          onClose={closeDeleteProjectDialog}
          onSuccess={handleProjectDeletionSuccess}
        />
      )}
      {AddMemberModalOpen && (
        <AddMemberToOrganization
          organizationId={organization.id!}
          opened={AddMemberModalOpen}
          onClose={closeAddMemberDialog}
          onSuccess={handleAddingMemberSuccess}
        />
      )}
    </Container>
  );
};

export default OrganizationSettingsPage;
