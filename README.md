## User and Password Security

A `User` entity was added with `Username`, `Email`, and `PasswordHash` fields. Username and email values are configured as unique in the database.

Passwords are never stored as plain text. The `BCrypt.Net-Next` package is used through an `IPasswordHasher` service to hash passwords and verify them securely. The service is registered with dependency injection.

A unit test verifies that:

- The generated hash is different from the original password.
- The correct password matches the hash.
- An incorrect password is rejected.

An EF Core migration was created and applied to add the `Users` table to SQL Server. Registration, login, and JWT generation will be implemented in the next checkpoint.
