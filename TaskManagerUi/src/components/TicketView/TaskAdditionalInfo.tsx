import { Tabs } from '@mantine/core';
import { IconPhoto, IconMessageCircle, IconSettings } from '@tabler/icons-react';
import { TaskHistoryComponent } from './TaskHistoryComponent';

type TaskAdditionalInfoProps ={
    taskId: string
}

export function TaskAdditionalInfo(taskId: TaskAdditionalInfoProps) {
  return (
    <Tabs color="lime" defaultValue="gallery">
        <Tabs.List>
            <Tabs.Tab value="gallery" leftSection={<IconPhoto size={12} />}>
                Gallery
            </Tabs.Tab>
            <Tabs.Tab value="messages" leftSection={<IconMessageCircle size={12} />}>
                History
            </Tabs.Tab>
            <Tabs.Tab value="settings" leftSection={<IconSettings size={12} />}>
                Settings
            </Tabs.Tab>
        </Tabs.List>

        <Tabs.Panel value="gallery">
            Gallery tab content
        </Tabs.Panel>

        <Tabs.Panel value="messages">
            <TaskHistoryComponent taskId={taskId.taskId} />
        </Tabs.Panel>

        <Tabs.Panel value="settings">
            Settings tab content
        </Tabs.Panel>
        </Tabs>
  );
}