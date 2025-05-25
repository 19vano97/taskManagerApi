// import { Outlet } from 'react-router-dom'
// import { Header } from './Header'
// import { Sidebar } from './Sidebar'
// import { Footer } from './Footer'
// import '../styles/MainLayout.css'

// export const MainLayout = () => {
//   return (
//     <div className="layout">

//       <div className="body">
//         <Sidebar />
//         <main className="content">
//           <Outlet />
//         </main>
//       </div>
//       <Footer />
//     </div>
//   )
// }

import { AppShell, Burger, Container } from '@mantine/core'
import { Outlet } from 'react-router-dom'
import { Header } from '../components/Header'
import { Sidebar } from '../components/Sidebar'
import { Footer } from '../components/Footer'
import { useDisclosure } from '@mantine/hooks'

export const MainLayout = () => {
  return (
    <AppShell
      padding="md"
      header={{ height: 60 }}
      navbar={{
        width: 200,
        breakpoint: 'sm',
        collapsed: { mobile: false },
      }}
      footer={{
        height: 60
      }}
      styles={{
        main: { backgroundColor: '#f8f9fa' },
      }}
    >
      <AppShell.Header>
        {/* <Burger
          opened={opened}
          onClick={toggle}
          hiddenFrom="sm"
          size="sm"
        /> */}
        <Header />
      </AppShell.Header>
      <AppShell.Navbar p="xs">
        <Sidebar />
      </AppShell.Navbar>
      <AppShell.Footer>
        <Footer />
      </AppShell.Footer>
      <AppShell.Main>
        <Container size={'lg'} p="md">
          <Outlet />
        </Container>
      </AppShell.Main>
    </AppShell>
  )
}