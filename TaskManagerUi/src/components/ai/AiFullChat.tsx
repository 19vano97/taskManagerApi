import {
    Container,
    Paper,
    Stack,
    Text,
    Title,
    Group,
    TextInput,
    Button,
    Box,
    Flex,
    useMantineColorScheme,
    Spoiler,
} from "@mantine/core";
import { useAiChatApi, useTaskApi } from "../../api/taskManagerApi";
import { useEffect, useState } from "react";
import type { ChatMessage, TicketAiView } from "../Types";
import { Bot, User } from "lucide-react";
import TaskDate from "../TicketView/TaskDate";
import { useRef } from "react";
import { TaskTypesBadge } from "../TicketView/TaskTypeBadge";
import React from "react";
import { LoaderMain } from "../LoaderMain";
import { useSafeAuth } from "../../hooks/useSafeAuth";

type AiFullChatProps = {
    threadId: string;
    projectId: string;
};

export const AiFullChat = ({ threadId, projectId }: AiFullChatProps) => {
    const { getChatHistoryByThreadId, postSendMessageToChat } = useAiChatApi();
    const { createTicketsForAi } = useTaskApi();
    const auth = useSafeAuth();
    const [chatHistory, setChatHistory] = useState<ChatMessage[]>([]);
    const [loading, setLoading] = useState(true);
    const [newMessage, setNewMessage] = useState("");
    const [sending, setSending] = useState(false);
    const chatEndRef = useRef<HTMLDivElement | null>(null);
    const { colorScheme } = useMantineColorScheme();
    const isDark = colorScheme === 'dark';

    useEffect(() => {
        chatEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [chatHistory]);

    const fetchChatHistory = async () => {
        setLoading(true);
        try {
            const history = await getChatHistoryByThreadId(threadId);
            setChatHistory(history.data);
        } catch (error) {
            console.error("Error fetching chat history:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchChatHistory();
    }, [threadId]);

    const sortedHistory = [...chatHistory].sort((a, b) => {
        const aTime = a.createDate ? new Date(a.createDate).getTime() : 0;
        const bTime = b.createDate ? new Date(b.createDate).getTime() : 0;
        return aTime - bTime;
    });

    const handleSend = async () => {
        if (!newMessage.trim()) return;

        const userMessage: ChatMessage = {
            role: "user",
            content: newMessage.trim(),
            IsAutomatedTicketCreationFlag: false,
            createDate: new Date(),
        };

        const placeholderMessage: ChatMessage = {
            role: "assistant",
            content: "AI is writing...",
            IsAutomatedTicketCreationFlag: false,
            createDate: new Date(Date.now() + 1),
        };

        setChatHistory((prev) => [...prev, userMessage, placeholderMessage]);
        setNewMessage("");
        setSending(true);

        try {
            const response = await postSendMessageToChat(userMessage, threadId);

            setChatHistory((prev) => {
                const updated = [...prev];
                const placeholderIndex = updated.findIndex(
                    (msg) =>
                        msg.role === "assistant" &&
                        msg.content === "AI is writing..."
                );
                if (placeholderIndex !== -1) {
                    updated[placeholderIndex] = {
                        ...response.data,
                        createDate: new Date(),
                    };
                } else {
                    updated.push(response.data);
                }
                return updated;
            });
        } catch (error) {
            console.error("Error sending message:", error);
        } finally {
            setSending(false);
        }
    };

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            handleSend();
        }
    };

    const handleCreateTasksRequest = () => {

    }

    const handleCreateTasksFromMessage = async (tickets: TicketAiView[]) => {
        tickets.forEach(element => {
            element.assigneeId = auth.user?.profile.sub;
            element.reporterId = auth.user?.profile.sub;
            element.projectId = projectId
        });

        console.log(tickets);

        try {
            const responce = await createTicketsForAi(tickets);
            console.log(responce);
        } catch (error) {
            console.log(error)
        }
    };

    return (
        <Container size="md" px="md" py="md">
            <Title order={3} mb="md">
                AI Conversation
            </Title>

            {loading ? (
                <Group justify="center" py="xl">
                    <LoaderMain />
                </Group>
            ) : chatHistory.length === 0 ? (
                <Text c="dimmed">No messages in this chat thread.</Text>
            ) : (
                <Container fluid>
                    <Stack>
                        {sortedHistory.map((message, index) => {
                            const isUser = message.role === "user";
                            return (
                                <Group
                                    key={index}
                                    justify={isUser ? "flex-end" : "flex-start"}
                                    align="flex-start"
                                >
                                    {!isUser && <Bot size={18} style={{ marginTop: 4 }} />}
                                    <Paper
                                        shadow="xs"
                                        p="sm"
                                        radius="md"
                                        withBorder
                                        maw="80%"
                                        c={"black"}
                                        bg={isUser ? isDark ? "green.5" : "green.1" : isDark ? "gray.5" : "gray.1"}
                                    >
                                        {(() => {
                                            let parsedTickets: TicketAiView[] | null = null;
                                            try {
                                                const parsed = JSON.parse(message.content);
                                                if (
                                                    Array.isArray(parsed) &&
                                                    parsed.every(
                                                        (item) =>
                                                            typeof item.title === "string" &&
                                                            typeof item.description === "string" &&
                                                            typeof item.type === "number" &&
                                                            (typeof item.parentName === "string" || item.parentName === null)
                                                    )
                                                ) {
                                                    parsedTickets = parsed;
                                                }
                                            } catch (e) { }

                                            return parsedTickets ? (
                                                <Stack>
                                                    <Spoiler
                                                        maxHeight={120}
                                                        showLabel={<Text size="sm" c="black">Show more</Text>}
                                                        hideLabel="Hide"
                                                    >
                                                        {parsedTickets.map((ticket, i) => (
                                                            <React.Fragment key={i}>
                                                                <Paper withBorder p="md" radius="md" shadow="xs" c={isUser ? isDark ? "blue.1" : "black" : isDark ? "blue.1" : "black"}>
                                                                    <Stack gap="xs">
                                                                        <Title size="sm" order={5}>{ticket.title}</Title>
                                                                        <Text size="sm" dangerouslySetInnerHTML={{ __html: ticket.description }} />
                                                                        <Group gap="xs">
                                                                            <Text size="xs" c="dimmed">Type:</Text>
                                                                            <TaskTypesBadge typeId={ticket.type} />
                                                                        </Group>
                                                                    </Stack>
                                                                </Paper>
                                                            </React.Fragment>
                                                        ))}
                                                    </Spoiler>
                                                    <Flex justify={"flex-end"}>
                                                        <Button
                                                            mt="sm"
                                                            size="xs"
                                                            onClick={() => handleCreateTasksFromMessage(parsedTickets!)}
                                                        // color={isUser ? isDark ? "green.5" : "green.1" : isDark ? "gray.5" : "gray.1"}
                                                        >
                                                            Create tasks in the project
                                                        </Button>
                                                    </Flex>

                                                </Stack>
                                            ) : (
                                                <Text size="sm" style={{ whiteSpace: "pre-wrap", wordBreak: "break-word" }}>
                                                    {message.content}
                                                </Text>
                                            );
                                        })()}
                                        {message.createDate && (
                                            <Box mt={4}>
                                                <TaskDate dateString={message.createDate.toString()} />
                                            </Box>
                                        )}
                                    </Paper>
                                    {isUser && <User size={18} style={{ marginTop: 4 }} />}
                                </Group>
                            );
                        })}
                    </Stack>
                    <div ref={chatEndRef} />
                </Container>
            )}

            <Box mt="md">
                <Flex gap="sm" align="flex-end">
                    <TextInput
                        placeholder="Type your message..."
                        value={newMessage}
                        onChange={(e) => setNewMessage(e.currentTarget.value)}
                        onKeyDown={handleKeyDown}
                        disabled={sending}
                        style={{ flex: 1 }}
                    />
                    <Button onClick={handleSend} loading={sending} disabled={!newMessage.trim()}>
                        Send
                    </Button>
                    <Button onClick={handleCreateTasksRequest} loading={sending} disabled={!newMessage.trim()}>
                        Create tasks
                    </Button>
                </Flex>
            </Box>
        </Container>
    );
};