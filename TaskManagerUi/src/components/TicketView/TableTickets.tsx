import {
    Avatar,
    Badge,
    Flex,
    ScrollArea,
    Table,
    Text,
} from '@mantine/core';
import { TaskTypesBadge } from './TaskTypeBadge';
import type { AccountDetails, Task } from '../Types';

type TaskTableProps = {
    tasks: Task[];
    accounts: AccountDetails[];
    onTaskClick: (task: Task) => void;
};

const getInitials = (firstName?: string, lastName?: string) =>
    `${firstName?.[0] ?? ''}${lastName?.[0] ?? ''}`.toUpperCase();

const getAccount = (id: string | undefined, accounts: AccountDetails[]) =>
    accounts.find((acc) => acc.id === id);

export const TableTickets = ({ tasks, accounts, onTaskClick }: TaskTableProps) => {
    const rows = tasks
        .filter((task) => task.statusName?.toLowerCase() !== 'done')
        .map((task) => {

            return (
                <Table.Tr
                    key={task.id}
                    onClick={() => onTaskClick(task)}
                    style={{ cursor: 'pointer' }}
                >
                    <Table.Td>
                        <Text fw={500}>{task.title}</Text>
                    </Table.Td>

                    <Table.Td>
                        {task.reporter ? (
                            <Flex>
                                <Avatar size="sm" radius="xl" color="blue">
                                    {getInitials(task.reporter?.firstName, task.reporter?.firstName)}
                                </Avatar>
                                <Text ml="xs">{task.reporter?.firstName} {task.reporter?.firstName}</Text>
                            </Flex>
                        ) : (
                            <Badge variant="light" color="gray">
                                Unassigned
                            </Badge>
                        )}
                    </Table.Td>

                    <Table.Td>
                        {task.assignee ? (
                            <Flex>
                                <Avatar size="sm" radius="xl" color="blue">
                                    {getInitials(task.assignee?.firstName, task.assignee?.lastName)}
                                </Avatar>
                                <Text ml="xs">{task.assignee?.firstName} {task.assignee?.lastName}</Text>
                            </Flex>
                        ) : (
                            <Badge variant="light" color="gray">
                                Unassigned
                            </Badge>
                        )}
                    </Table.Td>

                    <Table.Td>
                        <TaskTypesBadge typeId={task.typeId ?? 0} />
                    </Table.Td>

                    <Table.Td>
                        <Badge variant="light" color="indigo" radius="sm">
                            {task.statusName}
                        </Badge>
                    </Table.Td>
                </Table.Tr>
            );
        });

    return (
        <ScrollArea style={{ width: '100%', height: '100%' }}>
            <Table
                striped
                highlightOnHover
                withTableBorder
                withColumnBorders
                verticalSpacing="md"
                style={{ minWidth: '900px' }}
            >
                <Table.Thead>
                    <Table.Tr>
                        <Table.Th>Task</Table.Th>
                        <Table.Th>Reporter</Table.Th>
                        <Table.Th>Assignee</Table.Th>
                        <Table.Th>Type</Table.Th>
                        <Table.Th>Status</Table.Th>
                    </Table.Tr>
                </Table.Thead>
                <Table.Tbody>{rows}</Table.Tbody>
            </Table>
        </ScrollArea>
    );
};
