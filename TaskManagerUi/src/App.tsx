import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import PrivateRoute from './auth/PrivateRoute'
import { MainLayout } from './components/MainLayout'
import { lazy, Suspense } from 'react'
import { LoaderMain } from './components/LoaderMain'
import '@mantine/core/styles.css'
import '@mantine/tiptap/styles.css';

const Home = lazy(() => import('./pages/Home'))
const Kanban = lazy(() => import('./pages/Kanban'))
const Profile = lazy(() => import('./pages/Profile'))
const Backlog = lazy(() => import('./pages/Backlog'))
const SignInOidcHandler = lazy(() => import('./auth/SignInOidcHandler'))

function App() {
  return (
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
              <Suspense fallback={<LoaderMain />}>
                <Kanban />
              </Suspense>
            }
          />
          <Route
            path="profile"
            element={
              <Suspense fallback={<LoaderMain />}>
                <Profile />
              </Suspense>
            }
          />
          <Route
            path="backlog"
            element={
              <Suspense fallback={<LoaderMain />}>
                <Backlog />
              </Suspense>
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
  );
}


export default App
