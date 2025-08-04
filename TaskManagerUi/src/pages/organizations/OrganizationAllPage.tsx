import {
  Avatar,
  Button,
  Container,
  Group,
  Loader,
  Paper,
  Stack,
  Table,
  Text,
  Title,
} from "@mantine/core";
import { useOrganizationApi } from "../../api/taskManagerApi";
import { useSafeAuth } from "../../hooks/useSafeAuth";
import { useEffect, useState } from "react";
import { useIdentityServerApi } from "../../api/IdentityServerApi";
import { useNavigate } from "react-router-dom";
import type { AccountDetails, Organization } from "../../components/Types";

const OrganizationAllPage = () => {
  const auth = useSafeAuth();
  const { getAllOrganizationProjects: getOrganizationProjects } = useOrganizationApi();
  const [orgOptions, setOrgOptions] = useState<Organization[]>([]);
  const [accounts, setAccounts] = useState<AccountDetails[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const loadOrgs = async () => {
      if (!auth?.isAuthenticated) return;

      try {
        const data = await getOrganizationProjects();
        const owners = data.data.map((org: Organization) => org.owner);
        setOrgOptions(data.data);
        setAccounts(owners as AccountDetails[]);
      } catch (error) {
        console.error("Failed to load organizations:", error);
      } finally {
        setLoading(false);
      }
    };

    loadOrgs();
  }, [auth?.isAuthenticated]);

  const getInitials = (firstName: string, lastName: string) =>
    `${firstName?.[0] ?? ""}${lastName?.[0] ?? ""}`.toUpperCase();

  const tableRows = orgOptions.map((org) => {
    const owner = accounts.find((acc) => acc.id === org.owner);
    const hasOwner = !!owner;

    return (
      <Table.Tr key={org.id}>
        <Table.Td>
          <Text fw={500}>{org.name}</Text>
          <Text size="xs" c="dimmed">
            {org.description}
          </Text>
        </Table.Td>

        <Table.Td>{org.abbreviation}</Table.Td>

        <Table.Td>
          {hasOwner ? (
            <Group gap="sm">
              <Avatar color="blue" radius="xl" size="sm">
                {getInitials(owner.firstName, owner.lastName)}
              </Avatar>
              <div>
                <Text size="sm" fw={500}>
                  {owner.firstName} {owner.lastName}
                </Text>
                <Text size="xs" c="dimmed">
                  {owner.email}
                </Text>
              </div>
            </Group>
          ) : (
            <Text size="sm" c="dimmed">
              Unknown
            </Text>
          )}
        </Table.Td>

        <Table.Td>{org.projects?.length}</Table.Td>
        <Table.Td>{org.accounts?.length}</Table.Td>
        <Table.Td>
          <Button variant="light" color="blue" size="xs" onClick={() => { navigate(`/organization/${org.id}`) }}>
            View
          </Button>
        </Table.Td>
      </Table.Tr>
    );
  });

  return (
    <Container size="lg" py="xl">
      <Stack>
        <Title order={2} fw={600}>
          Organizations
        </Title>

        {loading ? (
          <Group justify="center" py="xl">
            <Loader size="lg" />
          </Group>
        ) : (
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
                  <Table.Th>Name</Table.Th>
                  <Table.Th>Abbreviation</Table.Th>
                  <Table.Th>Owner</Table.Th>
                  <Table.Th>Projects</Table.Th>
                  <Table.Th>Accounts</Table.Th>
                  <Table.Th>Actions</Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>{tableRows}</Table.Tbody>
            </Table>
          </Paper>
        )}
      </Stack>
    </Container>
  );
};

export default OrganizationAllPage;
