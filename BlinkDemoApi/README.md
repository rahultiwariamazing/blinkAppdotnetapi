# 🚀 BlinkDemoApi

A Clean Architecture-based ASP.NET Core Web API built for modern e-commerce and grocery applications.

BlinkDemoApi provides authentication, product catalog management, cart operations, order processing, address management, and user bootstrap APIs through a secure and scalable backend architecture.

---

## 📖 Overview

BlinkDemoApi serves as the backend foundation for mobile and web applications that require:

- User Authentication
- Product Catalog Management
- Shopping Cart Operations
- Order Processing
- Address Management
- User Bootstrap APIs
- JWT Security
- PostgreSQL Persistence

The solution follows Clean Architecture principles to maintain separation of concerns, scalability, testability, and long-term maintainability.

---

## 🔒 Secure Configuration (Local + Production)

Secrets must never be committed to Git. This repository keeps placeholders only in:

- src/BlinkDemoApi.Api/appsettings.json
- src/BlinkDemoApi.Api/appsettings.Development.json

Configuration loading order in runtime:

1. appsettings.json
2. appsettings.Development.json
3. .NET User Secrets (Development only)
4. Environment Variables

### Local Development (User Secrets)

Run from repo root:

```bash
dotnet user-secrets init --project src/BlinkDemoApi.Api
dotnet user-secrets set "ConnectionStrings:Default" "<your-local-connection-string>" --project src/BlinkDemoApi.Api
dotnet user-secrets set "Jwt:SigningKey" "<your-long-random-signing-key>" --project src/BlinkDemoApi.Api
dotnet user-secrets set "Jwt:Issuer" "BlinkDemoApi" --project src/BlinkDemoApi.Api
dotnet user-secrets set "Jwt:Audience" "BlinkDemoApiClients" --project src/BlinkDemoApi.Api
```

Then run the API normally:

```bash
dotnet restore
dotnet build BlinkDemoApi.sln
dotnet run --project src/BlinkDemoApi.Api
```

### Production (Environment Variables)

Set these environment variables in your host/container:

- ConnectionStrings__Default
- Jwt__SigningKey
- Jwt__Issuer
- Jwt__Audience
- Jwt__AccessTokenMinutes
- Jwt__RefreshTokenDays

Example:

```bash
export ConnectionStrings__Default="<prod-connection-string>"
export Jwt__SigningKey="<prod-long-random-signing-key>"
export Jwt__Issuer="BlinkDemoApi"
export Jwt__Audience="BlinkDemoApiClients"
```

---

## ✨ Features

### 🔐 Authentication & Security

- User Registration
- User Login
- JWT Access Tokens
- Refresh Token Rotation
- BCrypt Password Hashing
- Authorization Policies
- Secure Token Validation

### 🛍️ Product Catalog

- Categories
- Subcategories
- Product Listings
- Product Search
- Category-Based Filtering

### 🛒 Cart Management

- Add To Cart
- Update Quantity
- Remove Cart Items
- Clear Cart
- Stock Validation

### 📦 Order Management

- Place Orders
- Order History
- Order Details
- Order Cancellation
- Transaction Handling

### 📍 Address Management

- Create Address
- Update Address
- Delete Address
- Set Default Address
- Multiple Saved Addresses

### 👤 User Bootstrap API

Single endpoint for:

- User Profile
- Saved Addresses
- Cart Summary
- Recent Orders

Designed to reduce frontend API calls during app startup.

---

## 🏗️ Architecture

### Clean Architecture

```text
Presentation Layer
(API Controllers)
          ↓
Application Layer
(Services / DTOs / Validators)
          ↓
Domain Layer
(Entities / Enums)
          ↓
Infrastructure Layer
(Repositories / Security / EF Core)
          ↓
PostgreSQL Database
```

### Dependency Flow

```text
Api
 ↓
Application
 ↓
Domain

Infrastructure
 ↓
Application
 ↓
Domain
```

---

## 📂 Solution Structure

```text
BlinkDemoApi
│
├── docs/
│   ├── PROJECT_DETAILS.md
│   └── API_COLLECTION.md
│
├── src/
│   ├── BlinkDemoApi.Api
│   ├── BlinkDemoApi.Application
│   ├── BlinkDemoApi.Domain
│   └── BlinkDemoApi.Infrastructure
│
├── BlinkDemoApi.sln
├── README.md
├── LICENSE
└── NOTES.md
```

### Projects

#### BlinkDemoApi.Api

Responsible for:

