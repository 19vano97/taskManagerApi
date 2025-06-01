import React from 'react';
import { Select, Fieldset } from '@mantine/core';
import type { Task } from '../Types';

type TaskDropdownProps = {
  selectedTaskId: string | null;
  tasks: Task[] | null;
  onTaskChange: (taskId: string | null) => void;
};

export const TaskDropdown: React.FC<TaskDropdownProps> = ({ selectedTaskId, tasks, onTaskChange }) => {
    const handleTaskChange = (value: string | null) => {
        onTaskChange(value);
    };

    return (
            <Select
                placeholder={
                selectedTaskId || "Select parent task"
                }
                data={
                tasks?.map((task) => ({
                    value: task.id,
                    label: task.title,
                })) || []
                }
                value={selectedTaskId}
                onChange={handleTaskChange}
                searchable
            />
    );
};