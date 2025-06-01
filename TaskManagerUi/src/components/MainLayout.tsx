import { AppShell } from '@mantine/core'
import { Outlet } from 'react-router-dom'
import { Header } from '../components/Header'
import { Sidebar } from '../components/Sidebar'
import { Footer } from '../components/Footer'
import { useDisclosure } from '@mantine/hooks'
import { LoaderMain } from './LoaderMain'
import { useSafeAuth } from '../hooks/useSafeAuth'

export const MainLayout = () => {
  const [opened, { toggle }] = useDisclosure(false);
  const auth = useSafeAuth();

  if (auth.isLoading) return <LoaderMain />;

  if (!auth.isAuthenticated) {
    auth.signinRedirect();
    return <LoaderMain />; 
  }
  
  return (
    <AppShell
      header={{ height: 60 }}
      footer={{ height: 60 }}
      navbar={{ width: 250, breakpoint: 'sm', collapsed: { desktop: !opened, mobile: !opened } }}
      padding="md"
      styles={{
        root: {
          minHeight: '100vh',

        },
        main: {
          
          width: '100%',
          overflowX: 'auto',
        },
      }}
    >
      <AppShell.Header>
        <Header opened={opened} toggle={toggle}/>
      </AppShell.Header>

      <AppShell.Navbar p="md">
        <Sidebar />
      </AppShell.Navbar>

      <AppShell.Footer>
        <Footer />
      </AppShell.Footer>

      <AppShell.Main>
        <Outlet />
      </AppShell.Main>
    </AppShell>
  )
}
