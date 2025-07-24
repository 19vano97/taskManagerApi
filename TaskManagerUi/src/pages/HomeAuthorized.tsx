import {
    Container,
    Grid,
    Card,
    Text,
    Title,
    Avatar,
    Group,
    Button,
    Badge,
    Stack,
    Flex,
} from '@mantine/core';
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useOrganizationApi } from '../api/taskManagerApi';
import { useSafeAuth } from '../hooks/useSafeAuth';
import { Plus } from 'lucide-react';
import type { OrganizationDetails } from '../components/Types';
import CreateOrganization from '../components/Organization/CreateOrgamization';
import SuccessAlert from '../components/alerts/SuccessAlert';

const HomeAuthorized = () => {
    const { getAllOrganizationProjects } = useOrganizationApi();
    const auth = useSafeAuth();
    const navigate = useNavigate();
    const [organizations, setOrganizations] = useState<OrganizationDetails[]>([]);
    const [ownedOrgs, setOwnedOrgs] = useState<OrganizationDetails[]>([]);
    const [createOrganizationModalOpen, setCreateOrganizationModalOpen] = useState(false);
    const [showSuccessOrganizationCreation, setShowSuccessOrganizationCreation] = useState(false);
    const openCreateOrganizationDialog = () => {
        setCreateOrganizationModalOpen(true);
    };
    const closeCreateOrganizationDialog = () => {
        setCreateOrganizationModalOpen(false);
    };

    const fetchOrgs = async () => {
        try {
            const response = await getAllOrganizationProjects();
            const data = response.data;
            const userId = auth?.user?.profile?.sub;

            const owned = data.filter((org) => org.owner === userId);
            const others = data.filter((org) => org.owner !== userId);

            console.log('Owned Organizations:', owned);
            console.log('Other Organizations:', others);
            console.log('userId:', userId);

            setOwnedOrgs(owned);
            setOrganizations(others);
        } catch (error) {
            console.error('Error fetching organizations:', error);
        }
    };

    useEffect(() => {
        fetchOrgs();
    }, [auth?.user?.profile?.sub]);

    const handleOrgSelect = (orgId: string) => {
        navigate(`/org/${orgId}`);
    };

    const handleOrganizationCreationSuccess = async () => {
        await fetchOrgs();
        setShowSuccessOrganizationCreation(true);
        setTimeout(() => setShowSuccessOrganizationCreation(false), 4000);
    }

    return (
        <Container size="xl" py="xl">
            {showSuccessOrganizationCreation && (
                <SuccessAlert title="Organization successfully created!" />
            )}
            <Stack >
                <Flex justify="space-between" align="center">
                    <Title order={2}>Welcome to TaskType</Title>
                    <Button variant="outline" onClick={openCreateOrganizationDialog} leftSection={<Plus size={16} />}>
                        Create Organization
                    </Button>
                </Flex>
                <Text size="md" c="dimmed">
                    Choose an organization to start working on your projects
                </Text>
            </Stack>

            {ownedOrgs.length > 0 && (
                <>
                    <Title order={4} mt="xl">Your Organizations</Title>
                    <Grid mt="sm">
                        {ownedOrgs.map((org) => (
                            <Grid.Col key={org.id} span={{ base: 12, sm: 6, md: 4 }}>
                                <Card shadow="sm" radius="md" withBorder p="lg" style={{ cursor: 'pointer' }} onClick={() => handleOrgSelect(org.id)}>
                                    <Group>
                                        <Avatar color="blue" radius="xl">{org.name[0].toUpperCase()}</Avatar>
                                        <div>
                                            <Text fw={600}>{org.name}</Text>
                                            <Badge color="blue" size="xs" variant="light">Owner</Badge>
                                        </div>
                                    </Group>
                                </Card>
                            </Grid.Col>
                        ))}
                    </Grid>
                </>
            )}

            {organizations.length > 0 && (
                <>
                    <Title order={4} mt="xl">Organizations you’re in</Title>
                    <Grid mt="sm">
                        {organizations.map((org) => (
                            <Grid.Col key={org.id} span={{ base: 12, sm: 6, md: 4 }}>
                                <Card shadow="sm" radius="md" withBorder p="lg" style={{ cursor: 'pointer' }} onClick={() => handleOrgSelect(org.id)}>
                                    <Group>
                                        <Avatar color="teal" radius="xl">{org.name[0].toUpperCase()}</Avatar>
                                        <div>
                                            <Text fw={600}>{org.name}</Text>
                                            <Badge color="gray" size="xs" variant="light">Member</Badge>
                                        </div>
                                    </Group>
                                </Card>
                            </Grid.Col>
                        ))}
                    </Grid>
                </>
            )}

            {ownedOrgs.length == 0 && (
                <Flex justify="center" mt="xl">
                    <Text size="sm" c="dimmed">
                        You dont have any organizations yet. Click "Create Organization" to get started.
                    </Text>
                </Flex>
            )}
            {createOrganizationModalOpen
                && <CreateOrganization
                    opened={createOrganizationModalOpen}
                    onClose={closeCreateOrganizationDialog}
                    onSuccess={handleOrganizationCreationSuccess}
                />}
        </Container>
    );
};

export default HomeAuthorized;