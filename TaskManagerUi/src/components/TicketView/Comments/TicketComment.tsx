import { use, useEffect, useState } from "react";
import type { TaskComment } from "../../Types";
import { useTaskApi } from "../../../api/taskManagerApi";
import { Button, Container, Flex, Paper, Textarea, TextInput, Text, Stack, Box, Group, Divider } from "@mantine/core";
import { useAuth } from "react-oidc-context";
import { LoaderMain } from "../../LoaderMain";

type TicketCommentProps = {
  taskId: string;
};

export default function TicketComment({taskId}: TicketCommentProps){
    const auth = useAuth();
    const { getTaskComments, createTaskComment, editTaskComment, deleteTaskComment } = useTaskApi();
    const [ comments, setComments ] = useState<TaskComment[]>([]);
    const [newComment, setNewComment] = useState("");
    const [ loading, setLoading ] = useState(false);

    const fetchComments = async () => {
        try {
            const response = await getTaskComments(taskId);
            setComments(response.data);
        } catch (error) {
            console.error("Failed to fetch comments:", error);
        }
    };

    useEffect(() => {
        fetchComments();
    }, [taskId]);

    const addComment = async (comment: TaskComment) => {
        try {
            const response = await createTaskComment(comment);
            fetchComments();
        } catch (error) {
            console.error("Failed to add comment:", error);
        }
    };

    const updateComment = async (comment: TaskComment) => {
        try {
            const response = await editTaskComment(comment);
            setComments(comments.map(c => c.id === comment.id ? response.data : c));
        } catch (error) {
            console.error("Failed to update comment:", error);
        }
    };

    const removeComment = async (commentId: string) => {
        try {
            const response = await deleteTaskComment(taskId, commentId);
            setComments(comments.filter(c => c.id !== commentId));
        } catch (error) {
            console.error("Failed to delete comment:", error);
        }
    }

    const handleAddComment = async (newComment: string) => {
        setLoading(true);
        await addComment({
            ticketId: taskId,
            accountId: auth.user?.profile.sub!,
            message: newComment} as TaskComment);
        setNewComment("");
        setLoading(false);
    }

    return (
        <Box>
      <Stack gap="md">
        <Textarea
          placeholder="Write a comment..."
          autosize
          minRows={2}
          value={newComment}
          onChange={(e) => setNewComment(e.currentTarget.value)}
        />
        <Group justify="flex-end">
          <Button loading={loading} onClick={(e) => handleAddComment(newComment)} disabled={!newComment.trim()}>
            Post Comment
          </Button>
        </Group>

        <Divider label="Comments" labelPosition="center" />

        {loading ? (
          <LoaderMain />
        ) : comments.length === 0 ? (
          <Text c="dimmed">No comments yet</Text>
        ) : (
          comments.map((comment) => (
            <Paper key={comment.id} shadow="xs" p="sm" withBorder>
              <Text size="sm">{comment.message}</Text>
              <Text size="xs" c="dimmed">
                By {comment.account?.firstName + " " + comment.account?.lastName} • {new Date(comment.createDate!).toLocaleString()}
              </Text>
            </Paper>
          ))
        )}
      </Stack>
    </Box>
    );
}