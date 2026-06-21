# Leave Management System (LMS)

A robust, enterprise-grade Leave Management System backend built with ASP.NET Core 8, EF Core, and PostgreSQL. It features Clean Architecture, Role-Based Access Control (RBAC), and optimistic concurrency control for transaction safety.

## Tech Stack
- **Backend:** ASP.NET Core Web API (.NET 8)
- **Language:** C#
- **Database:** PostgreSQL 15
- **ORM:** Entity Framework Core 8
- **Authentication:** JWT Bearer Tokens
- **Validation:** FluentValidation
- **Logging:** Default ASP.NET Core Logging

## Quick Start (Docker)

1. **Spin up PostgreSQL:**
   ```bash
   docker-compose up -d
   ```

2. **Run the Application:**
   ```bash
   dotnet run --project LMS.Api
   ```

   The application will automatically apply EF Core migrations and seed the database on startup.

## Seed Data
The database is seeded with the following default users (Password: `hash`):
- `admin@lms.com` (Admin)
- `hr@lms.com` (HR)
- `manager@lms.com` (Manager)
- `employee@lms.com` (Employee)

## Features
- **RBAC:** Four distinct roles with specific access levels.
- **Concurrency Safety:** Uses PostgreSQL `xmin` internal row versions to prevent race conditions when updating leave balances concurrently.
- **Audit Logging:** EF Core `SaveChangesInterceptor` automatically records all entity modifications to an `AuditLogs` table.
- **Global Exception Handling:** Uses `.NET 8` `IExceptionHandler` to return RFC-compliant `ProblemDetails`.

## Documentation
- [Workflows](docs/workflows.md)
- [Data Flow Diagrams](docs/dfd.md)
