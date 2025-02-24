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

## Design Patterns
The project leverages several design patterns to ensure clean, maintainable, and scalable code:
- Factory Pattern:

  - Used in the RepositoryFactory class to create instances of repositories dynamically.

  - This pattern decouples the creation of repository objects from the service layer, making the code more flexible and easier to test.

- Singleton Pattern:

  - Implemented in the LoggerSingleton class to ensure a single instance of the logger is used throughout the application.

  - This pattern is useful for managing shared resources like logging, configuration, or database connections.

- Options Pattern:

  - Used for managing configuration settings (e.g., database connection strings) in a strongly-typed way.

  - The DatabaseSettings class is configured in appsettings.json and injected into services using the IOptions<T> interface.

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

Contacts
- Get All Contacts:

  - Endpoint: GET /api/v1/contact

  - Requires Authentication: Yes

  - Response:
```json
[
  {
    "contactId": 1,
    "contactName": "John Doe",
    "companyId": 1,
    "countryId": 1
  }
]
```
- Filter Contacts:

  - Endpoint: GET /api/v1/contact/filterContacts?countryId=1&companyId=1

  - Response:
    
```json
[
  {
    "contactId": 1,
    "contactName": "John Doe",
    "companyId": 1,
    "countryId": 1
  }
]
```
Companies
- Get All Companies:

  - Endpoint: GET /api/v1/company

  - Response:
```json
[
  {
    "companyId": 1,
    "companyName": "Google"
  }
]
```
- Get Paginated Companies:

  - Endpoint: GET /api/v1/company/companyPagionation?pageNumber=1&pageSize=2

  - Response:
```json
{
  "pageNumber": 1,
  "pageSize": 2,
  "totalPages": 5,
  "totalRecords": 10,
  "data": [
    {
      "companyId": 1,
      "companyName": "Google"
    }
  ]
}
```
Countries
- Get All Countries:

  - Endpoint: GET /api/v1/country

  - Response:
```json
[
  {
    "countryId": 1,
    "countryName": "United States"
  }
]
```

- Get Company Statistics by Country:

  - Endpoint: GET /api/v1/country/getCompanyStatisticsByCountryId?countryId=1

  - Response:
```json
{
  "Apple": 351,
  "Google": 424
}
```

## Additional Features
- FluentValidation: Ensures all input data is validated before processing.

- MediatR: Implements the Mediator pattern for handling requests.

- Minimal API: Support for minimal API endpoints.

- Logging and Error Handling: Built-in logging and error handling for better debugging.
