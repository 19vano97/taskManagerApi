// import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
// import '@mantine/core/styles.css';
// import { MantineProvider } from '@mantine/core';
// import { AuthProvider, useAuth } from 'react-oidc-context';
// import { oidcConfig } from './auth/authConfig'
// import SignInOidcHandler from './auth/SignInOidcHandler'
// import PrivateRoute from './auth/PrivateRoute'
// import { MainLayout } from './components/MainLayout'
// import Home from './pages/Home'
// import { Kanban } from './pages/Kanban'
// import Profile from './pages/Profile'
// import Backlog from './pages/Backlog'

// function App() {
//   return (
//     <MantineProvider>
//       <AuthProvider {...oidcConfig}>
//         <Router>
//           <Routes>
//             <Route path="/" element={<MainLayout />}>
//               <Route index element={<Home />} />
//               <Route path="/kanban" element={<PrivateRoute><Kanban /></PrivateRoute>} />
//               <Route path="profile" element={<Profile />} />
//               <Route path="/backlog" element={<Backlog />} />
//               <Route path="/signin-oidc" element={<SignInOidcHandler />} />
//             </Route>
//           </Routes>
//         </Router>
//       </AuthProvider>
//     </MantineProvider>
//   )
// }

// export default App

import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import '@mantine/core/styles.css'
import { MantineProvider } from '@mantine/core'
import { AuthProvider } from 'react-oidc-context'
import { oidcConfig } from './auth/authConfig'
import PrivateRoute from './auth/PrivateRoute'
import { MainLayout } from './components/MainLayout'
import { lazy, Suspense } from 'react'
import { LoaderMain } from './components/LoaderMain'

// 👇 Ленивые страницы
const Home = lazy(() => import('./pages/Home'))
const Kanban = lazy(() => import('./pages/Kanban'))
const Profile = lazy(() => import('./pages/Profile'))
const Backlog = lazy(() => import('./pages/Backlog'))
const SignInOidcHandler = lazy(() => import('./auth/SignInOidcHandler'))

function App() {
  return (
    <MantineProvider>
      <AuthProvider {...oidcConfig}>
        <Router>
          <Routes>
            <Route path="/" element={<MainLayout />}>
              <Route
                index
                element={
                  <Suspense fallback={<LoaderMain />}>
                    <Home />
                  </Suspense>
                }
              />
              <Route
                path="kanban"
                element={
                  <PrivateRoute>
                    <Suspense fallback={<LoaderMain />}>
                      <Kanban />
                    </Suspense>
                  </PrivateRoute>
                }
              />
              <Route
                path="profile"
                element={
                  <PrivateRoute>
                    <Suspense fallback={<LoaderMain />}>
                      <Profile />
                    </Suspense>
                  </PrivateRoute>
                }
              />
              <Route
                path="backlog"
                element={
                  <PrivateRoute>
                    <Suspense fallback={<LoaderMain />}>
                      <Backlog />
                    </Suspense>
                  </PrivateRoute>
                }
              />
              <Route
                path="signin-oidc"
                element={
                  <Suspense fallback={<LoaderMain />}>
                    <SignInOidcHandler />
                  </Suspense>
                }
              />
            </Route>
          </Routes>
        </Router>
      </AuthProvider>
    </MantineProvider>
  )
}

export default App
