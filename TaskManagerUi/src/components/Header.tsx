import { Text, Container, Flex, Image, Select, Button, Burger } from '@mantine/core';
import { LoaderMain } from './LoaderMain';
import { useEffect, useState } from 'react';
import { useOrganizationApi } from '../api/taskManagerApi';
import { useSafeAuth } from '../hooks/useSafeAuth';

export function Header({ opened, toggle }: { opened: boolean; toggle: () => void }) {
  const auth = useSafeAuth();
  const { getOrganizationProjects } = useOrganizationApi();

  const [orgOptions, setOrgOptions] = useState<
    { value: string; label: string; projects?: { id: string; title: string }[] }[]
  >([]);
  const [projectOptions, setProjectOptions] = useState<{ value: string; label: string }[]>([]);
  const [selectedOrgId, setSelectedOrgId] = useState<string | null>(null);
  const [selectedProject, setSelectedProject] = useState<string | null>(null);

  useEffect(() => {
    const savedOrgId = localStorage.getItem('organizationId');
    const savedProjectId = localStorage.getItem('projectId');

    setSelectedOrgId(savedOrgId || null);
    setSelectedProject(savedProjectId || null);
  }, []);

  useEffect(() => {
    if (!auth || !auth.isAuthenticated) return; // Fallback check for auth

    const loadOrgs = async () => {
      try {
        const data = await getOrganizationProjects();
        const mappedOrgs = data.map((org: any) => ({
          value: org.id,
          label: org.name,
          projects: org.projects,
        }));
        setOrgOptions(mappedOrgs);

        const savedOrgId = localStorage.getItem('organizationId');
        if (savedOrgId) {
          const selectedOrg = mappedOrgs.find((org: { value: string; label: string; projects?: { id: string; title: string }[] }) => org.value === savedOrgId);
          if (selectedOrg && selectedOrg.projects) {
            const mappedProjects = selectedOrg.projects.map((p: any) => ({
              value: p.id,
              label: p.title,
            }));
            setProjectOptions(mappedProjects);

            const savedProjectId = localStorage.getItem('projectId');
            if (savedProjectId) {
              const selectedProject = mappedProjects.find((p: { value: string; label: string }) => p.value === savedProjectId);
              if (selectedProject) {
                setSelectedProject(savedProjectId);
              }
            }
          }
        }
      } catch (error) {
        console.error('Error fetching organizations:', error);
      }
    };

    loadOrgs();
  }, [auth?.isAuthenticated]); // Ensure auth is defined before accessing its properties

  const handleOrgChange = (orgId: string | null) => {
    setSelectedOrgId(orgId);
    setSelectedProject(null);

    const selectedOrg = orgOptions.find((org) => org.value === orgId);
    if (selectedOrg && selectedOrg.projects) {
      const mappedProjects = selectedOrg.projects.map((p: any) => ({
        value: p.id,
        label: p.title,
      }));
      setProjectOptions(mappedProjects);
      localStorage.setItem('organizationId', orgId || '');
      localStorage.setItem('projectId', '');
    } else {
      setProjectOptions([]);
    }
  };

  const handleProjectChange = (projectId: string | null) => {
    setSelectedProject(projectId);
    if (projectId) {
      localStorage.setItem('projectId', projectId);

      const kanbanEvent = new CustomEvent('updateKanban', { detail: { projectId } });
      window.dispatchEvent(kanbanEvent);
    } else {
      localStorage.removeItem('projectId');
    }
  };

  return (
    <Container fluid p="xs" style={{ display: 'flex', justifyContent: 'space-between', gap: '10px' }}>
      <Flex align="center" gap="xs">
        <Image src="/logo.svg" alt="Logo" w={40} h={40} />
        <Text
          size="lg"
          w={600}
          style={{ fontFamily: 'Segoe UI, sans-serif', fontSize: 28, color: '#111', fontWeight: 600 }}
        >
          TaskType
        </Text>
        <Burger size="sm" color="#111" opened={opened} onClick={toggle} />
      </Flex>
      <Flex justify="flex-start" align="center" gap="xs">
        {auth?.isLoading ? (
          <LoaderMain />
        ) : auth?.isAuthenticated ? (
          <Flex gap="xs" align="center">
            <Select
              placeholder="Select Organization"
              data={orgOptions}
              value={selectedOrgId}
              onChange={handleOrgChange}
              searchable
            />
            <Select
              placeholder="Select Project"
              data={projectOptions}
              value={selectedProject}
              onChange={handleProjectChange}
              searchable
              disabled={!selectedOrgId}
            />
            <Text size="md">{auth.user?.profile.email}</Text>
            <Button variant="outline" onClick={() => auth.signoutRedirect()}>
              Logout
            </Button>
          </Flex>
        ) : (
          <Button onClick={() => auth.signinRedirect()}>Login</Button>
        )}
      </Flex>
    </Container>
  );
}
