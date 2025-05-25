import { Group, Text, Container, Flex, Image, Input, InputBase, Combobox, useCombobox, Select } from '@mantine/core'
import { LoaderMain } from "./LoaderMain";
import { useAuth } from 'react-oidc-context';
import { useEffect, useState } from 'react';
import { useOrganizationApi } from '../api/taskManagerApi';

export function Header() {
  const auth  = useAuth();

  const { getOrganizationProjects } = useOrganizationApi()
  const [orgOptions, setOrgOptions] = useState<
  { value: string; label: string; projects?: { id: string; title: string }[] }[]
  >([]);
  const [selectedOrgId, setSelectedOrgId] = useState<string | null>(null)

  const [projectOptions, setProjectOptions] = useState<{ value: string; label: string }[]>([])
  const [selectedProjectId, setSelectedProjectId] = useState<string | null>(null)

  useEffect(() => {
    if (!auth.isAuthenticated) return

    const loadOrgs = async () => {
      const data = await getOrganizationProjects()
      const mapped = data.map((org: any) => ({
        value: org.id,
        label: org.name,
        projects: org.projects,
      }))
      setOrgOptions(mapped)
    }

    loadOrgs()
  }, [auth.isAuthenticated])

  const handleOrgChange = (orgId: string | null) => {
    setSelectedOrgId(orgId)
    setSelectedProjectId(null)

    const selectedOrg = orgOptions.find(org => org.value === orgId)

    if (selectedOrg && selectedOrg.projects) {
      const projects = selectedOrg.projects.map((p: any) => ({
        value: p.id,
        label: p.title,
      }))
      setProjectOptions(projects)
      localStorage.setItem('organizationId', orgId || '')
    } else {
      setProjectOptions([])
    }
  }

  const handleProjectChange = (projectId: string | null) => {
    setSelectedProjectId(projectId)
    console.log('Selected Project ID:', projectId)
  }


  return ( 
      <Container fluid p="xs" style={{ display: 'flex', justifyContent: 'space-between', gap: '10px' }}>
        <Flex align="center" gap="xs">
          <Image src="../assets/react.svg" alt="Logo" />
        </Flex>
        <Flex justify="flex-start" align="flex-start" gap="xs">
          {auth.isLoading ? (
            <LoaderMain />
          ) : auth.isAuthenticated ? (
            <Flex  gap="xs" align="center">
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
              value={selectedProjectId}
              onChange={handleProjectChange}
              searchable
              disabled={!selectedOrgId}
            />
              <Text size="md">{auth.user?.profile.email}</Text>
              <button onClick={() => auth.signoutRedirect()}>Logout</button>
            </Flex>
          ) : (
            <button onClick={() => auth.signinRedirect()}>Login</button>
          )
          }
        </Flex>
      </Container>
    );
  
}