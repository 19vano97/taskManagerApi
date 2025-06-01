import React, { createContext, useContext, useState, useEffect } from 'react';

type ProjectContextType = {
  selectedProjectId: string | null;
  setSelectedProjectId: (id: string | null) => void;
  selectedOrgId: string | null;
  setSelectedOrgId: (id: string | null) => void;
};

const ProjectContext = createContext<ProjectContextType>({
  selectedProjectId: null,
  setSelectedProjectId: () => {},
  selectedOrgId: null,
  setSelectedOrgId: () => {},
});

export const useProject = () => useContext(ProjectContext);

export const ProjectProvider = ({ children }: { children: React.ReactNode }) => {
  const [selectedProjectId, setSelectedProjectIdState] = useState<string | null>(null);
  const [selectedOrgId, setSelectedOrgIdState] = useState<string | null>(null);

  useEffect(() => {
    const savedOrgId = localStorage.getItem('organizationId');
    const savedProjectId = localStorage.getItem('projectId');
    setSelectedOrgIdState(savedOrgId || null);
    setSelectedProjectIdState(savedProjectId || null);
  }, []);

  useEffect(() => {
    if (selectedOrgId) {
      localStorage.setItem('organizationId', selectedOrgId);
    } else {
      localStorage.removeItem('organizationId');
    }
  }, [selectedOrgId]);

  useEffect(() => {
    if (selectedProjectId) {
      localStorage.setItem('projectId', selectedProjectId);
    } else {
      localStorage.removeItem('projectId');
    }
  }, [selectedProjectId]);

  return (
    <ProjectContext.Provider
      value={{
        selectedProjectId,
        setSelectedProjectId: setSelectedProjectIdState,
        selectedOrgId,
        setSelectedOrgId: setSelectedOrgIdState,
      }}
    >
      {children}
    </ProjectContext.Provider>
  );
};
