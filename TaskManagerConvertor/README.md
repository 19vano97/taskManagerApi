# TaskManagerConvertor

## Overview

TaskManagerConvertor is a .NET 9 Web API microservice designed to facilitate data conversion, integration, and orchestration between various task management domains. It acts as a proxy or aggregator, calling other APIs to fetch information and combining their responses into a single unified result. This approach helps keep services independent, allowing each service to evolve separately while TaskManagerConvertor coordinates and aggregates data for clients. It provides endpoints for authentication, organization, project, ticket, and AI-powered operations.

## How It Works

1. **Organization Creation & Membership**: Users must first create or join an organization. Organizations are the top-level entities, and users can easily invite others to join and switch between multiple organizations. This is ideal for freelancers or users who participate in several teams.

2. **Projects Within Organizations**: Each organization can have multiple projects. Projects are isolated within their organization and have their own custom statuses and ticket workflows.

3. **Ticket Management**: Tickets are always linked to both an organization and a project. They cannot exist independently. Tickets can be connected as parent/child, allowing for hierarchical task management and complex workflows.

4. **Multi-Organization Support**: Users can be members of several organizations, making it easy to manage work across different teams or clients from a single account.

5. **AI-Powered Project & Ticket Creation**: The integrated AI assistant helps users create projects and tickets based on their needs. When a user initiates ticket creation from a conversation, the AI generates all relevant tickets, connects them as needed, and ensures they are properly linked within the project and organization.

This system streamlines collaboration, project management, and task tracking for individuals and teams, supporting both single-organization and multi-organization scenarios with powerful AI automation.

## Main Features

- **Authentication**: Endpoints for user authentication and token management.
- **Organization Management**: Create, edit, and manage organizations and their members.
- **Project Management**: CRUD operations for projects, including account assignment and status management.
- **Ticket Management**: Create, edit, and manage tickets, including AI-generated tasks and ticket histories.
- **AI Integration**: Create threads with OpenAI assistant, analyze chat context, and generate actionable tasks from AI responses.
- **Data Conversion**: Converts and adapts data formats between external sources and the main API.
- **Logging**: Integrated with Serilog for structured logging to file.
- **Exception Handling**: Centralized middleware for error handling and logging.
- **CORS Support**: Configurable for secure cross-origin requests.

## Solution Structure

- **Controllers**: RESTful endpoints for Auth, Organization, Project, Ticket, and AI operations.
- **Models**: DTOs for all major entities (Account, Organization, Project, Ticket, Status, AI).
- **Services**: Business logic, validation, and data access.
- **Providers**: Header propagation and context management.
- **Middlewares**: Exception handling and request logging.
- **Logs**: Rolling log files for audit and debugging.
- **Configuration**: Settings for API endpoints, connection strings, and OpenAI integration.

## Key Endpoints

- `/api/auth` - Authentication and token management.
- `/api/organization` - Organization CRUD and member management.
- `/api/project` - Project CRUD, status management, and account assignment.
- `/api/ticket` - Ticket CRUD, assignment, status changes, and history.
- `/api/ai` - AI thread creation, chat interaction, and task generation.

## Authentication

- Uses JWT Bearer authentication.
- Account verification is enforced for sensitive operations.
- Claims-based authorization for organization and project access.

## Logging

- Configured via Serilog in `appsettings.json`.
- Logs to rolling files in the `logs/` directory.

## Configuration

- All settings (connection strings, API endpoints, OpenAI integration, etc.) are managed via `appsettings.json`.
- CORS policies for secure frontend integration.

## Getting Started

1. **Clone the repository**
2. **Configure your database connection and API endpoints** in `appsettings.json`.
3. **Run EF Core migrations** if needed.
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
