// import { Card, Container, Flex } from '@mantine/core';
// import type { Status, Task } from '../Types';
// import { TaskCardOnBoard } from './Task';

// type ColumnProps = {
//     status: Status;
//     tasks: Task[];
//     onTaskClick: (task: Task) => void;
// };

// export const Column = ({ status, tasks, onTaskClick }: ColumnProps) => {
//     const background = status.typeId === 2
//         ? 'linear-gradient(135deg, #f0f4f8, #d9e2ec)'
//         : status.typeId === 3
//             ? 'linear-gradient(135deg, #ede9fe, #ddd6fe)'
//         : status.typeId === 4
//             ? 'linear-gradient(135deg, #dcfce7, #bbf7d0)'
//         : 'linear-gradient(135deg, #f9fafb, #f3f4f6)';

//     return (
//         <Container>
//             <Flex direction="column" gap="md" style={{ minWidth: '300px' }}>
//                 <Card shadow="sm" 
//                     padding="lg" 
//                     radius="md" 
//                     withBorder
//                     style={{ background }}> 
//                     <h2>{status.statusName}</h2>
//                     <Flex direction="column" gap="md">
//                         {tasks.map((task) => (
//                             <TaskCardOnBoard key={task.id} task={task} onClick={() => onTaskClick(task)} />
//                         ))}
//                     </Flex>
//                 </Card>
//             </Flex>
//         </Container>
//     );
// };

import { Droppable } from '@hello-pangea/dnd';
import { Container, Flex, Card } from '@mantine/core';
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
  const background = status.typeId === 2
    ? 'linear-gradient(135deg, #f0f4f8, #d9e2ec)'
    : status.typeId === 3
    ? 'linear-gradient(135deg, #ede9fe, #ddd6fe)'
    : status.typeId === 4
    ? 'linear-gradient(135deg, #dcfce7, #bbf7d0)'
    : 'linear-gradient(135deg, #f9fafb, #f3f4f6)';

  return (
    <Container>
      <Flex direction="column" gap="md" style={{ minWidth: '300px' }}>
        <Card shadow="sm" padding="lg" radius="md" withBorder style={{ background }}>
          <h2>{status.statusName}</h2>
          <Droppable droppableId={String(status.statusId)}>
            {(provided: import('@hello-pangea/dnd').DroppableProvided) => (
              <div ref={provided.innerRef} {...provided.droppableProps}>
                <Flex direction="column" gap="md">
                  {tasks.map((task, index) => (
                    <Draggable
                      draggableId={String(task.id)}
                      index={index}
                      key={task.id}
                    >
                      {(provided: import('@hello-pangea/dnd').DraggableProvided) => (
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
