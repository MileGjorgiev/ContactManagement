# Contact Management API
The Contact Management API is a web application built using .NET 8 and C#. It provides CRUD operations for managing contacts, companies, and countries. The API supports advanced features like filtering, pagination, authentication, and more. It follows clean architecture principles and uses technologies like Entity Framework Core, FluentValidation, MediatR, and Swagger for API documentation.

## Features
CRUD Operations: Create, Read, Update, and Delete for Contacts, Companies, and Countries.

Filtering: Filter contacts by countryId and/or companyId.

Pagination: Paginated results for companies.

Authentication: JWT-based authentication for secure API access.

Logging and Error Handling: Built-in logging and error handling for better debugging.

Unit Tests: Comprehensive unit tests for controllers and services.

Swagger UI: Interactive API documentation using Swagger.

Advanced Features:

  - FluentValidation: Input validation for all entities.

  - MediatR: Implementation of the Mediator pattern for handling requests.

  - Minimal API: Support for minimal API endpoints.

  - Pipeline behavior

## Technologies Used
- Backend: .NET 8, C#

- Database: SQL Server (Code-First with Entity Framework Core)

- Authentication: JWT (JSON Web Tokens)

- Validation: FluentValidation

- Mediator Pattern: MediatR

- Logging: Microsoft.Extensions.Logging

- Testing: xUnit, Moq

- API Documentation: Swagger

## API Documentation
Authentication
- Login:

  - Endpoint: POST /api/v1/auth/login

  - Request Body:
 
```json
{
  "username": "login",
  "password": "login"
}
  ```
  - Response:
```json
    {
  "token": "your-jwt-token"
}
```
