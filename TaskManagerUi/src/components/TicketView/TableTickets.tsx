import { Badge, Table } from "@mantine/core";
import type { AccountDetails, Task } from "../Types";
import { TaskTypesBadge } from "./TaskTypeBadge";

type TaskTableProps = {
  tasks: Task[];
  accounts: AccountDetails[];
  onTaskClick: (task: Task) => void;
};


export const TaskTable = ({ tasks, accounts, onTaskClick }: TaskTableProps) => {
    const rows = tasks.map((task) => (
        <Table.Tr
            key={task.id}
            onClick={() => onTaskClick(task)}
            style={{ cursor: 'pointer' }}
        >
            <Table.Td>{task.title}</Table.Td>
            <Table.Td>{
                    accounts.find((account: AccountDetails) => account.id == task.reporterId)?.firstName
                    + ' ' +
                    accounts.find((account: AccountDetails) => account.id == task.reporterId)?.lastName
                    || 'Unassigned'
                }</Table.Td>
            <Table.Td>
                {
                    accounts.find((account: AccountDetails) => account.id == task.assigneeId)?.firstName
                    + ' ' +
                    accounts.find((account: AccountDetails) => account.id == task.assigneeId)?.lastName
                    || 'Unassigned'
                }
            </Table.Td>
            <Table.Td><TaskTypesBadge typeId={task.type} /></Table.Td>
            <Table.Td>{task.statusName}</Table.Td>
        </Table.Tr>
    ));

    return (
        <Table striped highlightOnHover withTableBorder withColumnBorders>
            <Table.Thead>
                <Table.Tr>
                <Table.Th>Title</Table.Th>
                <Table.Th>Reporter</Table.Th>
                <Table.Th>Assignee</Table.Th>
                <Table.Th>Type</Table.Th>
                <Table.Th>Status</Table.Th>
                </Table.Tr>
            </Table.Thead>
            <Table.Tbody>{rows}</Table.Tbody>
        </Table>
    );
};