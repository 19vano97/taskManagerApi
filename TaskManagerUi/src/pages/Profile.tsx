import { useEffect, useState } from "react";
import {
  Avatar,
  Button,
  Card,
  Container,
  Flex,
  Group,
  Stack,
  Text,
  TextInput,
  Title,
} from "@mantine/core";
import { useIdentityServerApi } from "../api/IdentityServerApi";
import type { AccountDetails } from "../components/Types";
import { LoaderMain } from "../components/LoaderMain";

const Profile = () => {
  const { getAccountDetails, postAccountData } = useIdentityServerApi();
  const [accountDetails, setAccountDetails] = useState<AccountDetails | null>(null);
  const [editing, setEditing] = useState(false);
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');

  useEffect(() => {
    const fetchAccountDetails = async () => {
      try {
        const data = await getAccountDetails();
        setAccountDetails(data.data);
        setFirstName(data.data.firstName);
        setLastName(data.data.lastName);
      } catch (error) {
        console.error("Error fetching account details:", error);
      }
    };

    fetchAccountDetails();
  }, []);

  const handleSave = async () => {
    if (!accountDetails) return;
    try {
      await postAccountData({
        ...accountDetails,
        firstName,
        lastName,
      });
      setAccountDetails({ ...accountDetails, firstName, lastName });
      setEditing(false);
    } catch (error) {
      console.error("Error updating account details:", error);
    }
  };

  if (!accountDetails) return <LoaderMain />;

  const initials = `${firstName[0] ?? ""}${lastName[0] ?? ""}`;

  return (
    <Container size="sm" mt="xl">
      <Title order={1} mb="lg" ta="center">
        Your Profile
      </Title>

      <Card shadow="sm" padding="xl" radius="md" withBorder>
        <Flex direction="column" align="center" gap="md">
          <Avatar size={100} radius="xl" color="blue">
            {initials.toUpperCase()}
          </Avatar>

          <Stack gap="xs" w="100%" maw={400}>
            <Text size="sm" c="dimmed">
              Email:
            </Text>
            <Text fw={500}>{accountDetails.email}</Text>

            <Text size="sm" c="dimmed" mt="md">
              First Name:
            </Text>
            {editing ? (
              <TextInput
                value={firstName}
                onChange={(e) => setFirstName(e.currentTarget.value)}
              />
            ) : (
              <Text fw={500}>{firstName}</Text>
            )}

            <Text size="sm" c="dimmed" mt="md">
              Last Name:
            </Text>
            {editing ? (
              <TextInput
                value={lastName}
                onChange={(e) => setLastName(e.currentTarget.value)}
              />
            ) : (
              <Text fw={500}>{lastName}</Text>
            )}
          </Stack>

          <Group mt="lg">
            {editing ? (
              <>
                <Button variant="filled" onClick={handleSave}>
                  Save
                </Button>
                <Button variant="light" onClick={() => setEditing(false)}>
                  Cancel
                </Button>
              </>
            ) : (
              <Button variant="outline" onClick={() => setEditing(true)}>
                Edit Information
              </Button>
            )}
          </Group>
        </Flex>
      </Card>
    </Container>
  );
};

export default Profile;
