import { Button, Container, Modal, Title, Text, Flex } from "@mantine/core"
import { useProjectApi } from "../../api/taskManagerApi";
import { useState } from "react";

type DeleteProjectProps = {
    projectId: string;
    opened: boolean;
    onClose: () => void;
    onSuccess: () => void;
}

const DeleteProject = ({ projectId, opened, onClose, onSuccess }: DeleteProjectProps) => {
    const { deleteProject } = useProjectApi();
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const handleProjectDeletion = async () => {
        try {
            setLoading(true);
            const responce = await deleteProject(projectId);
            onSuccess();
            onClose();
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
            size="md"
            withCloseButton
            transitionProps={{ transition: 'fade', duration: 200 }}
        >
            <Container fluid>
                <Title order={2} mb="md">
                    Delete Project
                </Title>
                <Text mb="md">
                    Are you sure you want to delete this project? This action cannot be undone.
                </Text>
                <Flex justify={"space-between"} gap={"sm"}>
                    <Button onClick={onClose} w={"50%"}>Close</Button>
                    <Button color="red" w={"50%"} onClick={handleProjectDeletion}>
                        Delete Project
                    </Button>
                </Flex>
            </Container>
        </Modal>
    )
}

export default DeleteProject;