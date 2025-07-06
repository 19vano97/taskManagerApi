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
import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { useOrganizationApi } from "../../api/taskManagerApi";
import { useIdentityServerApi } from "../../api/IdentityServerApi";
import { useSafeAuth } from "../../hooks/useSafeAuth";
import type { AccountDetails, OrganizationDetails } from "../../components/Types";
import NotFoundPage from "../NotFoundPage";
import AddMemberToOrganization from "../../components/Account/AddMemberToOrganization";
import { useOrgLocalStorage } from "../../hooks/useOrgLocalStorage";

export default function OrganizationMembersPage() {
  const params = useParams<{ id?: string }>();
  const id = params?.id;
  const { getOrganizationProjectsById } = useOrganizationApi();
  const { getAllAccountDetails } = useIdentityServerApi();
  const auth = useSafeAuth();
  const [organization, setOrganization] = useState<OrganizationDetails | null>(null);
  const [accounts, setAccounts] = useState<AccountDetails[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAddMember, setShowAddMember] = useState(false);

  if (!id) return <NotFoundPage />;

  useEffect(() => {
    const fetchOrganization = async () => {
      try {
        if (!id) return;
        const org = await getOrganizationProjectsById(id);
        setOrganization(org.data);
        const details = await getAllAccountDetails(org.data.accounts);
        setAccounts(details.data);
      } catch (error) {
        console.error("Failed to load organization:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchOrganization();
  }, [id]);

  useEffect(() => {
    if (organization?.id) {
      localStorage.setItem('organizationId', organization.id);
    }
  }, [organization?.id]);

  const isOwner = auth?.user?.profile.sub === organization?.owner;

  const getInitials = (firstName: string, lastName: string) =>
    `${firstName?.[0] ?? ""}${lastName?.[0] ?? ""}`.toUpperCase();

  if (!id) return <NotFoundPage />;

  return (
    <Container size="lg" py="xl">
      <Stack>
        <Group justify="space-between">
          <Title order={2}>Organization Members</Title>
          {isOwner && (
            <Button variant="outline" onClick={() => setShowAddMember((v) => !v)}>
              {showAddMember ? "Close" : "Add Member"}
            </Button>
          )}
        </Group>

        {showAddMember && organization && (
          <AddMemberToOrganization organizationId={organization.id} />
        )}

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
        )}
      </Stack>
    </Container>
  );
}
