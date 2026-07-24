Library Management API

Library Management API is an ASP.NET Core Web API for managing books, authors, members, and loans. It was created to demonstrate CRUD operations, layered architecture, validation, pagination, error handling, and unit testing in a maintainable backend project.

Tech Stack

ASP.NET Core Web API

Entity Framework Core and SQL Server

Swagger / OpenAPI

xUnit and Moq

Architecture

The project follows this request flow:

Controller -> Service -> Repository -> AppDbContext -> SQL Server

API: Controllers, middleware, and application configuration

Application: Service implementations and business logic

Core: Entities, DTOs, interfaces, and common classes

Infrastructure: AppDbContext, repositories, BaseRepository, and migrations

Tests: Service-layer unit tests

BaseRepository<T> contains the shared CRUD operations, while entity repositories only contain entity-specific queries and checks. DTOs are used to avoid returning database entities directly from the API.

Main Features

Full CRUD operations for books, authors, members, and loans

Data validation with Data Annotations

Centralized error handling with ExceptionMiddleware

Unique book code and member email checks

Pagination and sorting for all list endpoints

Correct HTTP status codes

Swagger documentation and endpoint testing

BookService unit test with xUnit and Moq

Entity Relationships

Book and Author: many-to-many

Book and Loan: one-to-many

Member and Loan: one-to-many

API Endpoints

Resource

Collection endpoints

Endpoints by ID

Books

GET, POST /api/books

GET, PUT, DELETE /api/books/{id}

Authors

GET, POST /api/authors

GET, PUT, DELETE /api/authors/{id}

Members

GET, POST /api/members

GET, PUT, DELETE /api/members/{id}

Loans

GET, POST /api/loans

GET, PUT, DELETE /api/loans/{id}

All list endpoints support pageNumber, pageSize, sortBy, and sortDirection query parameters.

Example:

GET /api/books?pageNumber=1&pageSize=5&sortBy=title&sortDirection=asc

Getting Started

Clone the repository and open the solution in Visual Studio.

Add your SQL Server connection string as DefaultConnection in appsettings.json.

Apply the migrations from Package Manager Console:

Update-Database -Project LibraryManagement.Infrastructure -StartupProject LibraryManagement

Set LibraryManagement as the startup project and run it.

Open /swagger to view and test the endpoints.

Testing

The BookService unit test mocks IBookRepository with Moq, so it runs without a real database.

Run the tests from Visual Studio Test Explorer or use:

dotnet test