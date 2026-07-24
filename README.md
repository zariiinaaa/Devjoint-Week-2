# Library Management API

Library Management API is an ASP.NET Core Web API for managing books, authors, members, and loans. It demonstrates CRUD operations, layered architecture, validation, pagination, API documentation, error handling, and unit testing.

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- xUnit and Moq
- BCrypt.Net-Next

## Architecture

The project follows this request flow:

```text
Controller → Service → Repository → AppDbContext → SQL Server
```

The solution is divided into the following projects:

- **LibraryManagement:** Controllers, middleware, Swagger, and application configuration
- **LibraryManagement.Application:** Service implementations and business logic
- **LibraryManagement.Core:** Entities, DTOs, interfaces, and common classes
- **LibraryManagement.Infrastructure:** AppDbContext, repositories, BaseRepository, and migrations
- **LibraryManagement.Tests:** Service-layer unit tests

`BaseRepository<T>` contains shared CRUD operations, while entity repositories contain only entity-specific queries and checks.

DTOs are used to keep request and response models separate from database entities. Entity-to-DTO mapping is performed manually in the service layer.

## Main Features

- Full CRUD operations for books, authors, members, and loans
- Data validation with Data Annotations
- Centralized error handling with `ExceptionMiddleware`
- Unique book code and member email validation
- Book copy count and loan date validation
- Related book, member, and active membership checks
- Pagination and sorting for all list endpoints
- Correct HTTP status codes, including `200`, `201`, `204`, `400`, `404`, and `409`
- User entity and BCrypt password hashing
- Swagger documentation with XML summaries and response types
- HTTP request examples in `LibraryManagement.http`
- Unit tests for BookService, AuthorService, MemberService, and LoanService

## Entity Relationships

- **Book and Author:** many-to-many
- **Book and Loan:** one-to-many
- **Member and Loan:** one-to-many

Relationships and unique indexes are configured with the EF Core Fluent API.

## API Endpoints

| Resource | Collection endpoints | Endpoints by ID |
|---|---|---|
| Books | `GET`, `POST /api/books` | `GET`, `PUT`, `DELETE /api/books/{id}` |
| Authors | `GET`, `POST /api/authors` | `GET`, `PUT`, `DELETE /api/authors/{id}` |
| Members | `GET`, `POST /api/members` | `GET`, `PUT`, `DELETE /api/members/{id}` |
| Loans | `GET`, `POST /api/loans` | `GET`, `PUT`, `DELETE /api/loans/{id}` |

## Pagination and Sorting

All list endpoints support these query parameters:

- `pageNumber`
- `pageSize`
- `sortBy`
- `sortDirection`

Example:

```http
GET /api/books?pageNumber=1&pageSize=5&sortBy=title&sortDirection=asc
```

Paginated responses include the items, current page, page size, total count, and total pages.

## API Documentation

Swagger includes:

- Endpoint summaries
- Parameter descriptions
- Request and response models
- Documented HTTP response types

Run the project and open:

```text
/swagger
```

## Getting Started

1. Clone the repository:

```bash
git clone https://github.com/zariiiinaaa/Devjoint-Week-1.git
```

2. Open the solution in Visual Studio.

3. Add your SQL Server connection string as `DefaultConnection` in `appsettings.json`.

4. Apply the migrations from Package Manager Console:

```powershell
Update-Database -Project LibraryManagement.Infrastructure -StartupProject LibraryManagement
```

5. Set `LibraryManagement` as the startup project and run the application.

6. Open `/swagger` to view and test the endpoints.

## Testing

The service layer is tested with xUnit and Moq. Repository interfaces are mocked, so the tests run without connecting to SQL Server.

The tests cover:

- Pagination and DTO mapping
- Get, Create, Update, and Delete operations
- Missing record scenarios
- Duplicate book code and member email validation
- Invalid book copy counts
- Invalid loan and return dates
- Missing books and members
- Inactive member validation
- BCrypt password hashing and verification

Run the tests from Visual Studio Test Explorer or use:

```bash
dotnet test
```
