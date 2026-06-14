README.TXT
==========

BlinkDemoApi
------------
Backend for Blinkit/BigBasket/Dunzo-style mobile app (React Native frontend + later React.js Admin Portal).
Clean Architecture • .NET 9 • EF Core • SQLite • JWT Auth • Refresh Tokens • Serilog • OpenAPI + Swagger UI


1) What is this project?
------------------------
BlinkDemoApi is a REST API backend to support:
- User registration + login using Mobile + Password
- JWT access token + Refresh token rotation
- /me/bootstrap endpoint to load user + addresses + cart + recent orders in one call
- Catalog read APIs: categories, subcategories, products
- Address CRUD
- Cart CRUD (upsert quantity + clear)
- Orders: place, list, detail, cancel
- Consistent JSON response envelope for all APIs


2) Tech Stack / Libraries
-------------------------
- .NET 9 (TargetFramework: net9.0)
- ASP.NET Core Web API (Controllers)
- EF Core (SQLite default to run instantly)
- JWT Bearer Auth
- Refresh tokens stored as HASH
- BCrypt password hashing
- Serilog logging
- FluentValidation
- OpenAPI JSON + Swagger UI


3) Architecture (Clean Architecture)
------------------------------------
Solution folders:

src/
  BlinkDemoApi.Domain
    - Entities + Enums only (no EF, no ASP.NET)
  BlinkDemoApi.Application
    - DTOs, interfaces (ports), validators, application services
  BlinkDemoApi.Infrastructure
    - DbContext + EF mappings, repositories implementations, security implementations, DI registration
  BlinkDemoApi.Api
    - Controllers, middleware, Program.cs, appsettings

Dependency direction:
API -> Application -> Domain
Infrastructure -> Application & Domain


4) Database
-----------
Default DB is SQLite for zero config:
- ConnectionStrings:Default = "Data Source=blinkdemo.db"
DB file created automatically on first run.

IMPORTANT:
- This project currently uses EnsureCreated() for dev.
- If you change tables/mappings, delete blinkdemo.db and run again.


5) Standard API Response Format (Every endpoint)
------------------------------------------------
All APIs return an envelope like:

{
  "success": true/false,
  "statusCode": 200/400/401/404/409/500,
  "message": "Human friendly message",
  "errorCode": "SOME_CODE_OR_NULL",
  "traceId": "server trace id",
  "data": { ... payload ... }
}

Common error codes used:
- VALIDATION_FAILED
- DUPLICATE_MOBILE
- DUPLICATE_EMAIL
- INVALID_CREDENTIALS
- UNAUTHORIZED
- NOT_FOUND
- CONFLICT
- SERVER_ERROR


6) Authentication rules (Important for Mobile App)
--------------------------------------------------
- Register + Login DO NOT require token.
- All other APIs require Bearer token header.

Header:
Authorization: Bearer <accessToken>

Refresh token flow:
- On login/register: API returns accessToken + refreshToken
- Refresh token stored in DB as SHA256 HASH (raw token not stored)
- Refresh endpoint rotates token:
  - old refresh token is revoked
  - new access + refresh token returned

Roles:
- Roles are not enabled now, but code is written to add roles later easily.


7) Order Status (Must match frontend)
-------------------------------------
Allowed values:
- PLACED
- CONFIRMED
- DELIVERED
- CANCELLED


8) OpenAPI / Swagger (API definition file)
------------------------------------------
OpenAPI JSON (machine-readable schema):
- /openapi/v1.json

Swagger UI (human friendly UI):
- /swagger

You already added:
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/openapi/v1.json", "BlinkDemoApi v1");
});

So now you can open Swagger UI in browser and test APIs quickly:
- https://localhost:<port>/swagger

For mobile integration, you can use the OpenAPI JSON:
- https://localhost:<port>/openapi/v1.json


9) React Native Integration Guide
---------------------------------

A) Basic manual integration (Axios)
-----------------------------------
1) Install:
   npm install axios

2) Create api client (example):
   - create file: src/services/api.ts

   import axios from "axios";

   export const api = axios.create({
     baseURL: "http://<YOUR-IP>:5261",
     timeout: 10000,
   });

   export const setAuthToken = (token: string | null) => {
     if (token) api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
     else delete api.defaults.headers.common["Authorization"];
   };

3) Use:
   const res = await api.get("/api/catalog/products");
   console.log(res.data);

IMPORTANT for iOS/Android:
- Use LAN IP (example 192.168.x.x) when testing on a real device.
- Ensure same WiFi network.
- Ensure your Mac firewall allows incoming traffic.


B) Recommended: Generate TypeScript types from OpenAPI
------------------------------------------------------
This avoids manual typing and helps mapping endpoints quickly.

1) Generate types:
   npx openapi-typescript http://<YOUR-IP>:5261/openapi/v1.json --output api-types.ts

2) Use the generated types in your RN project to create typed services.


