import { Button, Container, Text, Flex, Input, Modal, Title, Space } from "@mantine/core";
import { useAiChatApi } from "../../api/taskManagerApi";
import { useState } from "react";
import { useSafeAuth } from "../../hooks/useSafeAuth";
import type { AiThreadDetails } from "../Types";

type CreateThreadProps = {
    opened: boolean;
    onClose: () => void;
    onSuccess: () => void;
    threadId: string
};

const DeleteThread = ({ opened, onClose, onSuccess, threadId }: CreateThreadProps) => {
    const { deleteThreadById } = useAiChatApi();
    const auth = useSafeAuth();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleThreadDeletion = async () => {
        console.log(threadId)
        if (!threadId)
        {
            onClose()
            setError("no threadId")
        }
        setLoading(true)
        try {
            const response = await deleteThreadById(threadId)

            if (response.status === 200) {
                onSuccess();
                onClose();
            }
        } catch (error) {
            setError('Failed to delete thread');
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
                <Text>Are you sure to delete the current thread? All history will be lost in this chat.</Text>
                <Space h={"md"}></Space>
                <Flex justify={"flex-end"}>
                    <Button variant="outline" onClick={onClose} mr="md">
                        Cancel
                    </Button>
                    <Button
                        onClick={handleThreadDeletion}
                        loading={loading}
                        disabled={loading}
                        bg={"red.9"}
                    >
                        Delete Thread
                    </Button>
                </Flex>
            </Container>
        </Modal>
    );
}

export default DeleteThread;