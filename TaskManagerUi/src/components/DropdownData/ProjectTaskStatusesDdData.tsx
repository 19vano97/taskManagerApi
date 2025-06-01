import type React from "react";
import type { Project, Status } from "../Types";
import { Fieldset, Select } from "@mantine/core";

type ProjectTaskStatus = {
    selectedProjectId: string | null;
    taskStatus: Status | null;
    projects: Project[] | null;
    onStatusChange: (status: Status | null) => void;
};

export const ProjectTaskStatusesDdData: React.FC<ProjectTaskStatus> = ({ selectedProjectId, taskStatus, projects, onStatusChange }) => {
    const statuses = projects?.find((project) => project.id === selectedProjectId)
      ?.statuses || [];

  const handleStatusChange = (value: string | null) => {
    const statusId = Number(value);
    const selectedStatus = statuses.find((status) => status.statusId === statusId) || null;
    onStatusChange(selectedStatus);
  };

  return (
    <Select
      placeholder={
        taskStatus
          ? taskStatus.statusName
          : "Select status"
      }
      data={
        statuses.map((status) => ({
          value: String(status.statusId),
          label: status.statusName ?? 'Unknown status',
        }))
      }
      value={taskStatus?.statusId ? String(taskStatus.statusId) : null}
      onChange={handleStatusChange}
      searchable
    />
  );
};