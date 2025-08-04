import { useEffect, useState } from "react";
import { useOrganizationApi, useTaskApi } from "../../api/taskManagerApi"
import type { AccountDetails, TaskHistory, TaskHistoryType } from "../Types";
import { Card, Timeline, Text } from "@mantine/core";
import { useIdentityServerApi } from "../../api/IdentityServerApi";
import TaskDate from "./TaskDate";
import { BadgePlus, Delete, Pencil } from "lucide-react";

type TaskHistoryComponentProp = {
    taskId: string,
    organizationId: string
}

export const TaskHistoryTypesConst: TaskHistoryType[] = [
    { id: 1, name: "TASK_CREATED" },
    { id: 2, name: "TASK_EDITED_TITLE" },
    { id: 3, name: "TASK_EDITED_DESCRIPTION" },
    { id: 4, name: "TASK_EDITED_REPORTEDID" },
    { id: 5, name: "TASK_EDITED_ASSIGNEEID" },
    { id: 6, name: "TASK_EDITED_PARENTTASK" },
    { id: 7, name: "TASK_EDITED_TASKTYPE" },
    { id: 8, name: "TASK_EDITED_STATUS" },
    { id: 9, name: "TASK_EDITED_PROJECT" },
    { id: 10, name: "TASK_DELETED" },
]

export const TaskHistoryComponent = ({taskId, organizationId}: TaskHistoryComponentProp) => {
    const { getTaskHistory } = useTaskApi();
    const { getOrganizationAccounts } = useOrganizationApi();
    const [ taskHistories, setTaskHistories ] = useState<TaskHistory[]>();
    const [ accounts, setAccounts ] = useState<AccountDetails[]>();

  const getIconTaskHistory = (id: number) => {
      switch (id) {
        case 1:
          return BadgePlus;
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
          return Pencil;
        case 10:
          return Delete;
        default:
          return Pencil;
      }
    };

    useEffect(() =>
    {
        const fetchHistory = async () => {
            try {
                const data = await getTaskHistory(taskId);
                const historyArray = Array.isArray(data.data) ? data.data : [];
                setTaskHistories(historyArray);
            } catch (error) {
                console.log(error)
            }
        }

        fetchHistory();
    },[])

    useEffect(() => {
        if (!taskHistories || taskHistories.length === 0) return;

        const fetchAccounts = async () => {
            try {
                const data = await getOrganizationAccounts(organizationId);
                setAccounts(data.data.accounts || []);
            } catch (error) {
                console.log("TaskHistoryAccounts:", error);
            }
        };

        fetchAccounts();
    }, [taskHistories]);

    return (
        <Timeline active={Array.isArray(taskHistories) ? taskHistories.length : 0} bulletSize={30} lineWidth={2}>
          {Array.isArray(taskHistories) &&
            taskHistories
              .slice()
              .sort((a, b) => new Date(b.createDate).getTime() - new Date(a.createDate).getTime())
              .map((taskHistory) => {
                const iconId = TaskHistoryTypesConst.find(
                  (taskHistoryType: TaskHistoryType) => taskHistoryType.name === taskHistory.eventName
                )?.id ?? 2;
                const IconComponent = getIconTaskHistory(iconId);

                return (
                  <Timeline.Item
                    key={taskHistory.id}
                    bullet={<IconComponent size={20} />}
                    title={
                      taskHistory.eventName +
                      ' By ' +
                      accounts?.find((account: AccountDetails) => account.id === taskHistory.author)?.email
                    }
                    w="100%"
                  >
                    <Text c="dimmed" size="sm">
                      {taskHistory.newState
                        ? `${taskHistory.previousState} -> ${taskHistory.newState}`
                        : ''}
                    </Text>
                    <Text size="xs" mt={4}>
                      <TaskDate dateString={taskHistory.createDate.toString()} />
                    </Text>
                  </Timeline.Item>
                );
              })}
        </Timeline>

    );
  }
