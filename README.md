# ERP Demo - Full-Stack Application

Demo full-stack com fluxo de ERP: API em .NET 8 + Frontend React + SQL Server.

## Arquitetura

```
/backend                          → API .NET 8 (Clean Architecture)
  /src
    /ErpDemo.Domain                → Entidades, Enums, Interfaces
    /ErpDemo.Application           → DTOs, Services, Validators
    /ErpDemo.Infrastructure        → EF Core, Repositories, Migrations
    /ErpDemo.Api                   → Controllers, Middleware, Config
  /tests
    /ErpDemo.Tests                 → Testes unitários (xUnit + Moq)
/frontend                         → React + Vite + TypeScript
docker-compose.yml                → SQL Server + API + Frontend
```

## Funcionalidades

- **Autenticação JWT** com roles (Admin / Operator)
- **CRUD de Clientes** com paginação, filtros e validação de documento
- **CRUD de Pedidos** com itens, cálculo automático de total, controle de status
- **Regras de negócio**: pedido confirmado não permite edição de itens; cancelado bloqueia alterações
- **Dashboard React** com login, telas de listagem, criação/edição e detalhes
- **Swagger** documentado com autenticação Bearer

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker](https://www.docker.com/) (para SQL Server)

## Execução Rápida (Docker Compose)

```bash
# Sobe tudo: SQL Server + API + Frontend
docker compose up --build -d

# Acesse:
# Frontend: http://localhost:3000
# API/Swagger: http://localhost:5000/swagger
```

## Execução Manual (Desenvolvimento)

### 1. SQL Server (Docker)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2024!" \
  -p 1433:1433 --name erp-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Backend (.NET 8)

```bash
cd backend

# Restaurar pacotes
dotnet restore

# A API aplica migrations automaticamente no startup
dotnet run --project src/ErpDemo.Api

# API disponível em: http://localhost:5000
# Swagger em: http://localhost:5000/swagger
```

### 3. Frontend (React)

```bash
cd frontend

npm install
npm run dev

# Frontend disponível em: http://localhost:5173
```

### 4. Testes

```bash
cd backend
dotnet test
```

## Usuários Seed

| Email               | Senha  | Role     |
|---------------------|--------|----------|
| admin@demo.com      | 123456 | Admin    |
| operator@demo.com   | 123456 | Operator |

## Dados Seed

O banco já vem com:
- 3 clientes (Empresa Alpha, João Silva, Maria Santos)
- 2 pedidos (1 Draft com 2 itens, 1 Confirmed com 1 item)

## Endpoints da API

### Auth
| Método | Rota             | Descrição                    |
|--------|------------------|------------------------------|
| POST   | /api/auth/login  | Login (retorna JWT)          |

### Customers
| Método | Rota                  | Auth       | Descrição              |
|--------|-----------------------|------------|------------------------|
| GET    | /api/customers        | Bearer     | Lista com paginação    |
| GET    | /api/customers/{id}   | Bearer     | Busca por ID           |
| POST   | /api/customers        | Bearer     | Cria cliente           |
| PUT    | /api/customers/{id}   | Bearer     | Atualiza cliente       |
| DELETE | /api/customers/{id}   | Admin only | Exclui cliente         |

### Orders
| Método | Rota                              | Auth       | Descrição              |
|--------|-----------------------------------|------------|------------------------|
| GET    | /api/orders                       | Bearer     | Lista com paginação    |
| GET    | /api/orders/{id}                  | Bearer     | Busca por ID           |
| POST   | /api/orders                       | Bearer     | Cria pedido com itens  |
| PATCH  | /api/orders/{id}/status           | Bearer     | Altera status          |
| POST   | /api/orders/{id}/items            | Bearer     | Adiciona item          |
| PUT    | /api/orders/{id}/items/{itemId}   | Bearer     | Atualiza item          |
| DELETE | /api/orders/{id}/items/{itemId}   | Bearer     | Remove item            |
| DELETE | /api/orders/{id}                  | Admin only | Exclui pedido          |

### Imports
| Método | Rota                    | Auth       | Descrição                          |
|--------|-------------------------|------------|------------------------------------|
| POST   | /api/imports/customers  | Admin only | Importa clientes via CSV           |

### Audit
| Método | Rota          | Auth       | Descrição                                |
|--------|---------------|------------|------------------------------------------|
| GET    | /api/audit    | Admin only | Lista auditoria com filtros              |

### Reports
| Método | Rota                  | Auth   | Descrição                                  |
|--------|-----------------------|--------|--------------------------------------------|
| GET    | /api/reports/summary  | Bearer | Relatório: totais, status, top clientes    |

## Checklist de Validação

### No Swagger (http://localhost:5000/swagger)
- [ ] POST /api/auth/login com admin@demo.com / 123456 → token retornado
- [ ] GET /api/customers com Bearer token → lista 3 clientes
- [ ] POST /api/customers → cria novo cliente
- [ ] GET /api/orders → lista 2 pedidos com itens
- [ ] POST /api/orders → cria pedido com itens (total calculado)
- [ ] PATCH /api/orders/{id}/status → confirma pedido Draft
- [ ] POST /api/orders/{id}/items em pedido Confirmed → erro 400
- [ ] DELETE /api/customers/{id} com Operator → erro 403
- [ ] POST /api/imports/customers com CSV → relatório de importação
- [ ] GET /api/audit → registros de auditoria das operações
- [ ] GET /api/reports/summary → relatório com totais e top clientes

### No Frontend (http://localhost:5173 ou http://localhost:3000)
- [ ] Login com admin@demo.com / 123456
- [ ] Tela de Clientes: listagem, filtro por nome, criar, editar, excluir
- [ ] Tela de Pedidos: listagem, filtro por status, criar pedido com itens
- [ ] Detalhe do Pedido: ver itens, adicionar/remover item, confirmar/cancelar
- [ ] Logout e login com operator@demo.com → botão Excluir não aparece

## Stack Técnica

- **Backend**: .NET 8, C#, EF Core, SQL Server, JWT, FluentValidation, Swagger
- **Frontend**: React 18, TypeScript, Vite, Axios, React Router
- **Testes**: xUnit, Moq, FluentAssertions
- **Infra**: Docker, Docker Compose, Nginx
