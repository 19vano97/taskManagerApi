import { Button, Container, Fieldset, Flex, Input, Modal, Title } from "@mantine/core";
import { useAiChatApi } from "../../api/taskManagerApi";
import { useState } from "react";
import { useSafeAuth } from "../../hooks/useSafeAuth";
import type { AiThreadDetails } from "../Types";

type CreateThreadProps = {
    opened: boolean;
    onClose: () => void;
    onSuccess: () => void;
    organizationId: string;
};

const CreateThread = ({ opened, onClose, onSuccess, organizationId }: CreateThreadProps) => {
    const { createNewThread } = useAiChatApi();
    const auth = useSafeAuth();
    const [Name, setThread] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleThreadCreation = async () => {
        if (!Name) {
            setError('Please fill in the Name field.')
        }

        const threadData: AiThreadDetails = {
            name: Name!,
            organziationId: organizationId!,
            accountId: auth.user?.profile.sub!
        }

        setLoading(true)
        try {
            const response = await createNewThread(threadData)

            if (response.status === 200) {
                onSuccess();
                onClose();
            }
        } catch (error) {
            setError('Failed to create thread');
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
            <Container fluid size={"xxl"}>
                <Title order={2} mb="md">
                    Create Thread
                </Title>
                <Fieldset legend="Thread Details" mb="md">
                    <Input.Wrapper label="Thread Title" required>
                        <Input
                            placeholder="Enter thread name"
                            value={Name}
                            onChange={(e) => setThread(e.target.value)}
                        />
                    </Input.Wrapper>
                </Fieldset>
                <Flex justify={"flex-end"}>
                    <Button variant="outline" onClick={onClose} mr="md">
                        Cancel
                    </Button>
                    <Button
                        onClick={handleThreadCreation}
                        loading={loading}
                        disabled={loading || !Name}
                    >
                        Create Thread
                    </Button>
                </Flex>
            </Container>
        </Modal>
    );
}

export default CreateThread;