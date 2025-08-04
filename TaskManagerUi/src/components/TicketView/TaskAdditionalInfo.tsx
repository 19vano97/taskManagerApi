import { Tabs } from '@mantine/core';
import { TaskHistoryComponent } from './TaskHistoryComponent';
import { History, MessageCircle } from 'lucide-react';

type TaskAdditionalInfoProps ={
    taskId: string
    organizationId: string
}

export function TaskAdditionalInfo({taskId, organizationId}: TaskAdditionalInfoProps) {
  return (
        <Tabs color="blue" defaultValue="comments">
            <Tabs.List>
                <Tabs.Tab value="gallery" leftSection={<MessageCircle size={20} />}>
                    Comments
                </Tabs.Tab>
                <Tabs.Tab value="messages" leftSection={<History size={20} />}>
                    History
                </Tabs.Tab>
            </Tabs.List>

            <Tabs.Panel value="comments" pt="20px">
                Comments
            </Tabs.Panel>

            <Tabs.Panel value="messages" pt="20px">
                <TaskHistoryComponent taskId={taskId} organizationId={organizationId}/>
            </Tabs.Panel>

        </Tabs>
  );
}