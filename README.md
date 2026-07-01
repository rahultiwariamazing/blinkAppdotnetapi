# 🚀 QuickKartApi

A Clean Architecture-based ASP.NET Core Web API powering modern quick-commerce and grocery delivery applications.

QuickKartApi provides authentication, product catalog management, shopping cart operations, order processing, address management, and user bootstrap APIs through a secure, scalable, and production-oriented backend architecture.

---

## 📖 Overview

QuickKartApi serves as the backend platform powering the QuickKart commerce ecosystem.

The platform provides all core capabilities required by modern grocery and quick-commerce applications, including:

- User Authentication
- Product Catalog Management
- Shopping Cart Operations
- Order Processing
- Address Management
- User Bootstrap APIs
- JWT Security
- PostgreSQL Persistence

The solution follows Clean Architecture principles to ensure maintainability, scalability, testability, and long-term extensibility.

---

## 🔒 Secure Configuration

Secrets must never be committed to Git.

This repository intentionally stores only placeholders inside configuration files.

Configuration loading order:

1. appsettings.json
2. appsettings.Development.json
3. .NET User Secrets
4. Environment Variables

### Local Development

```bash
dotnet user-secrets init --project src/BlinkDemoApi.Api

dotnet user-secrets set "ConnectionStrings:Default" "<your-connection-string>" --project src/BlinkDemoApi.Api

dotnet user-secrets set "Jwt:SigningKey" "<your-long-random-signing-key>" --project src/BlinkDemoApi.Api

dotnet user-secrets set "Jwt:Issuer" "QuickKartApi" --project src/BlinkDemoApi.Api

dotnet user-secrets set "Jwt:Audience" "QuickKartClients" --project src/BlinkDemoApi.Api
```

Run the project:

```bash
dotnet restore
dotnet build BlinkDemoApi.sln
dotnet run --project src/BlinkDemoApi.Api
```

### Production

Configure:

```text
ConnectionStrings__Default
Jwt__SigningKey
Jwt__Issuer
Jwt__Audience
Jwt__AccessTokenMinutes
Jwt__RefreshTokenDays
```

Example:

```bash
export ConnectionStrings__Default="<prod-connection-string>"
export Jwt__SigningKey="<prod-signing-key>"
export Jwt__Issuer="QuickKartApi"
export Jwt__Audience="QuickKartClients"
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
- Category Filtering

### 🛒 Cart Management

- Add To Cart
- Update Quantity
- Remove Items
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

Single endpoint providing:

- User Profile
- Saved Addresses
- Cart Summary
- Recent Orders

Designed to reduce application startup API calls.

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
QuickKartApi
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
├── NOTES.md
└── SECURITY.md
```

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
- BCrypt Password Hashing
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
- Clean Architecture
- Swagger Documentation

### Platform Roadmap

- CQRS + MediatR
- Redis Caching
- Pagination
- Rate Limiting
- Audit Logging
- Background Jobs
- Push Notifications
- Analytics
- CI/CD Pipelines

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

Swagger UI is available during development.

Suggested flow:

```text
Register User
↓
Login User
↓
Receive JWT Token
↓
Authorize Swagger
↓
Browse Catalog
↓
Manage Cart
↓
Place Order
```

---

## 📚 Documentation

### Technical Documentation

```text
docs/PROJECT_DETAILS.md
```

### API Reference

```text
docs/API_COLLECTION.md
```

### Security Guidance

```text
SECURITY.md
```

Documentation covers:

- Architecture
- Authentication Flow
- Database Design
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

Mobile Application Architect | Cloud & AI Enthusiast

```text
.NET • ASP.NET Core • MAUI • React Native • Flutter • Azure • Firebase • AI
```

---

## 📄 License

Copyright © 2026 Rahul Tiwari

All Rights Reserved.

Unauthorized use, reproduction, modification, distribution, or commercial use of this software is prohibited without prior written permission from the copyright holder.
