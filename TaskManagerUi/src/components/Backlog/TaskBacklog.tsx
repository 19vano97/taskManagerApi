import { Card, Flex } from "@mantine/core"
import type { Task } from "../Types"

type TaskProps = {
    task: Task;
    onTaskClick: (task: Task) => void;
};

export const TaskBacklog = ({task, onTaskClick}: TaskProps) => {
    
    return (
        <Card shadow="sm" padding="lg" radius="md" withBorder onClick={() => onTaskClick(task)} style={{ cursor: 'pointer', width: '100%' }}>
            <Flex>
                <p>{task.title}</p>
                <p>Reporter: {task.reporterId}</p>
                <p>Assignee: {task.assigneeId}</p>
                <p>Type: {task.typeName}</p>
                <p>Status: {task.statusName}</p>
            </Flex>
        </Card>
    )
}