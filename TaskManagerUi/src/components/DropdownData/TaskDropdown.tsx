import React, { useEffect, useState } from 'react';
import { Select } from '@mantine/core';
import type { Task } from '../Types';
import { useTaskApi } from '../../api/taskManagerApi';

type TaskDropdownProps = {
    selectedTaskId: Task | null;
    organizationId: string;
    onTaskChange: (taskId: string | null) => void;
};

export const TaskDropdown = ({
    selectedTaskId: selectedTask,
    organizationId,
    onTaskChange,
}: TaskDropdownProps) => {
    const { getAllTasksByOrganization } = useTaskApi();
    const [tasks, setTasks] = useState<Task[]>([]);
    const [loading, setLoading] = useState(false);

    const fetchTasks = async () => {
        setLoading(true);
        try {
            const data = await getAllTasksByOrganization(organizationId);
            setTasks(data.data ?? []);
        } catch (error) {
            console.error('Error fetching tasks:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (organizationId) {
            fetchTasks();
        }
    }, [organizationId]);

    const handleTaskChange = (value: string | null) => {
        onTaskChange(value);
    };

    return (
        <Select
            placeholder={selectedTask?.title || 'Select parent task'}
            data={tasks.map((task) => ({
                value: task.id,
                label: task.title,
            }))}
            value={selectedTask?.id ?? null}
            onChange={handleTaskChange}
            searchable
            disabled={loading}
        />
    );
};