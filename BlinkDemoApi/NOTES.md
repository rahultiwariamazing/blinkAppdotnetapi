# BlinkDemoApi – Notes (Run + Deploy)

This solution is designed to run **immediately** with **SQLite** (no external DB needed).

## 1) Prerequisites

- .NET SDK (recommended: .NET 10)
- Optional: EF Core tools
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## 2) Run locally (1 command)

From the solution folder:

```bash
dotnet restore
dotnet run --project src/BlinkDemoApi.Api
```

The API will create a local SQLite DB file: `blinkdemo.db`.

## 3) OpenAPI

In Development, OpenAPI is exposed at:

- `GET /openapi/v1.json`

You can plug this into any Swagger UI tool.

## 4) Auth flow

- `POST /api/auth/register` (no token)
- `POST /api/auth/login` (no token)
- `GET /api/me/bootstrap` (requires Bearer token)
- `POST /api/auth/refresh` (requires Bearer token + refresh token)

Add header:

- `Authorization: Bearer <accessToken>`

## 5) Publishing (Modern way)

Publish generates a self-contained folder you deploy to server.

### Windows publish (framework-dependent)

```bash
dotnet publish src/BlinkDemoApi.Api -c Release -o publish
```

### Windows publish (self-contained)

```bash
dotnet publish src/BlinkDemoApi.Api -c Release -r win-x64 --self-contained true -o publish
```

## 6) Deploy to IIS (Windows)

1. Install **ASP.NET Core Hosting Bundle** on the server.
2. Create a website in IIS pointing to the `publish` folder.
3. Ensure app pool is **No Managed Code**.
4. Deploy by copying the **publish** folder (not your source).

> Old approach: copying `bin/Release` is not recommended. Always deploy the **publish output**.

## 7) Deploy to Linux (systemd + reverse proxy)

```bash
dotnet publish src/BlinkDemoApi.Api -c Release -r linux-x64 --self-contained false -o publish
```

- Run using `dotnet BlinkDemoApi.Api.dll` behind Nginx.

## 8) Docker (optional)

Add a Dockerfile later when you are ready.

## 9) Swapping DB provider later

Replace the SQLite registration in:

- `src/BlinkDemoApi.Infrastructure/DependencyInjection.cs`

Example for SQL Server:

- add package `Microsoft.EntityFrameworkCore.SqlServer`
- use `UseSqlServer(connectionString)`

## 10) Common errors

- If JWT key is weak/missing → set `Jwt:SigningKey` in `appsettings.Development.json` or environment variables.
- If DB path issues → update `ConnectionStrings:Default`.
