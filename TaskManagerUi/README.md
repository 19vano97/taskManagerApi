# React + TypeScript + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh


# TaskManagerUi

## Overview

TaskManagerUi is a modern React + TypeScript web application that serves as the frontend for the TaskManagerApi ecosystem. It provides a rich, interactive interface for managing organizations, projects, tickets, and AI-powered workflows. The UI is designed for productivity, supporting Kanban boards, backlogs, chat-based AI task creation, and secure authentication.

## Main Features

- **Authentication**: Secure login via OpenID Connect (OIDC) and JWT, with support for silent renew and protected routes.
- **Organization Management**: Create, edit, and delete organizations; manage members and settings.
- **Project Management**: Create, edit, and delete projects; manage project statuses and members.
- **Ticket Management**: Create, edit, and view tickets; add comments, track history, and manage additional info.
- **Kanban Board**: Visualize tasks and tickets in columns; drag-and-drop for workflow management.
- **Backlog View**: Prioritize and organize tasks in a backlog.
- **AI Integration**: Chat with an OpenAI assistant, create threads, and generate tasks directly from chat responses.
- **Dropdowns & Data Selection**: Rich dropdowns for accounts, projects, statuses, and types.
- **Responsive Design**: Optimized for desktop and mobile.
- **Custom Theming**: Mantine theme wrapper for consistent UI.
- **Error Handling & Alerts**: Success and error alerts for user feedback.
- **Context & State Management**: React context for project state, hooks for authentication and local storage.

## Solution Structure

- **src/**: Main source code.
  - **api/**: HTTP client and API integrations.
  - **auth/**: Authentication logic and OIDC wrappers.
  - **components/**: Reusable UI components (Header, Sidebar, Footer, Loader, etc.).
  - **pages/**: Main pages (Home, Profile, TaskPage, Organization/Project dashboards).
  - **Kanban/**: Kanban board and task card components.
  - **Backlog/**: Backlog view and related components.
  - **ai/**: AI chat and thread management.
  - **Organization/**: Organization CRUD and settings.
  - **project/**: Project CRUD and settings.
  - **TicketView/**: Ticket details, comments, and history.
  - **DropdownData/**: Data-driven dropdowns for selection.
  - **hooks/**: Custom React hooks for auth, date formatting, etc.
  - **context/**: Project context provider.
  - **wrappers/**: Theme and layout wrappers.
  - **assets/**: Images and SVGs.
  - **styles/**: CSS and module styles.
- **public/**: Static assets and silent-renew page.
- **cert/**: Local development SSL certificates.
- **configs/**: Configuration files.
- **index.html**: Main HTML entry point.
- **package.json**: Project dependencies and scripts.
- **vite.config.ts**: Vite configuration for fast development.

## Key Workflows

- **Login & Auth**: Uses OIDC for secure authentication; private routes protect sensitive pages.
- **Organization & Project CRUD**: Create, edit, and delete organizations/projects; assign members and manage settings.
- **Task & Ticket Management**: Create, edit, and view tickets; add comments and track changes.
- **Kanban & Backlog**: Visualize and manage tasks in Kanban columns or backlog lists.
- **AI Chat**: Interact with OpenAI assistant, create threads, and generate tasks from chat.
- **Dropdowns**: Select accounts, projects, statuses, and types via dynamic dropdowns.

## Configuration

- **Environment Variables**: Configure API endpoints, OIDC settings, and other options in `authConfig.ts` and `package.json`.
- **SSL**: Local development uses certificates in `cert/`.
- **CORS**: Works with backend CORS policies for secure API calls.

## Getting Started

1. **Install dependencies**:  
  `npm install`
2. **Configure environment**:  
  Update API endpoints and OIDC settings in `src/auth/authConfig.ts`.
3. **Run the app**:  
  `npm run dev`
4. **Access the UI**:  
  Open [https://localhost:5173](https://localhost:5173) in your browser.

## Development

- Built with React, TypeScript, and Vite for fast development.
- Uses Mantine for UI theming.
- Modular component structure for easy extension.
- Custom hooks and context for state management.

## Extending the UI

- Add new pages in `src/pages/`.
- Add new components in `src/components/`.
- Integrate new APIs in `src/api/`.
- Update routing and navigation as needed.

## License

This project is proprietary to the owner. Contact the maintainer for licensing details.
