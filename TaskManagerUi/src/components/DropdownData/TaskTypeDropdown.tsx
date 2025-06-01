import React from 'react';
import { Select, Fieldset } from '@mantine/core';
import type { TaskType } from '../Types';

type TaskTypeDropdownProps = {
  taskType: TaskType | null;
  taskTypes: TaskType[];
  onTaskTypeChange: (taskType: TaskType | null) => void;
};

export const TaskTypeDropdown: React.FC<TaskTypeDropdownProps> = ({ taskType, taskTypes, onTaskTypeChange }) => {
  const handleTaskTypeChange = (value: string | null) => {
    const selectedType = taskTypes.find((type) => String(type.id) === value) || null;
    onTaskTypeChange(selectedType);
  };

  return (
    <Select
      placeholder={
        taskType
          ? taskType.name
          : "Select type"
      }
      data={
        taskTypes.map((type) => ({
          value: String(type.id),
          label: type.name,
        }))
      }
      value={taskType?.id ? String(taskType.id) : null}
      onChange={handleTaskTypeChange}
      searchable
    />
  );
};