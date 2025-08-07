import React, { useEffect, useState } from 'react';
import { Select, Fieldset } from '@mantine/core';
import type { Task } from '../Types';
import { useTaskApi } from '../../api/taskManagerApi';

type TaskDropdownProps = {
    selectedTaskId: Task | null;
    organizationId: string;
    onTaskChange: (taskId: string | null) => void;
};

export const TaskDropdown = ({ selectedTaskId, organizationId, onTaskChange }: TaskDropdownProps) => {
    const { getAllTasksByOrganization } = useTaskApi();
    const [tasks, setTasks] = useState<Task[] | null>(null);

    const fetchTasks = async () => {
        try {
            const data = await getAllTasksByOrganization(organizationId!);
            setTasks(data.data);
        } catch (error) {
            console.error('Error fetching tasks:', error);
        }
    };

    const handleOnClick = () => {
        if (!tasks || tasks.length === 0) {
            fetchTasks();
        }
    }

    const handleTaskChange = (value: string | null) => {
        onTaskChange(value);
    };

    return (
        <Select
            placeholder={
                selectedTaskId?.title || "Select parent task"
            }
            data={
                Array.isArray(tasks)
                    ? tasks.map((task) => ({
                        value: task.id,
                        label: task.title,
                    }))
                    : []
            }
            onClick={handleOnClick}
            value={selectedTaskId?.id}
            onChange={handleTaskChange}
            searchable
        />
    );
};