10) API Endpoints (Quick Reference)
-----------------------------------

AUTH:
- POST /api/auth/register      (public)
- POST /api/auth/login         (public)
- POST /api/auth/refresh       (protected)

ME:
- GET  /api/me/bootstrap       (protected)

CATALOG (protected):
- GET  /api/catalog/categories
- GET  /api/catalog/subcategories?categoryId=#
- GET  /api/catalog/products?categoryId=#   (categoryId optional)

ADDRESSES (protected):
- GET    /api/addresses
- POST   /api/addresses
- PUT    /api/addresses/{id}
- DELETE /api/addresses/{id}
- POST   /api/addresses/{id}/set-default

CART (protected):
- GET    /api/cart
- POST   /api/cart/upsert
- DELETE /api/cart/{productId}
- POST   /api/cart/clear

ORDERS (protected):
- GET  /api/orders
- GET  /api/orders/{id}
- POST /api/orders/place
- POST /api/orders/{id}/cancel


11) Database Tables (Current)
-----------------------------
- users
  id, name, mobile (unique), email (unique), passwordHash, createdAt, updatedAt

- user_addresses
  id, userId(FK users), label, addressLine, pincode, city, lat, lng, isDefault, createdAt, updatedAt

- categories
  id, name, iconUrl, createdAt, updatedAt

- subcategories
  id, categoryId(FK categories), name, createdAt, updatedAt
  unique: (categoryId, name)

- products
  id, name, description, price, mrp, imageUrl, categoryId(FK categories), stockQty, unit, createdAt, updatedAt

- cart_items
  id, userId(FK users), productId(FK products), quantity, createdAt, updatedAt
  unique: (userId, productId)

- orders
  id, userId(FK users), totalAmount, status, createdAt, updatedAt

- order_items
  id, orderId(FK orders), productId(FK products), quantity, priceAtPurchase, subtotal, createdAt, updatedAt

- refresh_tokens
  id, userId(FK users), tokenHash, expiresAt, revokedAt, deviceInfo, createdAt, updatedAt
  unique: (userId, tokenHash)


12) Running locally
-------------------
From solution root:

dotnet restore
dotnet run --project src/BlinkDemoApi.Api

First run will create:
- blinkdemo.db (SQLite file)


13) Deployment (Modern approach)
--------------------------------

A) Publish (recommended for any server)
---------------------------------------
Publish produces the correct deployable output (do NOT upload bin/Release directly).

dotnet publish src/BlinkDemoApi.Api -c Release -o publish

Upload the "publish" folder to your server.


B) Docker Deployment (recommended for cloud)
--------------------------------------------
1) Create a Dockerfile in repo root:

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/BlinkDemoApi.Api -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BlinkDemoApi.Api.dll"]

2) Build image:
docker build -t blinkdemoapi .

3) Run:
docker run -p 8080:8080 blinkdemoapi

4) Cloud options:
- Azure App Service (Docker)
- AWS ECS / Fargate
- Google Cloud Run
- DigitalOcean Apps


C) Linux Server (Ubuntu) with systemd + Nginx
---------------------------------------------
1) Publish:
dotnet publish src/BlinkDemoApi.Api -c Release -o publish

2) Copy publish folder to server:
Example path: /var/www/blinkdemo

3) Install runtime:
sudo apt install dotnet-runtime-9.0

4) Create service file:
sudo nano /etc/systemd/system/blinkdemo.service

[Unit]
Description=BlinkDemoApi
After=network.target

[Service]
WorkingDirectory=/var/www/blinkdemo
ExecStart=/usr/bin/dotnet BlinkDemoApi.Api.dll
Restart=always
SyslogIdentifier=blinkdemo-api

[Install]
WantedBy=multi-user.target

5) Enable + start:
sudo systemctl enable blinkdemo
sudo systemctl start blinkdemo

6) Nginx reverse proxy routes traffic from domain->Kestrel port.


D) Windows IIS Deployment
-------------------------
1) Install ASP.NET Core Hosting Bundle on server.
2) Publish:
dotnet publish src/BlinkDemoApi.Api -c Release -o publish
3) Create IIS website pointing to publish folder.
4) App Pool: No Managed Code.
5) Deploy by copying the publish folder.


14) Production Notes (Important)
--------------------------------
- Replace SQLite with SQL Server/PostgreSQL in production.
- Use environment variables or secret vault for Jwt:SigningKey.
- Replace EnsureCreated() with EF migrations in production.
- Add CORS settings for your mobile + admin portal origins later.
- Add seed data for categories/products (optional for quick UI testing).


15) Next recommended improvements
---------------------------------
- Admin APIs (CRUD) for categories/subcategories/products
- Pagination + search for products
- Order status transitions for admin (CONFIRMED/DELIVERED)
- Seeding scripts for demo data
- EF migrations + proper DB versioning
- Rate limiting + request logging enhancements


END
===