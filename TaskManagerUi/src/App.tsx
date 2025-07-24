import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { MainLayout } from './components/MainLayout'
import { lazy, Suspense } from 'react'
import { LoaderMain } from './components/LoaderMain'
import '@mantine/core/styles.css'
import '@mantine/tiptap/styles.css';
import NotFoundPage from './pages/NotFoundPage'
import { ChatAi } from './pages/ai/ChatAi'
import { PrivateRoute } from './auth/PrivateRoute'
import Callback from './auth/Callback'

const HomeAuthorized = lazy(() => import('./pages/HomeAuthorized'))
const Home = lazy(() => import('./pages/Home'))
const Kanban = lazy(() => import('./pages/projects/Kanban'))
const Profile = lazy(() => import('./pages/Profile'))
const Backlog = lazy(() => import('./pages/projects/Backlog'))
const OrganizationAllPage = lazy(() => import('./pages/organizations/OrganizationAllPage'))
const OrganizationPage = lazy(() => import('./pages/organizations/OrganizationDashboard'))
const OrganizationSettingsPage = lazy(() => import('./pages/organizations/OrganizationSettingsPage'))
const ProjectPage = lazy(() => import('./pages/projects/ProjectPage'))
const ProjectSettingsPage = lazy(() => import('./pages/projects/ProjectSettings'))
const TaskPage = lazy(() => import('./pages/TaskPage'))

function App() {
  return (
    <Routes>
      <Route
        index
        element={
          <Suspense fallback={<LoaderMain />}>
            <Home />
          </Suspense>
        }
      />
      <Route path="/" element={<MainLayout />}>
        <Route
          path='/me'
          element={
            <PrivateRoute>
              <Suspense fallback={<LoaderMain />}>
                <HomeAuthorized />
              </Suspense>
            </PrivateRoute>
          }
        />
        <Route
          // path="project/:id/kanban"
          path="project/:id/kanban"
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
          path="project/:id/backlog"
          element={
            <PrivateRoute>
            <Suspense fallback={<LoaderMain />}>
              <Backlog />
            </Suspense>
            </PrivateRoute>
          }
        />
        <Route
          path="project/:id/chatai"
          element={
            <PrivateRoute>
            <Suspense fallback={<LoaderMain />}>
              <ChatAi />
            </Suspense>
            </PrivateRoute>
          }
        />
        <Route
          path="organizations"
          element={
            <PrivateRoute>
            <Suspense fallback={<LoaderMain />}>
              <OrganizationAllPage />
            </Suspense>
            </PrivateRoute>
          }
        />
        <Route
          path="task/:id"
          element={
            <PrivateRoute>
            <Suspense fallback={<LoaderMain />}>
              <TaskPage />
            </Suspense>
            </PrivateRoute>
          }
        />
        <Route
          path="project/:id"
          element={
            <PrivateRoute>
            <Suspense fallback={<LoaderMain />}>
              <ProjectPage />
            </Suspense>
            </PrivateRoute>
          }
        />
        <Route
          path="project/:id/settings"
          element={
            <PrivateRoute>
            <Suspense fallback={<LoaderMain />}>
              <ProjectSettingsPage />
            </Suspense>
            </PrivateRoute>
          }
        />
        <Route
          path="org/:id"
          element={
            <PrivateRoute>
            <Suspense fallback={<LoaderMain />}>
              <OrganizationPage />
            </Suspense>
            </PrivateRoute>
          }
        />
        <Route
          path="org/:id/settings"
          element={
            <PrivateRoute>
            <Suspense fallback={<LoaderMain />}>
              <OrganizationSettingsPage />
            </Suspense>
            </PrivateRoute>
          }
        />
        <Route path="*" element={<NotFoundPage />} />
        <Route
          path="signin-oidc"
          element={
            <Suspense fallback={<LoaderMain />}>
              <Callback />
            </Suspense>
          }
        />
      </Route>
    </Routes>
  );
}


export default App
