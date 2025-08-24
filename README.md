# TaskManager Solution

## Overview

TaskManager is a modular, enterprise-grade system for managing organizations, projects, tickets, and task histories. Built with .NET 7+/9, React, and OpenAI integration, it consists of several independent services and a modern frontend, all designed for scalability, extensibility, and productivity.

## Solution Structure

- **TaskManagerApi**: Main backend API for organizations, projects, tickets, statuses, and AI integration.
- **TaskManagerConvertor**: Proxy/aggregator service that fetches and combines data from other APIs, keeping services independent.
- **TaskHistoryService**: Microservice for tracking and querying ticket history and audit events.
- **TaskManagerUi**: React + TypeScript frontend for all user interactions, including Kanban boards, backlogs, and AI chat.

## Key Features

- **Organization & Project Management**: CRUD operations, member management, and custom statuses.
- **Ticket Management**: Create, edit, assign, and track tickets with full history and comments.
- **AI Integration**: Create threads with OpenAI assistant, generate tasks from chat, and automate workflows.
- **History & Audit**: Record and query all ticket events and state changes for transparency.
- **Authentication & Authorization**: Secure login via OIDC/JWT, claims-based access control.
- **Logging**: Serilog integration for structured logging across all services.
- **CORS & API Gateway**: Configurable for multiple frontends and secure cross-service communication.
- **Extensible Architecture**: Service interfaces, dependency injection, and modular design for easy extension.

## How It Works

- **TaskManagerApi** exposes RESTful endpoints for all core entities and business logic.
- **TaskManagerConvertor** acts as a proxy, calling other APIs and aggregating responses for clients, ensuring loose coupling and service independence.
- **TaskHistoryService** records every ticket event, providing a complete audit trail.
- **TaskManagerUi** offers a rich, interactive interface for users, including Kanban boards, backlogs, organization/project dashboards, and AI chat.

## Getting Started

1. **Clone the repository**
2. **Configure each service** via its `appsettings.json` (database, endpoints, OpenAI, etc.)
3. **Run EF Core migrations** for each backend service
4. **Start backend services** using `dotnet run` or your IDE
5. **Install frontend dependencies** in `TaskManagerUi` and run with `npm run dev`
6. **Access the UI** at [https://localhost:5173](https://localhost:5173)

## Development & Extensibility

- All services use async/await, DI, and separation of concerns
- DTOs and models use data annotations for validation
- XML documentation for controllers and DTOs
- Modular structure for easy addition of new features/services

## License

This project is proprietary to the owner. Contact the maintainer for licensing details.
