import { Card, Container, Flex } from '@mantine/core';
import type { Status, Task } from '../Types';
import { TaskCardOnBoard } from './Task';

type ColumnProps = {
    status: Status;
    tasks: Task[];
    onTaskClick: (task: Task) => void;
};

export const Column = ({ status, tasks, onTaskClick }: ColumnProps) => {
    const background = status.typeId === 2
        ? 'linear-gradient(135deg, #f0f4f8, #d9e2ec)'
        : status.typeId === 3
            ? 'linear-gradient(135deg, #ede9fe, #ddd6fe)'
        : status.typeId === 4
            ? 'linear-gradient(135deg, #dcfce7, #bbf7d0)'
        : 'linear-gradient(135deg, #f9fafb, #f3f4f6)';

    return (
        <Container>
            <Flex direction="column" gap="md" style={{ minWidth: '300px' }}>
                <Card shadow="sm" 
                    padding="lg" 
                    radius="md" 
                    withBorder
                    style={{ background }}> 
                    <h2>{status.statusName}</h2>
                    <Flex direction="column" gap="md">
                        {tasks.map((task) => (
                            <TaskCardOnBoard key={task.id} task={task} onClick={() => onTaskClick(task)} />
                        ))}
                    </Flex>
                </Card>
            </Flex>
        </Container>
    );
};
