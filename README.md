# desenvolvimento-entrevista-cyncly

A full-stack ERP-style demo application built to demonstrate senior-level engineering capabilities across C#/.NET, SQL Server, and React. The project simulates real enterprise workflows including customer management, order processing, audit logging, CSV data imports, and business reporting.

---

## Project Overview

This repository contains a complete, production-structured application following **Clean Architecture** principles on the backend and a modern **React + TypeScript** dashboard on the frontend. It is designed as a technical portfolio piece that showcases end-to-end development skills in an enterprise context.

The system manages customers and orders with full CRUD operations, enforces business rules around order lifecycle (Draft → Confirmed → Canceled), and includes professional modules commonly found in ERP systems such as audit trails, bulk imports, and aggregated reports.

---

## Architecture

The backend follows **Clean Architecture** with four distinct layers ensuring separation of concerns and testability:

```
desenvolvimento-entrevista-cyncly/
│
├── backend/                              .NET 8 Web API
│   ├── src/
│   │   ├── ErpDemo.Domain/              Entities, Enums, Repository Interfaces
│   │   ├── ErpDemo.Application/         DTOs, Services, Validators, Service Interfaces
│   │   ├── ErpDemo.Infrastructure/      EF Core DbContext, Repositories, Migrations
│   │   └── ErpDemo.Api/                 Controllers, Middleware, Auth Config
│   ├── tests/
│   │   └── ErpDemo.Tests/               Unit Tests (xUnit + Moq + FluentAssertions)
│   └── Dockerfile
│
├── frontend/                             React + Vite + TypeScript
│   ├── src/
│   │   ├── api/                         Centralized API client with interceptors
│   │   ├── context/                     Auth context provider
│   │   ├── components/                  Layout and shared components
│   │   ├── pages/                       Login, Customers, Orders
│   │   └── types/                       TypeScript type definitions
│   └── Dockerfile
│
├── docker-compose.yml                    SQL Server + API + Frontend (Nginx)
└── README.md
```

---

## Backend Technologies

| Technology           | Purpose                                      |
|----------------------|----------------------------------------------|
| **.NET 8**           | Web API framework                            |
| **C# 12**           | Primary language                             |
| **SQL Server 2022** | Relational database                          |
| **Entity Framework Core 8** | ORM with code-first migrations       |
| **FluentValidation** | Request validation with clear rules         |
| **JWT Bearer**       | Token-based authentication with role claims |
| **Swagger / OpenAPI** | API documentation                          |
| **xUnit + Moq**     | Unit testing framework                       |

## Frontend Technologies

| Technology           | Purpose                                      |
|----------------------|----------------------------------------------|
| **React 18**         | UI library                                  |
| **TypeScript**       | Type-safe JavaScript                        |
| **Vite**             | Build tool and dev server                   |
| **Axios**            | HTTP client with auth interceptors          |
| **React Router v6**  | Client-side routing                         |

---

## Main Features

### Customer Management
Full CRUD with pagination, search filters (name, document, status), and document uniqueness validation (CPF/CNPJ).

<!-- ![Customers Screenshot](docs/screenshots/customers.png) -->

### Order Management
Complete order lifecycle with items management. Orders follow a strict status flow: **Draft → Confirmed → Canceled**. Business rules enforce that confirmed orders cannot have items modified and canceled orders are fully locked.

<!-- ![Orders Screenshot](docs/screenshots/orders.png) -->

### Authentication & Authorization
JWT-based authentication with role-based access control. Two roles are supported:
- **Admin** — Full access including delete operations, imports, and audit logs
- **Operator** — Standard CRUD operations without destructive actions

<!-- ![Login Screenshot](docs/screenshots/login.png) -->

### Business Reports
Aggregated reporting endpoint returning total orders, total revenue (from confirmed orders), orders grouped by status, and top 10 customers ranked by spend. Queries are optimized using EF Core `GroupBy` with server-side aggregation.

### Audit Log
Automatic change tracking for Customer and Order entities. Every create, update, and delete operation is recorded with the full before/after JSON state, the acting user, and a timestamp. Queryable via API with filters on entity type, action, user, and date range.

