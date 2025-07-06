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
} from '@mantine/core';
import { useNavigate, useParams } from 'react-router-dom';
import { useOrganizationApi } from '../../api/taskManagerApi';
import { useEffect, useState } from 'react';
import type { AccountDetails, OrganizationDetails } from '../../components/Types';
import { useIdentityServerApi } from '../../api/IdentityServerApi';
import { useSafeAuth } from '../../hooks/useSafeAuth';
import AddMemberToOrganization from '../../components/Account/AddMemberToOrganization';
import NotFoundPage from '../NotFoundPage';
import { LoaderMain } from '../../components/LoaderMain';
import { useOrgLocalStorage } from '../../hooks/useOrgLocalStorage';

const OrganizationSettingsPage = () => {
  const params = useParams<{ id?: string }>();
  const id = params?.id;
  const { getOrganizationProjectsById, postEditOrganization: editOrganization } = useOrganizationApi();
  const { getAllAccountDetails } = useIdentityServerApi();
  const auth = useSafeAuth();
  const navigate = useNavigate();

  const [organization, setOrganization] = useState<OrganizationDetails>();
  const [accounts, setAccounts] = useState<AccountDetails[]>([]);
  const [loading, setLoading] = useState(true);
  const [editMode, setEditMode] = useState(false);
  const [name, setName] = useState('');
  const [abbreviation, setAbbreviation] = useState('');
  const [showAddMember, setShowAddMember] = useState(false);

  if (!id || id === 'undefined') {
    return <NotFoundPage />;
  }

  useEffect(() => {
    const fetchOrganization = async () => {
      try {
        const data = await getOrganizationProjectsById(id!);
        const uniqueAccountIds: string[] = Array.from(new Set(data.data.accounts));
        const accountDetails = await getAllAccountDetails(uniqueAccountIds);

        setOrganization(data.data);
        setName(data.data.name);
        setAbbreviation(data.data.abbreviation);
        setAccounts(accountDetails.data);
      } catch (error) {
        console.error('Failed to fetch organization:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchOrganization();
  }, []);

  useEffect(() => {
    if (organization?.id) {
      localStorage.setItem('organizationId', organization.id);
    }
  }, [organization?.id]);

  const isOwner = auth?.user?.profile.sub === organization?.owner;

  const handleSave = async () => {
    if (!organization) return;

    try {
      const updatedOrg = {
        ...organization,
        name,
        abbreviation,
        modifyDate: new Date().toISOString(),
      };
      await editOrganization(organization.id, updatedOrg);
      setOrganization(updatedOrg);
      setEditMode(false);
    } catch (error) {
      console.error('Failed to update organization:', error);
    }
  };

  const owner = accounts.find((acc) => acc.id === organization?.owner);

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

  return (
    <Container fluid size="xl" py="xl">
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
                <Button variant="light" onClick={() => setShowAddMember(!showAddMember)}>
                  {showAddMember ? 'Close' : 'Add Member'}
                </Button>
              )}
            </Group>
            <Divider my="sm" />
            {showAddMember && <AddMemberToOrganization organizationId={organization.id} />}
            <Paper mt="md" withBorder p="md">
              {accounts.map((acc) => (
                <Group key={acc.id} py="xs" >
                  <Avatar radius="xl" color="blue">
                    {acc.firstName?.[0]}{acc.lastName?.[0]}
                  </Avatar>
                  <div>
                    <Text fw={500}>{acc.firstName} {acc.lastName}</Text>
                    <Text size="xs" c="dimmed">{acc.email}</Text>
                  </div>
                </Group>
              ))}
            </Paper>
          </Card>
        </Tabs.Panel>

        <Tabs.Panel value="projects" pt="md">
          <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Title order={3} mb="sm">Projects</Title>
            <Grid>
              {organization.projects.map((project) => {
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
                    </Card>
                  </Grid.Col>
                );
              })}
            </Grid>
          </Card>
        </Tabs.Panel>
      </Tabs>
    </Container>
  );
};

export default OrganizationSettingsPage;
