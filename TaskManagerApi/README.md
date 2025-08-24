# TaskManagerApi

## Overview

TaskManagerApi is a modular .NET 9 Web API solution for managing organizations, projects, tickets, and task histories. It is designed for enterprise use, supporting multi-organization workflows, robust authentication, and extensible business logic. The solution is split into several services, each responsible for a distinct domain area.


## Main Features

- **Organization Management**: Create, edit, and manage organizations and their members.
- **Project Management**: Create, edit, and delete projects within organizations. Assign accounts to projects and manage project-specific statuses.
- **Ticket Management**: CRUD operations for tickets, including assignment, status tracking, and history.
- **Task History**: Track changes and events for tickets, providing audit trails.
- **Status Management**: Customizable status flows for projects and tickets.
- **Authentication & Authorization**: JWT-based authentication, role-based access, and account verification.
- **Logging**: Integrated with Serilog for structured logging to console and file.
- **OpenAPI/Swagger**: Auto-generated API documentation and UI for testing endpoints.
- **CORS Support**: Configurable for multiple frontends (e.g., React, Convertor).
- **Extensible Architecture**: Service interfaces and dependency injection for easy extension and testing.
- **AI Integration**: The API integrates with OpenAI to provide smart assistant features. Users can create AI threads (conversations) with an OpenAI assistant. The assistant can analyze chat context and generate actionable tasks directly from the conversation. This enables automated ticket creation and project management based on natural language input and AI recommendations.

## Solution Structure

- **TaskManagerApi**: Main API for organizations, projects, tickets, and statuses.
- **TaskHistoryService**: Handles ticket history and audit events.
- **TaskManagerConvertor**: Auxiliary service for data conversion and integration.
- **Models**: DTOs for all major entities (Organization, Project, Ticket, Status, etc.).
- **Controllers**: RESTful endpoints for each domain (Organization, Project, Ticket, Ai).
- **Services**: Business logic, validation, and data access (via EF Core).
- **Middlewares**: Exception handling and request logging.
- **Attributes**: Custom attributes for request validation and organization context.
- **Migrations**: EF Core migrations for database schema evolution.

## Key Endpoints

- `/api/organization` - Organization CRUD and member management.
- `/api/project` - Project CRUD, status management, and account assignment.
- `/api/task` - Ticket CRUD, assignment, status changes, and history.
- `/api/task/all/{organizationId}/organization` - Get all tickets for an organization.
- `/api/project/all/{organizationId}` - Get all projects and their tasks for an organization.

## Authentication

- Uses JWT Bearer authentication.
- Account verification is enforced for sensitive operations.
- Claims-based authorization for organization and project access.

## Logging

- Configured via Serilog in `appsettings.json`.
- Logs to both console and rolling file (`logs/serilog-.log`).

## Configuration

- All settings (connection strings, API endpoints, OpenAI integration, etc.) are managed via `appsettings.json`.
- CORS policies for React and Convertor frontends.

## Getting Started

1. **Clone the repository**
2. **Configure your database connection** in `appsettings.json`.
3. **Run EF Core migrations** to set up the schema.
4. **Start the API** using `dotnet run` or your preferred IDE.
5. **Access Swagger UI** at `/swagger` for interactive API documentation.

## Development

- Follows best practices for async/await, dependency injection, and separation of concerns.
- DTOs use data annotations for validation.
- XML documentation is present for controllers and DTOs.
- All business logic is encapsulated in service classes.

## Extending the API

- Add new services by implementing the appropriate interface in `Services/Interfaces`.
- Register new services in `Program.cs` using `AddScoped`.
- Add new endpoints in the relevant controller.

## License

This project is proprietary to the owner. Contact the maintainer for licensing details.
