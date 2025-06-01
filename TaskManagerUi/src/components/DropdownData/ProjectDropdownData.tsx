import React, { useEffect, useState } from 'react';
import { Select, Fieldset } from '@mantine/core';
import type { Project } from '../Types';
import { useProjectApi } from '../../api/taskManagerApi';

type ProjectDropdownProps = {
  selectedProjectId: string | null;
  onProjectChange: (projectId: string | null) => void;
};

export const ProjectDropdownData: React.FC<ProjectDropdownProps> = ({ selectedProjectId, onProjectChange }) => {
  const { getAllProjects } = useProjectApi();
  const [projects, setProjects] = useState<Project[] | null>(null);

  useEffect(() => {
    const fetchProjects = async () => {
      try {
        const data = await getAllProjects();
        setProjects(data);
      } catch (error) {
        console.error('Error fetching projects:', error);
      }
    };
    fetchProjects();
  }, []);

  return (
    <Select
      placeholder={
        selectedProjectId
          ? "Select project"
          : projects?.find((p: Project) => p.id === selectedProjectId)?.title || "Select project"
      }
      data={
        projects?.map((project) => ({
          value: project.id,
          label: project.title,
        })) || []
      }
      value={selectedProjectId}
      onChange={onProjectChange}
      searchable
      style={{ width: '100%' }}
    />
  );
};