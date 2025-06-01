import { TaskTypesBadge } from '../TicketView/TaskTypeBadge';
import type { Task } from '../Types';
import { Card, Text, Group, Badge } from '@mantine/core';

type TaskCardOnBoardProps = {
    task: Task;
    onClick?: () => void;
};

export const TaskCardOnBoard = ({ task, onClick }: TaskCardOnBoardProps) => {
    return (
        <Card shadow="sm" padding="lg" radius="md" withBorder onClick={onClick} style={{ cursor: 'pointer' }}>
        <Group justify="space-between" mb="xs">
            <Text fw={500}>{task.title}</Text>
            <TaskTypesBadge typeId={task.type} />
        </Group>
        <Group justify="space-between" mt="md">
            <Text size="xs" c="dimmed">Reporter: {task.reporterId}</Text>
            <Text size="xs" c="dimmed">Assignee: {task.assigneeId}</Text>
        </Group>
        </Card>
    );
};

