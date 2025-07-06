// src/components/ProjectStatusEditor.tsx
import {
  ActionIcon,
  Button,
  Group,
  Paper,
  Select,
  Stack,
  TextInput,
} from '@mantine/core';
import { GripVertical, Contrast, Plus, Trash } from 'lucide-react';
import { DragDropContext, Droppable, Draggable, type DroppableProvided, type DraggableProvided, type DropResult } from '@hello-pangea/dnd';
import { useState } from 'react';
import type { Status } from '../../../components/Types';
import { TaskStatusType, TaskStatusTypeDropdown } from '../../../components/DropdownData/TaskStatusTypeDropdown';

interface Props {
  statuses: Status[];
  onChange: (statuses: Status[]) => void;
  isEditable?: boolean;
}

export const ProjectStatusEditor = ({ statuses, onChange, isEditable = true }: Props) => {
  const [localStatuses, setLocalStatuses] = useState<Status[]>([...statuses]);

  const updateOrder = (newStatuses: Status[]) => {
    const reordered = newStatuses.map((s, index) => ({ ...s, order: index }));
    setLocalStatuses(reordered);
    onChange(reordered);
  };

  const handleDrag = (result: DropResult) => {
    const { source, destination } = result;
    if (!destination) return;
    const updated = [...localStatuses];
    const [removed] = updated.splice(source.index, 1);
    updated.splice(destination.index, 0, removed);
    updateOrder(updated);
  };

  const updateField = (index: number, key: keyof Status, value: string | number) => {
    const updated = [...localStatuses];
    (updated[index] as any)[key] = value;
    setLocalStatuses(updated);
    onChange(updated);
  };

  const addStatus = () => {
    const newStatus: Status = {
      statusId: 0,
      statusName: '',
      typeId: 0,
      typeName: '',
      order: localStatuses.length,
    };
    const updated = [...localStatuses, newStatus];
    updateOrder(updated);
  };

  const removeStatus = (index: number) => {
    const updated = [...localStatuses];
    updated.splice(index, 1);
    updateOrder(updated);
  };

  return (
    <Stack>
      <Group justify="space-between">
        <strong>Status Workflow</strong>
        {isEditable && (
          <Button
            leftSection={<Plus size={16} />}
            variant="light"
            onClick={addStatus}
          >
            Add Status
          </Button>
        )}
      </Group>

      <DragDropContext onDragEnd={handleDrag}>
        <Droppable droppableId="statusList">
          {(provided: DroppableProvided) => (
            <Stack ref={provided.innerRef} {...provided.droppableProps}>
              {localStatuses.map((status, index) => (
                <Draggable key={status.statusId} draggableId={String(status.statusId)} index={index}>
                  {(provided: DraggableProvided) => (
                    <Paper
                      withBorder
                      p="sm"
                      ref={provided.innerRef}
                      {...provided.draggableProps}
                    >
                      <Group align="flex-end">
                        <div {...provided.dragHandleProps}>
                          <GripVertical size={16} />
                        </div>
                        <TextInput
                          label="Status Name"
                          value={status.statusName}
                          disabled={false}
                          onChange={(e) => updateField(index, 'statusName', e.currentTarget.value)}
                        />
                        {isEditable && (
                          <>
                            <TaskStatusTypeDropdown
                              typeId={status.typeId}
                              typeName={status.typeName}
                              onChange={(newTypeId) => {
                                const selected = TaskStatusType.find(t => t.typeId === newTypeId);
                                if (selected) {
                                  updateField(index, 'typeId', selected.typeId);
                                  updateField(index, 'typeName', selected.typeName);
                                }
                              }}
                            />
                            <ActionIcon
                              color="red"
                              onClick={() => removeStatus(index)}
                              mt={22}
                            >
                              <Trash size={16} />
                            </ActionIcon>
                          </>
                        )}
                      </Group>
                    </Paper>
                  )}
                </Draggable>
              ))}
              {provided.placeholder}
            </Stack>
          )}
        </Droppable>
      </DragDropContext>
    </Stack>
  );
};
