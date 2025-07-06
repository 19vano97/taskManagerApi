import { Button, Container, Fieldset, Flex, Input, Modal, Title } from "@mantine/core";
import { useOrganizationApi, useProjectApi } from "../../api/taskManagerApi";
import { useEffect, useState } from "react";
import type { AccountDetails } from "../Types";
import { useIdentityServerApi } from "../../api/IdentityServerApi";
import { AccountDropdown } from "../DropdownData/AccountDropdown";

type CreateProjectProps = {
    organizationId: string;
    opened: boolean;
    onClose: () => void;
    onSuccess: () => void;
};

const CreateProject = ({organizationId, opened, onClose, onSuccess}: CreateProjectProps) => {
    const { createProject } = useProjectApi();
    const { getOrganizationAccounts } = useOrganizationApi();
    const { getAllAccountDetails } = useIdentityServerApi();
    const [ title, setTitle ] = useState<string>('');
    const [ description, setDescription ] = useState<string>('');
    const [ selectedAccount, setSelectedAccount ] = useState<AccountDetails | null>(null);
    const [ accounts, setAccounts ] = useState<AccountDetails[]>([]);
    const [ loading, setLoading ] = useState(false);
    const [ error, setError ] = useState<string | null>(null);

    useEffect(() => {
        const fetchAccounts = async () => {
            try {
                setLoading(true);
                const orgAccounts = await getOrganizationAccounts(organizationId);
                const accountDetails = await getAllAccountDetails(orgAccounts.data.accounts);
                setAccounts(accountDetails.data);
            } catch (err) {
                console.error('Failed to fetch accounts:', err);
                setError('Failed to load accounts');
            } finally {
                setLoading(false);
            }
        };

        fetchAccounts();
    }, []);

    const handleProjectCreation = async () => {
        if (!title || !description || !selectedAccount || !selectedAccount.id) {
            setError('Please fill in all fields and select a project owner.');
            return;
        }

        const projectData = {
            title: title,
            description: description,
            ownerId: selectedAccount.id,
            organizationId: organizationId
        };

        try {
            setLoading(true);
            const responce = await createProject(projectData);
            
            if (responce.status === 200) {
                onSuccess();
                onClose(); 
            }
        } catch (err) {
            console.error('Failed to create project:', err);
            setError('Failed to create project');
        } finally {
            setLoading(false);
        }
    }

    return (
        <Modal
            opened={opened}
            onClose={onClose}
            title="Create New Ticket"
            size="xxl"
            withCloseButton
            transitionProps={{ transition: 'fade', duration: 200 }}>
            <Container fluid size={"xxl"}>
                <Title order={2} mb="md">
                    Create Project
                </Title>
                <Fieldset legend="Project Details" mb="md">
                    <Input.Wrapper label="Project Title" required>
                        <Input
                            placeholder="Enter project title"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                        />
                    </Input.Wrapper>
                    <Input.Wrapper label="Description" required mt="md">
                        <Input
                            placeholder="Enter project description"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                        />
                    </Input.Wrapper>
                </Fieldset>
                <Fieldset legend="Project Owner" mb="md">
                    <AccountDropdown 
                        accounts={accounts}
                        selectedAccount={selectedAccount}
                        placeholder="Select Project Owner"
                        onAccountChange={(account) => setSelectedAccount(account)}
                    />
                </Fieldset>
                {error && <div style={{ color: 'red', marginBottom: '10px' }}>{error}</div>}
                <Flex justify={"flex-end"} mt="md">
                    <Button variant="outline" onClick={onClose} mr="md">
                        Cancel
                    </Button>
                    <Button
                        onClick={handleProjectCreation}
                        loading={loading}
                        disabled={loading || !title || !description || !selectedAccount}
                    >
                        Create Project
                    </Button>
                </Flex>
            </Container>
        </Modal>
    )
}

export default CreateProject;