### CSV Customer Import
Bulk import endpoint accepting CSV files with validation, duplicate detection (both in-file and against the database), and a detailed summary report showing inserted, duplicated, and invalid rows with per-row error details.

---

## How to Run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker](https://www.docker.com/)

### Option 1 — Docker Compose (Recommended)

```bash
docker compose up --build -d
```

| Service   | URL                           |
|-----------|-------------------------------|
| Frontend  | http://localhost:3000          |
| API       | http://localhost:5000          |
| Swagger   | http://localhost:5000/swagger  |

### Option 2 — Manual Setup

#### 1. Database Setup

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2024!" \
  -p 1433:1433 --name erp-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

#### 2. Backend Setup

```bash
cd backend
dotnet restore
dotnet run --project src/ErpDemo.Api
```

The API starts at `http://localhost:5000` and applies migrations automatically on startup. Swagger is available at `http://localhost:5000/swagger`.

#### 3. Frontend Setup

```bash
cd frontend
npm install
npm run dev
```

The frontend starts at `http://localhost:5173`.

#### 4. Running Tests

```bash
cd backend
dotnet test
```

---

## Example Credentials

| Email                | Password | Role     |
|----------------------|----------|----------|
| `admin@demo.com`     | `123456` | Admin    |
| `operator@demo.com`  | `123456` | Operator |

The database is seeded with 3 customers and 2 orders on first run.

---

## API Endpoints

### Auth
| Method | Route              | Auth   | Description            |
|--------|--------------------|--------|------------------------|
| POST   | `/api/auth/login`  | Public | Returns JWT token      |

### Customers
| Method | Route                   | Auth       | Description            |
|--------|-------------------------|------------|------------------------|
| GET    | `/api/customers`        | Bearer     | List with pagination   |
| GET    | `/api/customers/{id}`   | Bearer     | Get by ID              |
| POST   | `/api/customers`        | Bearer     | Create customer        |
| PUT    | `/api/customers/{id}`   | Bearer     | Update customer        |
| DELETE | `/api/customers/{id}`   | Admin      | Delete customer        |

### Orders
| Method | Route                                | Auth       | Description            |
|--------|--------------------------------------|------------|------------------------|
| GET    | `/api/orders`                        | Bearer     | List with pagination   |
| GET    | `/api/orders/{id}`                   | Bearer     | Get by ID with items   |
| POST   | `/api/orders`                        | Bearer     | Create order           |
| PATCH  | `/api/orders/{id}/status`            | Bearer     | Update status          |
| POST   | `/api/orders/{id}/items`             | Bearer     | Add item               |
| PUT    | `/api/orders/{id}/items/{itemId}`    | Bearer     | Update item            |
| DELETE | `/api/orders/{id}/items/{itemId}`    | Bearer     | Remove item            |
| DELETE | `/api/orders/{id}`                   | Admin      | Delete order           |

### Imports
| Method | Route                     | Auth  | Description                |
|--------|---------------------------|-------|----------------------------|
| POST   | `/api/imports/customers`  | Admin | Import customers from CSV  |

### Audit
| Method | Route          | Auth  | Description                    |
|--------|----------------|-------|--------------------------------|
| GET    | `/api/audit`   | Admin | Query audit logs with filters  |

### Reports
| Method | Route                   | Auth   | Description              |
|--------|-------------------------|--------|--------------------------|
| GET    | `/api/reports/summary`  | Bearer | Business summary report  |

---

## Screenshots

> Screenshots can be added to a `docs/screenshots/` directory.

| Screen          | Preview                                            |
|-----------------|----------------------------------------------------|
| Login           | <!-- ![Login](docs/screenshots/login.png) -->      |
| Customers List  | <!-- ![Customers](docs/screenshots/customers.png) --> |
| Order Detail    | <!-- ![Orders](docs/screenshots/orders.png) -->    |
| Swagger API     | <!-- ![Swagger](docs/screenshots/swagger.png) -->  |

---

## License

This project is intended for demonstration and interview purposes.
