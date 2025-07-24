import { Button, Container, Flex, Tabs, Title, Text, ActionIcon } from "@mantine/core";
import { useAiChatApi, useProjectApi } from "../../api/taskManagerApi";
import { useEffect, useState } from "react";
import type { AiThreadDetails, Project } from "../../components/Types";
import { LoaderMain } from "../../components/LoaderMain";
import { AiFullChat } from "../../components/ai/AiFullChat";
import { WrongAlert } from "../../components/alerts/WrongAlert";
import { useOrgLocalStorage } from "../../hooks/useOrgLocalStorage";
import { useParams } from "react-router-dom";
import NotFoundPage from "../NotFoundPage";
import CreateThread from "../../components/ai/CreateThread";
import SuccessAlert from "../../components/alerts/SuccessAlert";
import { Trash } from "lucide-react";
import DeleteThread from "../../components/ai/DeleteThread";

export const ChatAi = () => {
    const { getAllThreads } = useAiChatApi();
    const { getProjectById } = useProjectApi();
    const [project, setProject] = useState<Project>();
    const params = useParams<{ id?: string }>();
    const id = params?.id;
    const [allThreads, setAllThreads] = useState<AiThreadDetails[]>([]);
    const [currentThread, setCurrentThread] = useState<AiThreadDetails>();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string>();
    const [threadToDelete, setThreadToDelete] = useState<string>();
    const [createThreadModalOpen, setCreateThreadModalOpen] = useState(false);
    const [deleteThreadModalOpen, setDeleteThreadModalOpen] = useState(false);
    const [showSuccessThreadCreation, setShowSuccessThreadCreation] = useState(false);
    const [showSuccessThreadDeletion, setShowSuccessThreadDeletion] = useState(false);
    const openCreateThreadDialog = () => {
        setCreateThreadModalOpen(true);
    };
    const closeCreateThreadDialog = () => {
        setCreateThreadModalOpen(false);
    };
    const openDeleteThreadDialog = (organizationId: string) => {
        setThreadToDelete(organizationId);
        setDeleteThreadModalOpen(true);
    };
    const closeDeleteThreadDialog = () => {
        setThreadToDelete(null!);
        setDeleteThreadModalOpen(false);
    };

    useEffect(() => {
        const fetchProject = async () => {
            setLoading(true)
            try {
                if (!id) return;
                const project = await getProjectById(id);
                setProject(project.data);
            } catch (err) {
                console.log(err)
            } finally {
                setLoading(false)
            }
        }
        fetchProject()
    }, [id])

    const fetchThreads = async () => {
        setLoading(true);
        try {
            const threads = await getAllThreads();
            const rawData = threads.data as AiThreadDetails[] | { threads?: AiThreadDetails[] };
            setAllThreads(Array.isArray(rawData) ? rawData : (rawData.threads ?? []));
        } catch (error) {
            console.error('Error fetching threads:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchThreads();
    }, []);

    useEffect(() => {
        if (project?.organizationId) {
            localStorage.setItem('organizationId', project?.organizationId);
        }
    }, [project?.organizationId]);

    if (!id || loading || !project) return <LoaderMain />;
    if (!id || id === 'undefined') return <NotFoundPage />

    const handleThreadCreationSuccess = async () => {
        fetchThreads();
        setShowSuccessThreadCreation(true);
        setTimeout(() => setShowSuccessThreadCreation(false), 4000);
    }

    const handleThreadDeleteionSuccess = async () => {
        setCurrentThread(null!)
        fetchThreads();
        setShowSuccessThreadDeletion(true);
        setTimeout(() => setShowSuccessThreadDeletion(false), 4000);
    }

    return (
        <Container fluid w={"100%"}>
            {showSuccessThreadCreation && (
                <SuccessAlert title="Thread successfully created!" />
            )}
            {showSuccessThreadDeletion && (
                <SuccessAlert title="Thread successfully deleted!" />
            )}
            <Flex justify={"space-between"}>
                <Title order={1}>ChatAi</Title>
                <Button
                    variant="outline"
                    size="xs"
                    onClick={openCreateThreadDialog}
                >
                    Create Thread
                </Button>
            </Flex>
            {loading ? (
                <LoaderMain />
            ) : (
                allThreads.length === 0 ? (
                    <Text>No threads</Text>
                ) : (
                    <Tabs w={"100%"}>
                        <Tabs.List>
                            {allThreads
                                .filter(thread => typeof thread.id === "string")
                                .map(thread => (
                                    <Tabs.Tab key={thread.id} value={thread.id as string} onClick={() => setCurrentThread(thread)}>
                                        <Flex justify={"space-between"} align={"center"} gap={"sm"}>
                                            {thread.name}
                                            <ActionIcon
                                                component="span"
                                                color="red"
                                                onClick={() => openDeleteThreadDialog(thread.id!)}
                                                size={"input-xxs"}
                                            >
                                                <Trash size={12} />
                                            </ActionIcon>
                                        </Flex>
                                    </Tabs.Tab>
                                ))}
                        </Tabs.List>
                    </Tabs>
                )
            )}
            {currentThread && currentThread.id && (
                <AiFullChat threadId={currentThread.id as string} projectId={id} />
            )}
            {createThreadModalOpen && <CreateThread
                opened={createThreadModalOpen}
                onClose={closeCreateThreadDialog}
                onSuccess={handleThreadCreationSuccess}
                organizationId={project.organizationId}
            />}
            {deleteThreadModalOpen && <DeleteThread
                opened={deleteThreadModalOpen}
                onClose={closeDeleteThreadDialog}
                onSuccess={handleThreadDeleteionSuccess}
                threadId={threadToDelete!}
            />}
        </Container>
    );
}