- Controllers
- Middleware
- Authentication Setup
- Swagger
- Configuration

#### BlinkDemoApi.Application

Responsible for:

- Business Logic
- DTOs
- Service Layer
- Validation Rules
- Interfaces

#### BlinkDemoApi.Domain

Responsible for:

- Entities
- Enums
- Core Business Objects

#### BlinkDemoApi.Infrastructure

Responsible for:

- Entity Framework Core
- Repositories
- JWT Services
- Password Hashing
- Database Access

---

## 🗃️ Database

### Technology

- PostgreSQL
- Entity Framework Core
- Npgsql Provider

### Core Entities

```text
User
RefreshToken
UserAddress
Category
SubCategory
Product
CartItem
Order
OrderItem
```

### Relationship Overview

```text
User
├── Addresses
├── Orders
├── CartItems
└── RefreshTokens

Category
├── SubCategories
└── Products

Order
└── OrderItems
```

---

## 🔐 Authentication Flow

```text
Register
↓
Login
↓
Receive JWT + Refresh Token
↓
Access Protected APIs
↓
Refresh Token
↓
Receive New JWT
```

### Security Features

- JWT Bearer Authentication
- Refresh Token Rotation
- Password Hashing using BCrypt
- Refresh Token Hash Storage
- Authorization Middleware

---

## 🌐 API Modules

### Authentication

```text
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
```

### Catalog

```text
GET /api/catalog/categories
GET /api/catalog/subcategories
GET /api/catalog/products
GET /api/catalog/search
```

### Cart

```text
GET    /api/cart
POST   /api/cart/upsert
DELETE /api/cart/{productId}
POST   /api/cart/clear
```

### Orders

```text
GET  /api/orders
GET  /api/orders/{id}
POST /api/orders/place
POST /api/orders/{id}/cancel
```

### Addresses

```text
GET    /api/addresses
POST   /api/addresses
PUT    /api/addresses/{id}
DELETE /api/addresses/{id}
POST   /api/addresses/{id}/set-default
```

### User Bootstrap

```text
GET /api/me/bootstrap
```

---

## ✅ Current Capabilities

### Implemented

- User Authentication
- JWT Authorization
- Refresh Tokens
- Product Catalog
- Cart Management
- Order Placement
- Order Cancellation
- Address Management
- User Bootstrap API
- Global Exception Handling
- Request Logging
- Entity Framework Core Integration
- PostgreSQL Support
- Clean Architecture Structure
- Swagger Documentation

### Partially Implemented

- Refresh Token UX Improvements
- Validation Coverage Expansion
- Schema Migration Strategy

### Future Improvements

- CQRS + MediatR
- Redis Caching
- Pagination
- Rate Limiting
- Audit Logging
- Background Jobs
- Notification System
- Analytics
- CI/CD Pipeline

---

## 📊 Project Metrics

| Metric | Count |
|----------|----------|
| Projects | 4 |
| Controllers | 7 |
| Endpoints | 21 |
| Entities | 10 |
| Services | 8 |
| Repositories | 6 |
| Validators | 2 |
| Middleware | 1 |

---

## 🧪 API Testing

Swagger UI available in development mode.

Use:

```text
Register
↓
Login
↓
Copy JWT Token
↓
Authorize Swagger
↓
Test Protected APIs
```

---

## 📚 Documentation

Detailed documentation is available in:

```text
docs/PROJECT_DETAILS.md
```

Complete API reference is available in:

```text
docs/API_COLLECTION.md
```

Security guidance is available in:

```text
SECURITY.md
```

These documents include:

- Full Architecture
- Database Design
- Authentication Flow
- API Specifications
- DTO Documentation
- Security Review
- Dependency Analysis
- Change Impact Maps

---

## 🛠 Technology Stack

### Backend

- ASP.NET Core Web API
- .NET 9
- C#

### Database

- PostgreSQL
- Entity Framework Core
- Npgsql

### Security

- JWT Authentication
- BCrypt Password Hashing

### Validation

- FluentValidation

### Documentation

- Swagger / OpenAPI

### Logging

- Serilog

---

## 👨‍💻 Developer

**Rahul Tiwari**

**Mobile Architect | Cloud & AI Enthusiast**

.NET • MAUI • React Native • Flutter • Azure • Firebase • AI

---

## 📄 License

Copyright © 2026 Rahul Tiwari

All Rights Reserved.

Unauthorized use, reproduction, modification, distribution, or commercial use of this software is prohibited without prior written permission from the copyright holder.