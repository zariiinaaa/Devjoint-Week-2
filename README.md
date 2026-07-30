# Library Management API — Week 2

## Overview

During Week 2, authentication and authorization were added to the Library Management API. Users can now register, log in, receive a JWT access token, and access endpoints according to their roles.

## Implemented Features

- User entity with `Username`, `Email`, `PasswordHash`, and `Role`
- Unique username and email validation
- Password hashing with BCrypt
- User registration and login endpoints
- JWT access token generation and validation
- Stateless authentication with Bearer tokens
- Role-based authorization for `User` and `Admin`
- Correct `401 Unauthorized` and `403 Forbidden` responses
- Token expiration validation

## Authentication Endpoints

| Method | Endpoint | Description | Access |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | Registers a new user | Public |
| POST | `/api/auth/login` | Authenticates a user and returns a JWT | Public |

Successful registration returns `201 Created`, while successful login returns `200 OK`. Both responses include the user information, role, access token, and token expiration time.

## Password Security

Passwords are never stored as plain text. BCrypt is used to hash each password, and only the generated `PasswordHash` is saved in the SQL Server database.

The username and email are checked before registration to prevent duplicate users.

## JWT Authentication

The generated JWT contains the user's:

- Identifier
- Username
- Email
- Role
- Token identifier
- Expiration time

Protected requests must include the token in the authorization header:

```http
Authorization: Bearer <access_token>
```

The application uses stateless authentication, so no server-side session is stored. The token is validated on every protected request.

## Role-Based Authorization

| Operations | User | Admin |
| --- | --- | --- |
| GET books, authors, members, and loans | Allowed | Allowed |
| POST new records | Forbidden | Allowed |
| PUT existing records | Forbidden | Allowed |
| DELETE records | Forbidden | Allowed |

Newly registered accounts receive the `User` role by default. The user's role is added to the JWT and checked by ASP.NET Core authorization.

## Authentication Responses

- `401 Unauthorized` — the token is missing, invalid, or expired.
- `403 Forbidden` — the token is valid, but the user does not have the required role.

For example, a request without a token to a protected GET endpoint returns `401`. A user with the `User` role can access GET endpoints, but receives `403` when trying to use an Admin-only POST, PUT, or DELETE endpoint.

## Token Expiration

The access token lifetime is configured in `appsettings.json`:

```json
"JwtSettings": {
  "Key": "<YOUR_SECRET_KEY>",
  "Issuer": "LibraryManagement.Api",
  "Audience": "LibraryManagement.Client",
  "ExpirationMinutes": 60
}
```

Lifetime validation is enabled, and `ClockSkew` is set to zero. Therefore, an expired token is rejected immediately with `401 Unauthorized`.

> Do not commit real JWT secrets or access tokens to GitHub.

## Technologies Used in Week 2

- ASP.NET Core JWT Bearer Authentication
- System.IdentityModel.Tokens.Jwt
- BCrypt.Net
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI

## Week 2 Result

By the end of Week 2, the API supports secure registration and login, BCrypt password hashing, JWT authentication, protected endpoints, role-based access control, correct authentication error responses, and token expiration handling.
