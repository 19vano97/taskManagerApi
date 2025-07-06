import { TaskTypesBadge } from '../TicketView/TaskTypeBadge';
import type { AccountDetails, Task } from '../Types';
import { Card, Text, Group, Badge } from '@mantine/core';

type TaskCardOnBoardProps = {
    task: Task;
    reporter: AccountDetails;
    assignee: AccountDetails;
    onClick?: () => void;
};

export const TaskCardOnBoard = ({ task, reporter, assignee, onClick }: TaskCardOnBoardProps) => {
    return (
        <Card shadow="sm" padding="lg" radius="md" withBorder onClick={onClick} style={{ cursor: 'pointer' }}>
            <Group justify="space-between" mb="xs">
                <Text fw={500}>{task.title}</Text>
                <TaskTypesBadge typeId={task.typeId ?? 0} />
            </Group>
            <Group justify="space-between" mt="md">
                <Text size="xs" c="dimmed">Reporter: {reporter ? reporter.firstName + ' ' + reporter.lastName : null}</Text>
                <Text size="xs" c="dimmed">Assignee: {assignee ? assignee.firstName + ' ' + assignee.lastName : null}</Text>
            </Group>
        </Card>
    );
};

