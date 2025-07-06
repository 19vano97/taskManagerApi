import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import PrivateRoute from './auth/PrivateRoute'
import { MainLayout } from './components/MainLayout'
import { lazy, Suspense } from 'react'
import { LoaderMain } from './components/LoaderMain'
import '@mantine/core/styles.css'
import '@mantine/tiptap/styles.css';
import NotFoundPage from './pages/NotFoundPage'
import { ChatAi } from './pages/ai/ChatAi'

const HomeAuthorized = lazy(() => import('./pages/HomeAuthorized'))
const Home = lazy(() => import('./pages/Home'))
const Kanban = lazy(() => import('./pages/projects/Kanban'))
const Profile = lazy(() => import('./pages/Profile'))
const Backlog = lazy(() => import('./pages/projects/Backlog'))
const OrganizationAllPage = lazy(() => import('./pages/organizations/OrganizationAllPage'))
const OrganizationPage = lazy(() => import('./pages/organizations/OrganizationDashboard'))
const OrganizationMembersPage = lazy(() => import('./pages/organizations/OrganizationMembersPage'))
const OrganizationSettingsPage = lazy(() => import('./pages/organizations/OrganizationSettingsPage'))
const ProjectPage = lazy(() => import('./pages/projects/ProjectPage'))
const ProjectSettingsPage = lazy(() => import('./pages/projects/ProjectSettings'))
const TaskPage = lazy(() => import('./pages/TaskPage'))
const SignInOidcHandler = lazy(() => import('./auth/SignInOidcHandler'))

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
            <Suspense fallback={<LoaderMain />}>
              <HomeAuthorized />
            </Suspense>
          }
        />
        <Route
          // path="project/:id/kanban"
          path="project/:id/kanban"
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
          path="project/:id/backlog"
          element={
            <Suspense fallback={<LoaderMain />}>
              <Backlog />
            </Suspense>
          }
        />
        <Route
          path="project/:id/chatai"
          element={
            <Suspense fallback={<LoaderMain />}>
              <ChatAi />
            </Suspense>
          }
        />
        <Route
          path="organizations"
          element={
            <Suspense fallback={<LoaderMain />}>
              <OrganizationAllPage />
            </Suspense>
          }
        />
        <Route
          path="task/:id"
          element={
            <Suspense fallback={<LoaderMain />}>
              <TaskPage />
            </Suspense>
          }
        />
        <Route
          path="project/:id"
          element={
            <Suspense fallback={<LoaderMain />}>
              <ProjectPage />
            </Suspense>
          }
        />
        <Route
          path="project/:id/settings"
          element={
            <Suspense fallback={<LoaderMain />}>
              <ProjectSettingsPage />
            </Suspense>
          }
        />
        <Route
          path="org/:id"
          element={
            <Suspense fallback={<LoaderMain />}>
              <OrganizationPage />
            </Suspense>
          }
        />
         <Route
          path="org/:id/members"
          element={
            <Suspense fallback={<LoaderMain />}>
              <OrganizationMembersPage />
            </Suspense>
          }
        />
         <Route
          path="org/:id/settings"
          element={
            <Suspense fallback={<LoaderMain />}>
              <OrganizationSettingsPage />
            </Suspense>
          }
        />
        <Route path="*" element={<NotFoundPage />} />
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
  );
}


export default App
