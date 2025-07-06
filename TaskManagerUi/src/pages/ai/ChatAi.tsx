import { Container, Tabs, Title } from "@mantine/core";
import { useAiChatApi, useProjectApi } from "../../api/taskManagerApi";
import { useEffect, useState } from "react";
import type { AiThreadDetails, Project } from "../../components/Types";
import { LoaderMain } from "../../components/LoaderMain";
import { AiFullChat } from "../../components/ai/AiFullChat";
import { WrongAlert } from "../../components/alerts/WrongAlert";
import { useOrgLocalStorage } from "../../hooks/useOrgLocalStorage";
import { useParams } from "react-router-dom";
import NotFoundPage from "../NotFoundPage";

export const ChatAi = () => {
    const { getAllThreads } = useAiChatApi();
    const { getProjectById } = useProjectApi();
    const [project, setProject] = useState<Project>();
    const params = useParams<{ id?: string }>();
    const id = params?.id;
    const [allTreads, setAllThreads] = useState<AiThreadDetails[]>([]);
    const [currentThread, setCurrentThread] = useState<AiThreadDetails>();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string>();

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

    useEffect(() => {
        const fetchThreads = async () => {
            setLoading(true);
            try {
                const threads = await getAllThreads();
                setAllThreads(threads.data);
            } catch (error) {
                console.error('Error fetching threads:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchThreads();
    }, []);

    useEffect(() => {
        if (project?.organizationId) {
            localStorage.setItem('organizationId', project?.organizationId);
        }
    }, [project?.organizationId]);

    if (!id || loading || !project) return <LoaderMain />;
    if (!id || id === 'undefined') return <NotFoundPage />

    return (
        <Container fluid>
            <Title order={1}>ChatAi</Title>
            {loading ? (
                <LoaderMain />
            ) : (
                <Tabs>
                    <Tabs.List>
                        {allTreads
                            .filter(thread => typeof thread.id === "string")
                            .map(thread => (
                                <Tabs.Tab key={thread.id} value={thread.id as string} onClick={() => setCurrentThread(thread)}>
                                    {thread.name}
                                </Tabs.Tab>
                            ))}
                    </Tabs.List>
                </Tabs>
            )}
            {currentThread && currentThread.id && (
                <AiFullChat threadId={currentThread.id as string} projectId={id} />
            )}
        </Container>
    );
}