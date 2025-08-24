# TaskHistoryService

## Overview

TaskHistoryService is a .NET 9 Web API microservice dedicated to tracking and managing the history of tasks (tickets) within the TaskManager ecosystem. It provides endpoints for recording events, state changes, and audit trails for tickets, ensuring transparency and traceability across all task operations.

## Main Features

- **Task History Recording**: Add history events for any ticket, including state changes, actions, and authorship.
- **History Retrieval**: Query the full history of a ticket by its ID, including all events and state transitions.
- **Validation**: Ensures all history records are valid before saving.
- **Logging**: Integrated with Serilog for structured error and event logging.
- **Entity Framework Core**: Uses EF Core for data access and migrations.
- **RESTful API**: Simple endpoints for adding and retrieving history.

## Solution Structure

- **Controllers/**: Exposes REST endpoints for history operations.
- **Models/**: DTOs for history events (`TaskHistoryDto`).
- **Entities/**: Database entities for history records (`TicketHistory`).
- **Services/**: Business logic for writing and retrieving history (`HistoryService`).
- **Data/**: EF Core DbContext for database access.
- **Migrations/**: Database schema migrations.
- **Logging/**: Serilog integration for error and event logs.

## Key Endpoints

- `POST /api/thistory/add` — Add a new history event for a ticket.
- `GET /api/thistory/info/{taskId}` — Retrieve all history events for a given ticket.

## Data Model

- **TaskHistoryDto**: Represents a history event, including task ID, event name, previous and new state, author, and timestamps.
- **TicketHistory**: Entity mapped to the database, mirroring the DTO structure.

## How It Works

- When a ticket changes state or an event occurs, a history record is created and sent to TaskHistoryService.
- The service validates the record and saves it to the database.
- History can be queried at any time to see the full audit trail for a ticket.

## Configuration

- All settings (connection strings, logging, etc.) are managed via `appsettings.json`.
- EF Core migrations are used for schema management.

## Getting Started

1. **Clone the repository**
2. **Configure your database connection** in `appsettings.json`.
3. **Run EF Core migrations** to set up the schema.
4. **Start the API** using `dotnet run` or your preferred IDE.

## Development

- Follows best practices for async/await, dependency injection, and separation of concerns.
- DTOs use data annotations for validation.
- All business logic is encapsulated in service classes.

## Extending the Service

- Add new endpoints in `Controllers/TaskHistoryController.cs`.
- Extend business logic in `Services/Implementations/HistoryService.cs`.
- Update data models in `Models/` and `Entities/` as needed.

## License

This project is proprietary to the owner. Contact the maintainer for licensing details.
