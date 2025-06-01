import { useEffect, useState } from "react";
import { useTaskApi } from "../../api/taskManagerApi"
import type { TaskHistory } from "../Types";
import { Card } from "@mantine/core";

type TaskHistoryComponentProp = {
    taskId: string
}

export const TaskHistoryComponent = (taskId: TaskHistoryComponentProp) => {
    const { getTaskHistory } = useTaskApi();
    const [taskHistories, setTaskHistories] = useState<TaskHistory[]>();

    useEffect(() =>
    {
        const fetchHistory = async () => {
            try {
                const data = await getTaskHistory(taskId.taskId);
                setTaskHistories(data);
            } catch (error) {
                console.log(error)
            }
        }

        fetchHistory();
    },[])

    return (
        <Card>
            {taskHistories?.map((taskHistory) => (
                <div key={taskHistory.id}>
                    {/* Render taskHistory details here */}
                    {JSON.stringify(taskHistory)}
                </div>
                
            ))}
        </Card>
    )
}