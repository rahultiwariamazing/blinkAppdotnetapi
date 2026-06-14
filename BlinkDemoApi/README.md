# BlinkDemoApi

Clean Architecture style ASP.NET Core Web API backend for an e-commerce/grocery app flow (auth, catalog, cart, orders, addresses, and profile features).

## Tech stack

- .NET 9 (SDK)
- ASP.NET Core Web API
- Entity Framework Core
- JWT Bearer authentication
- FluentValidation
- Serilog
- Swagger / OpenAPI (Swashbuckle)

## Solution structure

```text
BlinkDemoApi.sln
src/
	BlinkDemoApi.Api/            # Presentation layer (controllers, middleware, Program.cs)
	BlinkDemoApi.Application/    # Use cases, DTOs, validation, service contracts
	BlinkDemoApi.Domain/         # Core entities and enums
	BlinkDemoApi.Infrastructure/ # EF Core DbContext, repositories, security implementation
```

## Prerequisites

- .NET 9 SDK installed
- A configured database connection string in app settings or environment variables

## Configuration

Main settings are in:

- `src/BlinkDemoApi.Api/appsettings.json`
- `src/BlinkDemoApi.Api/appsettings.Development.json`

Required JWT settings:

```json
{
	"Jwt": {
		"Issuer": "your-issuer",
		"Audience": "your-audience",
		"SigningKey": "your-long-strong-secret-key"
	}
}
```

If `Jwt:SigningKey` is missing, the API intentionally fails at startup.

## Run locally

```bash
dotnet restore
dotnet build BlinkDemoApi.sln
dotnet run --project src/BlinkDemoApi.Api
```

Once running, open Swagger UI at:

- `https://localhost:<port>/swagger`

## Authentication flow

1. Register using `POST /api/auth/register`
2. Login using `POST /api/auth/login`
3. Copy the access token from the response
4. Click Authorize in Swagger and provide: `Bearer <token>`
5. Call protected endpoints (for example, addresses endpoints)

## Addresses endpoints (authenticated)

Base route: `api/addresses`

- `GET /api/addresses` - Get current user addresses
- `POST /api/addresses` - Create address
- `PUT /api/addresses/{id}` - Update address
- `DELETE /api/addresses/{id}` - Delete address
- `POST /api/addresses/{id}/set-default` - Mark address as default

## Response envelope

Controllers return a consistent envelope from shared API response helpers with:

- `statusCode`
- `message`
- `traceId`
- `data` (on success)
- error metadata (on failure)

## Logging and error handling

- Serilog request logging is enabled.
- A global exception handler is registered.
- Forwarded headers are configured for reverse proxy deployments.

## Deployment notes

Published output is available under `publish/`.

For hosting and deployment-specific guidance, see `NOTES.md`.
