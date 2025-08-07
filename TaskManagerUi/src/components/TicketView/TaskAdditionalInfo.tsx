import { Tabs } from '@mantine/core';
import { TaskHistoryComponent } from './TaskHistoryComponent';
import { History, MessageCircle } from 'lucide-react';
import TicketComment from './Comments/TicketComment';

type TaskAdditionalInfoProps ={
    taskId: string
    organizationId: string
}

export function TaskAdditionalInfo({taskId, organizationId}: TaskAdditionalInfoProps) {
  return (
        <Tabs color="blue" defaultValue="comments" w={'100%'}>
            <Tabs.List>
                <Tabs.Tab value="comments" leftSection={<MessageCircle size={20} />}>
                    Comments
                </Tabs.Tab>
                <Tabs.Tab value="messages" leftSection={<History size={20} />}>
                    History
                </Tabs.Tab>
            </Tabs.List>

            <Tabs.Panel value="comments" pt="20px">
                <TicketComment taskId={taskId}/>
            </Tabs.Panel>

            <Tabs.Panel value="messages" pt="20px">
                <TaskHistoryComponent taskId={taskId} organizationId={organizationId}/>
            </Tabs.Panel>

        </Tabs>
  );
}