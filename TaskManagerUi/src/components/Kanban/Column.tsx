import { Droppable, type DraggableProvided, type DroppableProvided, type DroppableStateSnapshot } from '@hello-pangea/dnd';
import { Container, Flex, Card, useMantineColorScheme } from '@mantine/core';
import type { AccountDetails, Status, Task } from '../Types';
import { TaskCardOnBoard } from './TaskCardOnBoard';
import { Draggable } from '@hello-pangea/dnd';

type ColumnProps = {
  status: Status;
  tasks: Task[];
  accounts: AccountDetails[];
  onTaskClick: (task: Task) => void;
};

export const Column = ({ status, tasks, accounts, onTaskClick }: ColumnProps) => {
  const { colorScheme } = useMantineColorScheme();
  const isDark = colorScheme === 'dark';

  const background = status.typeId === 1
    ? isDark ? 'linear-gradient(135deg,rgb(91, 101, 111),rgb(46, 65, 87))' : 'linear-gradient(135deg, #f0f4f8, #d9e2ec)'
    : status.typeId === 2
      ? isDark ? 'linear-gradient(135deg,rgb(103, 109, 116),rgb(125, 114, 171))' : 'linear-gradient(135deg, #ddd6fe, #ede9fe)'
      : status.typeId === 3
        ? isDark ? 'linear-gradient(135deg,rgb(35, 185, 88),rgb(60, 93, 71))' : 'linear-gradient(135deg, #bbf7d0, #dcfce7)'
        : 'linear-gradient(135deg, #f9fafb, #f3f4f6)';

  return (
    <Container>
      <Flex direction="column" gap="sm" miw={"300px"}>
        <Card shadow="sm" padding="lg" radius="md" withBorder style={{ background }}>
          <h2>{status.statusName}</h2>
          <Droppable droppableId={String(status.statusId)}>
            {(provided: DroppableProvided, snapshot: DroppableStateSnapshot) => (
              <div
                ref={provided.innerRef}
                {...provided.droppableProps}
                style={{
                  minHeight: 100,
                  transition: 'background-color 0.2s ease',
                  backgroundColor: snapshot.isDraggingOver ? '#f1f5f9' : 'transparent',
                  borderRadius: 8,
                  padding: 4,
                }}
              >
                <Flex direction="column" gap="sm" style={{
                  minWidth: 300,
                  maxWidth: 300,
                  flex: '0 0 300px',
                }}>
                  {tasks.map((task, index) => (
                    <Draggable
                      draggableId={String(task.id)}
                      index={index}
                      key={task.id}
                    >
                      {(provided: DraggableProvided) => (
                        <div
                          ref={provided.innerRef}
                          {...provided.draggableProps}
                          {...provided.dragHandleProps}
                        >
                          {task.reporterId && task.assigneeId
                            ? (
                              <TaskCardOnBoard
                                task={task}
                                reporter={accounts.find(account => account.id === task.reporterId)!}
                                assignee={accounts.find(account => account.id === task.assigneeId)!}
                                onClick={() => onTaskClick(task)}
                              />
                            )
                            : null
                          }
                        </div>
                      )}
                    </Draggable>
                  ))}
                  {provided.placeholder}
                </Flex>
              </div>
            )}
          </Droppable>
        </Card>
      </Flex>
    </Container>
  );
};
