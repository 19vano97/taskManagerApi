import { Button, Fieldset, Flex, Input, Modal, Title } from "@mantine/core";
import { useState } from "react";
import { useOrganizationApi } from "../../api/taskManagerApi";
import { useSafeAuth } from "../../hooks/useSafeAuth";

type CreateOrganizationProps = {
    opened: boolean;
    onClose: () => void;
    onSuccess: () => void;
};

const CreateOrganization = ({ opened, onClose, onSuccess }: CreateOrganizationProps) => {
    const auth = useSafeAuth();
    const { postCreateOrganization } = useOrganizationApi();
    const [title, setTitle] = useState<string>('');
    const [abbreviation, setAbbreviation] = useState<string>('');
    const [description, setDescription] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleOrganizationCreation = async () => {
        if (!title || !description || !abbreviation) {
            setError('Please fill in all fields');
            return;
        }

        const orgData = {
            name: title,
            description: description,
            abbreviation: abbreviation,
            owner: auth.user?.profile.sub!
        };

        try {
            setLoading(true);
            const responce = await postCreateOrganization(orgData);
            
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
            withCloseButton
            transitionProps={{ transition: 'fade', duration: 200 }}
        >
            <Title order={2} mb="md">
                Create Organization
            </Title>
            <Fieldset legend="Title" mb="md" w={"100%"}>
                <Input.Wrapper label="Organization Title" required>
                    <Input
                        placeholder="Enter project title"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                    />
                </Input.Wrapper>
            </Fieldset>
            <Fieldset legend="Abbreviation" mb="md" w={"100%"}>
                <Input.Wrapper label="Abbreviation" required mt="md">
                    <Input
                        placeholder="Enter project abbreviation"
                        value={abbreviation}
                        onChange={(e) => setAbbreviation(e.target.value)}
                    />
                </Input.Wrapper>
            </Fieldset>
            <Fieldset legend="Description" mb="md" w={"100%"}>
                <Input.Wrapper label="Description" required mt="md">
                    <Input
                        placeholder="Enter project description"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                    />
                </Input.Wrapper>
            </Fieldset>
            <Flex justify={"flex-end"}>
                <Button variant="outline" onClick={onClose} mr="md">
                    Cancel
                </Button>
                <Button
                    onClick={handleOrganizationCreation}
                    loading={loading}
                    disabled={loading || !title || !description || !abbreviation}
                >
                    Create
                </Button>
            </Flex>
        </Modal>
    );
}

export default CreateOrganization;