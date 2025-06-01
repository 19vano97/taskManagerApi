import React from 'react';
import { Badge, Flex } from '@mantine/core';

type TaskType = {
  id: number;
  name: string;
};

export const taskTypesConst: TaskType[] = [
  { id: 1, name: 'Task' },
  { id: 2, name: 'Initiative' },
  { id: 3, name: 'Project' },
  { id: 4, name: 'Epic' },
  { id: 5, name: 'Subtask' },
  { id: 6, name: 'Bug' },
  { id: 7, name: 'Change request' },
];

type TaskTypesBadgeProps = {
  typeId: number;
};

export const TaskTypesBadge: React.FC<TaskTypesBadgeProps> = ({ typeId }) => {
  const taskType = taskTypesConst.find((type) => type.id === typeId);
  if (!taskType) {
    return null;
  }

  const getBadgeColor = (id: number): string => {
    switch (id) {
      case 1:
        return 'blue';
      case 2:
        return 'green';
      case 3:
        return 'orange';
      case 4:
        return 'purple';
      case 5:
        return 'cyan';
      case 6:
        return 'red';
      case 7:
        return 'yellow';
      default:
        return 'gray';
    }
  };
  
  return (
    <Flex direction="column" gap="sm">
        <Badge key={typeId} color={getBadgeColor(typeId)} variant="filled">
          {taskType.name}
        </Badge>
    </Flex>
  );
};