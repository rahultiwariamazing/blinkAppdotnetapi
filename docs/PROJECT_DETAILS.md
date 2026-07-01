# BlinkDemoApi - Comprehensive Backend Documentation

Generated from actual source code in the solution only.
No refactor or code changes were performed.

## 1. Project Overview

### Project Name
BlinkDemoApi

### What is BlinkDemoApi?
BlinkDemoApi is a Clean Architecture style ASP.NET Core Web API backend for an e-commerce and grocery ordering flow. It provides authentication, catalog browsing, cart management, order placement/cancellation, saved addresses, and a bootstrap endpoint for logged-in users.

### Business Goal
Provide a production-oriented backend foundation for a mobile/web storefront that needs:
- Secure user authentication
- Product discovery
- Cart and checkout lifecycle
- Order history and status tracking
- Address management

### Problem Solved
The API centralizes domain rules and persistence for core commerce workflows so frontend clients do not implement business logic locally. It solves:
- User identity and session lifecycle
- Single source of truth for products/cart/orders
- Consistent API contracts and error envelopes
- Relational data consistency via EF Core and transactions

### Target Users
- End users: customers buying products
- Frontend developers: consuming stable APIs
- Backend developers: extending business features
- Architects: evaluating layering, dependencies, and boundaries
- AI assistants: understanding impact and code navigation
- Recruiters/interviewers: assessing project maturity and scope

### Current Status (Source-Based)
- Functional multi-module backend with 4 projects
- JWT auth with refresh token rotation implemented
- PostgreSQL persistence via EF Core and Npgsql
- Global exception handling and request logging enabled
- No EF Migrations folder in source; schema creation uses EnsureCreated
- CQRS/MediatR pattern not used despite Clean Architecture layering

### Why was it built?
To support a frontend client with end-to-end backend capabilities for authentication and commerce flows while keeping code organized by architectural responsibilities.

### How it supports the frontend application
- Exposes 21 HTTP endpoints under predictable route groups
- Uses a consistent response envelope (success, statusCode, message, traceId, data)
- Provides startup bootstrap payload via /api/me/bootstrap (user + addresses + cart + recent orders)
- Uses backend validations and domain/persistence checks to prevent invalid frontend state

---

## 2. Solution Architecture

## 2.1 Clean Architecture Implementation
The solution follows layered separation:
- API: HTTP transport and middleware concerns
- Application: use-case orchestration, DTO contracts, validators, interfaces
- Domain: core entities/enums only
- Infrastructure: EF Core data access, repository implementations, security implementations

## 2.2 Projects and References
- BlinkDemoApi.Api references:
  - BlinkDemoApi.Application
  - BlinkDemoApi.Infrastructure
- BlinkDemoApi.Application references:
  - BlinkDemoApi.Domain
- BlinkDemoApi.Infrastructure references:
  - BlinkDemoApi.Application
  - BlinkDemoApi.Domain
- BlinkDemoApi.Domain references:
  - None

## 2.3 Dependency Flow
~~~mermaid
flowchart LR
  API[BlinkDemoApi.Api] --> APP[BlinkDemoApi.Application]
  API --> INFRA[BlinkDemoApi.Infrastructure]
  APP --> DOMAIN[BlinkDemoApi.Domain]
  INFRA --> APP
  INFRA --> DOMAIN
~~~

### Notes on boundary quality
- Positive: Domain is framework-agnostic and dependency-free.
- Positive: Application depends on abstractions, Infrastructure supplies concrete implementations.
- Caution: API references Infrastructure directly for composition root convenience.

---

## 3. Project Structure

## 3.1 Complete Solution Tree (source-focused)
BlinkDemoApi/
- BlinkDemoApi.sln
- README.md
- NOTES.md
- src/
  - BlinkDemoApi.Api/
    - Program.cs
    - appsettings.json
    - appsettings.Development.json
    - Controllers/
      - BaseApiController.cs
      - AuthController.cs
      - CatalogController.cs
      - CartController.cs
      - OrdersController.cs
      - AddressesController.cs
      - MeController.cs
    - Middleware/
      - ApiExceptionHandler.cs
  - BlinkDemoApi.Application/
    - Common/
      - ApiResponse.cs
      - ErrorCodes.cs
    - Auth/
      - Dtos/AuthDtos.cs
      - Interfaces/IAuthRepository.cs
      - Interfaces/IJwtTokenService.cs
      - Interfaces/IPasswordHasher.cs
      - Services/AuthService.cs
      - Validators/RegisterRequestValidator.cs
      - Validators/LoginRequestValidator.cs
    - Catalog/
      - Dtos/CatalogDtos.cs
      - Services/CatalogService.cs
    - Cart/
      - Dtos/CartDtos.cs
      - Services/CartService.cs
    - Orders/
      - Dtos/OrderDtos.cs
      - Services/OrderService.cs
    - Addresses/
      - Dtos/AddressDtos.cs
      - Services/AddressService.cs
    - Me/
      - Dtos/BootstrapDtos.cs
      - Services/MeService.cs
  - BlinkDemoApi.Domain/
    - Enums/OrderStatus.cs
    - Entities/
      - BaseEntity.cs
      - User.cs
      - RefreshToken.cs
      - UserAddress.cs
      - Category.cs
      - SubCategory.cs
      - Product.cs
      - CartItem.cs
      - Order.cs
      - OrderItem.cs
  - BlinkDemoApi.Infrastructure/
    - DependencyInjection.cs
    - Data/AppDbContext.cs
    - Repositories/
      - AuthRepository.cs
      - MeRepository.cs
      - CatalogRepository.cs
      - AddressRepository.cs
      - CartRepository.cs
      - OrderRepository.cs
    - Security/
      - BcryptPasswordHasher.cs
      - JwtTokenService.cs

## 3.2 Layer Responsibilities

### API Layer
- Receives HTTP requests and maps route semantics
- Applies authentication/authorization attributes
- Calls application services
- Returns response envelope via BaseApiController
- Delegates error shaping to global exception handler

### Application Layer
- Defines request/response DTO contracts
- Defines interfaces for repositories and security abstractions
- Contains service-level use-case logic orchestration
- Hosts FluentValidation validators for auth payloads

### Domain Layer
- Defines business entities and enum state model
- Holds pure domain data structures, no transport/persistence frameworks

### Infrastructure Layer
- Implements application interfaces using EF Core
- Configures DbContext mappings and relationships
- Implements password hashing and JWT token generation
- Handles transactions for order placement

### Persistence Layer (inside Infrastructure)
- AppDbContext with explicit table and column mapping
- PostgreSQL configured through Npgsql provider
- SaveChanges override updates UpdatedAt

---

## 4. API Documentation (All Controllers and Endpoints)

Common behavior:
- Base route groups per controller
- Protected by JWT except auth register/login
- Standard response envelope: ApiResponse<T>

| Controller | Endpoint | Method | Auth | Request DTO / Params | Response DTO (data) | Validation |
|---|---|---|---|---|---|---|
| AuthController | /api/auth/register | POST | AllowAnonymous | RegisterRequest { Name, Mobile, Email, Password } | AuthResponse | FluentValidation RegisterRequestValidator; duplicate mobile/email checks |
| AuthController | /api/auth/login | POST | AllowAnonymous | LoginRequest { Mobile, Password } | AuthResponse | FluentValidation LoginRequestValidator; credential verification |
| AuthController | /api/auth/refresh | POST | Required | RefreshRequest { RefreshToken } + user id from JWT sub | AuthResponse | RefreshToken non-empty check; active hashed refresh token check |
| CatalogController | /api/catalog/categories | GET | Required | None | List<CategoryDto> | None |
| CatalogController | /api/catalog/subcategories | GET | Required | Query: categoryId (Guid) | List<SubCategoryDto> | categoryId must not be empty |
| CatalogController | /api/catalog/products | GET | Required | Query: categoryId? (Guid?) | List<ProductDto> | Optional filter only |
| CatalogController | /api/catalog/search | GET | Required | Query: q (string) | List<ProductDto> | q must be non-empty |
| CartController | /api/cart | GET | Required | User id from JWT | List<CartItemDto> | JWT user id must parse as Guid |
| CartController | /api/cart/upsert | POST | Required | UpsertCartItemRequest { ProductId, Quantity } | List<CartItemDto> | Product existence, stock checks, quantity logic |
| CartController | /api/cart/{productId} | DELETE | Required | Route: productId (Guid) | List<CartItemDto> | Item must exist for user |
| CartController | /api/cart/clear | POST | Required | User id from JWT | { cleared: true } | JWT user id must parse as Guid |
| OrdersController | /api/orders | GET | Required | User id from JWT | List<OrderDto> | JWT user id must parse as Guid |
| OrdersController | /api/orders/{id} | GET | Required | Route: id (Guid) | OrderDto | Must belong to current user |
| OrdersController | /api/orders/place | POST | Required | PlaceOrderRequest { AddressId? } | OrderDto | Cart non-empty, stock, optional address ownership, transaction |
| OrdersController | /api/orders/{id}/cancel | POST | Required | Route: id (Guid) | { cancelled: true } | Cannot cancel DELIVERED order |
| AddressesController | /api/addresses | GET | Required | User id from JWT | List<AddressDto> | JWT user id must parse as Guid |
| AddressesController | /api/addresses | POST | Required | CreateAddressRequest | AddressDto | No FluentValidation; repository trims and persists |
| AddressesController | /api/addresses/{id} | PUT | Required | Route id + UpdateAddressRequest | AddressDto | Ownership check |
| AddressesController | /api/addresses/{id} | DELETE | Required | Route id | { deleted: true } | Ownership check |
| AddressesController | /api/addresses/{id}/set-default | POST | Required | Route id | { isDefaultSet: true } | Ownership check |
| MeController | /api/me/bootstrap | GET | Required | User id from JWT | BootstrapResponse | User existence check |

### Endpoint Error Envelope
- ValidationException: 400 with VALIDATION_FAILED and field errors
- Unauthorized token scenarios: 401 with UNAUTHORIZED
- Not found scenarios: 404 with NOT_FOUND
- Conflicts/business failures: often 409 with CONFLICT
- Unhandled exceptions: 500 with SERVER_ERROR

---

## 5. Authentication

## 5.1 JWT Implementation
- JWT Bearer configured in Program.cs
- Token validation checks:
  - Issuer
  - Audience
  - Lifetime
  - Signing key
- NameClaimType set to sub

## 5.2 Login Flow
1. Client sends mobile + password.
2. LoginRequestValidator validates shape.
3. AuthService loads user by mobile.
4. BCrypt password verification performed.
5. Access token and refresh token generated.
6. Refresh token hash persisted.
7. AuthResponse returned.

## 5.3 Registration Flow
1. Validator checks name/mobile/email/password.
2. Service verifies unique mobile/email.
3. Password hashed with BCrypt.
4. User persisted.
5. Access/refresh tokens generated.
6. Refresh token hash persisted.
7. AuthResponse returned.

## 5.4 Refresh Flow
1. Endpoint requires valid access token (user id from sub).
2. Input refresh token hashed with SHA-256.
3. Active refresh token row looked up by user + token hash.
4. Existing refresh row revoked.
5. New access and refresh tokens issued.
6. New refresh hash persisted.

## 5.5 Authorization
- Controller-level Authorize on Catalog, Cart, Orders, Addresses, Me
- AuthController uses AllowAnonymous on register/login only
- Refresh endpoint currently requires valid bearer access token

---

## 6. Database

## 6.1 DbContext
AppDbContext defines DbSets:
- Users
- UserAddresses
- Categories
- SubCategories
- Products
- CartItems
- Orders
- OrderItems
- RefreshTokens

## 6.2 Tables and Relationships
- users (1) -> (many) user_addresses
- users (1) -> (many) cart_items
- users (1) -> (many) orders
- users (1) -> (many) refresh_tokens
- categories (1) -> (many) products
- categories (1) -> (many) subcategories
- subcategories (1) -> (many) products
- products (1) -> (many) cart_items
- orders (1) -> (many) order_items
- products (1) -> (many) order_items
- user_addresses (1) -> (many) orders via addressId nullable

## 6.3 ER Diagram
~~~mermaid
erDiagram
  USERS ||--o{ USER_ADDRESSES : has
  USERS ||--o{ CART_ITEMS : owns
  USERS ||--o{ ORDERS : places
  USERS ||--o{ REFRESH_TOKENS : has

  CATEGORIES ||--o{ SUBCATEGORIES : contains
  CATEGORIES ||--o{ PRODUCTS : groups
  SUBCATEGORIES ||--o{ PRODUCTS : classifies

  PRODUCTS ||--o{ CART_ITEMS : appears_in
  ORDERS ||--o{ ORDER_ITEMS : has
  PRODUCTS ||--o{ ORDER_ITEMS : sold_as
  USER_ADDRESSES ||--o{ ORDERS : selected_for
~~~

## 6.4 Migrations Status
- No Migrations source folder detected.
- Startup currently uses EnsureCreatedAsync.
- Implication: schema evolution/versioned migrations are not tracked in source.

---

## 7. Entities (Complete)

| Entity | Fields (Type) | Relationships | Purpose |
|---|---|---|---|
| BaseEntity | Id Guid, CreatedAt DateTime, UpdatedAt DateTime | Base class | Common audit and id fields |
| User | Name string, Mobile string, Email string, PasswordHash string | 1-many Addresses, CartItems, Orders, RefreshTokens | System user account |
| RefreshToken | UserId Guid, TokenHash string, ExpiresAt DateTime, RevokedAt DateTime?, DeviceInfo string? | many-1 User | Refresh token lifecycle and revocation |
| UserAddress | UserId Guid, Label string, AddressLine string, Pincode string, City string, Lat decimal?, Lng decimal?, IsDefault bool | many-1 User | Saved delivery addresses |
| Category | Name string, IconUrl string? | 1-many Products | Top-level product grouping |
| SubCategory | CategoryId Guid, Name string | many-1 Category, 1-many Products | Nested product grouping |
| Product | Name string, Description string?, Price decimal, Mrp decimal, ImageUrl string?, CategoryId Guid, SubcategoryId Guid, StockQty int, Unit string | many-1 Category, many-1 SubCategory | Sellable catalog item |
| CartItem | UserId Guid, ProductId Guid, Quantity int | many-1 User, many-1 Product | User cart line item |
| Order | UserId Guid, AddressId Guid?, TotalAmount decimal, Status OrderStatus | many-1 User, 1-many OrderItems | Order master record |
| OrderItem | OrderId Guid, ProductId Guid, Quantity int, PriceAtPurchase decimal, Subtotal decimal | many-1 Order, many-1 Product | Purchased item snapshot |

---

## 8. Application Layer (Commands, Queries, Handlers, Validators)

This codebase uses Service + Repository orchestration rather than explicit MediatR CQRS handlers.

## 8.1 Operation Mapping

| Type | Operation | Handler/Service | Data Access |
|---|---|---|---|
| Command | Register user | AuthService.RegisterAsync | IAuthRepository |
| Command | Login | AuthService.LoginAsync | IAuthRepository |
| Command | Refresh tokens | AuthService.RefreshAsync | IAuthRepository |
| Query | List categories | CatalogService.GetCategoriesAsync | ICatalogRepository |
| Query | List subcategories | CatalogService.GetSubCategoriesAsync | ICatalogRepository |
| Query | List products | CatalogService.GetProductsAsync | ICatalogRepository |
| Query | Search products | CatalogService.SearchProductsAsync | ICatalogRepository |
| Query | Get cart | CartService.GetMyCartAsync | ICartRepository |
| Command | Upsert cart item | CartService.UpsertAsync | ICartRepository |
| Command | Remove cart item | CartService.RemoveAsync | ICartRepository |
| Command | Clear cart | CartService.ClearAsync | ICartRepository |
| Command | Place order | OrderService.PlaceAsync | IOrderRepository |
| Query | Get orders | OrderService.GetMyOrdersAsync | IOrderRepository |
| Query | Get order detail | OrderService.GetMyOrderDetailAsync | IOrderRepository |
| Command | Cancel order | OrderService.CancelAsync | IOrderRepository |
| Query | Get addresses | AddressService.GetMyAddressesAsync | IAddressRepository |
| Command | Create address | AddressService.CreateAsync | IAddressRepository |
| Command | Update address | AddressService.UpdateAsync | IAddressRepository |
| Command | Delete address | AddressService.DeleteAsync | IAddressRepository |
| Command | Set default address | AddressService.SetDefaultAsync | IAddressRepository |
| Query | Bootstrap current user state | MeService.GetBootstrapAsync | IMeRepository |

## 8.2 Validators
- RegisterRequestValidator
  - Name required, max 100
  - Mobile required and 10-digit regex
  - Email required and valid format
  - Password required, min length 6
- LoginRequestValidator
  - Mobile required and 10-digit regex
  - Password required

---

## 9. Services Inventory

| Service | Layer | Purpose | Key Methods | Dependencies | Primary Consumers |
|---|---|---|---|---|---|
| AuthService | Application | Auth use-cases | RegisterAsync, LoginAsync, RefreshAsync | IAuthRepository, IPasswordHasher, IJwtTokenService | AuthController |
| CatalogService | Application | Catalog read flows | GetCategoriesAsync, GetSubCategoriesAsync, GetProductsAsync, SearchProductsAsync | ICatalogRepository | CatalogController |
| CartService | Application | Cart lifecycle | GetMyCartAsync, UpsertAsync, RemoveAsync, ClearAsync | ICartRepository | CartController |
| OrderService | Application | Order lifecycle | PlaceAsync, GetMyOrdersAsync, GetMyOrderDetailAsync, CancelAsync | IOrderRepository | OrdersController |
| AddressService | Application | Address CRUD/default | GetMyAddressesAsync, CreateAsync, UpdateAsync, DeleteAsync, SetDefaultAsync | IAddressRepository | AddressesController |
| MeService | Application | Aggregated bootstrap payload | GetBootstrapAsync | IMeRepository | MeController |
| JwtTokenService | Infrastructure | JWT and refresh token generation/hash | CreateAccessToken, CreateRefreshToken, HashRefreshToken | JwtOptions | AuthService |
| BcryptPasswordHasher | Infrastructure | Password hashing and verification | Hash, Verify | BCrypt library | AuthService |

---

## 10. Repositories Inventory

| Interface | Implementation | Entity Focus | Major Responsibilities |
|---|---|---|---|
| IAuthRepository | AuthRepository | User, RefreshToken | user uniqueness checks, user reads, refresh token upsert/lookup/revoke |
| ICatalogRepository | CatalogRepository | Category, SubCategory, Product | category/subcategory/product reads, search |
| ICartRepository | CartRepository | CartItem, Product | cart read, upsert, remove, clear, stock validation |
| IOrderRepository | OrderRepository | Order, OrderItem, CartItem, Product | place order transaction, list/detail, cancel |
| IAddressRepository | AddressRepository | UserAddress | CRUD + default management |
| IMeRepository | MeRepository | User, UserAddress, CartItem, Order, OrderItem | aggregate bootstrap response |

---

## 11. Validation

### FluentValidation Rules
- RegisterRequestValidator and LoginRequestValidator only

### Additional Manual Validation (Controller/Repository/Service)
- Token subject parsing to Guid for authenticated flows
- Query parameter checks (categoryId, search q)
- Refresh token required check
- Cart business rules:
  - Product must exist
  - Stock sufficiency
  - Quantity behavior (increment/decrement/remove)
- Order placement checks:
  - Cart non-empty
  - Product stock available
  - Address ownership when AddressId provided

Gap noted:
- Address DTOs and order/cart DTO inputs currently do not have FluentValidation validators.

---

## 12. Middleware and Pipeline

## 12.1 Custom Middleware/Handlers
- ApiExceptionHandler (IExceptionHandler)
  - Converts ValidationException to structured 400 response
  - Logs unhandled exceptions and returns structured 500 response

## 12.2 Pipeline Components in Program
- Serilog request logging
- Exception handler + problem details
- Forwarded headers support
- Swagger and Swagger UI
- HTTPS redirection
- Authentication
- Authorization
- Controller endpoint mapping

---

## 13. Dependency Inventory (NuGet)

| Package | Project | Purpose | Location Used |
|---|---|---|---|
| Microsoft.AspNetCore.Authentication.JwtBearer 9.0.1 | Api | JWT bearer auth middleware | Program.cs |
| Microsoft.AspNetCore.OpenApi 9.0.1 | Api | OpenAPI endpoint support | Program.cs |
| FluentValidation.AspNetCore 11.3.0 | Api | ASP.NET integration for validators | Program.cs, AuthController |
| Swashbuckle.AspNetCore 6.5.0 | Api | Swagger UI and OpenAPI generation | Program.cs |
| Serilog.AspNetCore 8.0.3 | Api | Request/application logging | Program.cs |
| Serilog.Sinks.Console 6.0.0 | Api | Console log sink | Program.cs via config |
| FluentValidation 11.11.0 | Application | Validator definitions | Auth validators |
| Microsoft.EntityFrameworkCore 9.0.1 | Infrastructure | ORM base functionality | AppDbContext, repositories |
| Microsoft.EntityFrameworkCore.Sqlite 9.0.1 | Infrastructure | SQLite provider (declared, currently not wired) | csproj only |
| Microsoft.EntityFrameworkCore.Design 9.0.1 | Infrastructure | Design-time tooling | csproj only |
| Npgsql.EntityFrameworkCore.PostgreSQL 9.0.1 | Infrastructure | PostgreSQL EF provider | DependencyInjection.cs, search query ILike |
| Microsoft.Extensions.Options.ConfigurationExtensions 9.0.1 | Infrastructure | Bind options from configuration | DependencyInjection.cs |
| Microsoft.Extensions.Options.DataAnnotations 9.0.1 | Infrastructure | Validate options with attributes | DependencyInjection.cs, JwtOptions |
| System.IdentityModel.Tokens.Jwt 8.6.0 | Infrastructure | JWT token construction | JwtTokenService.cs |
| BCrypt.Net-Next 4.0.3 | Infrastructure | Password hashing/verification | BcryptPasswordHasher.cs |

---

## 14. Security Review

## 14.1 JWT Security
Strengths:
- Issuer, audience, lifetime, signing key validation enabled
- Symmetric signing with HMAC SHA-256
- Startup fails when signing key is missing
- Token contains unique JTI

Risks/Observations:
- No explicit clock skew configuration shown (default behavior applies)
- No token revocation list for access tokens (common in JWT stateless setups)

## 14.2 Password Handling
Strengths:
- Passwords stored only as BCrypt hash
- Verification performed via BCrypt library

Risks/Observations:
- Minimum password policy currently only length >= 6 via validator

## 14.3 Refresh Token Handling
Strengths:
- Raw refresh token not stored; SHA-256 hash stored
- Rotation on refresh implemented (old revoked, new issued)

Risks/Observations:
- Refresh endpoint requires valid access token and refresh token together; if access token expires, refresh may fail depending on client strategy
- Per-device/session management is basic (single insert strategy)

## 14.4 Sensitive Data Risks
- Development config contains a placeholder DB password and dev signing key pattern
- Ensure production secrets are always environment-managed

## 14.5 Security Recommendations
1. Add stronger password policy (length + complexity or passphrase policy).
2. Consider allowing refresh using only refresh token (with separate secure flow) if desired by product UX.
3. Add structured security audit logs for auth failures and refresh events.
4. Add rate limiting on auth endpoints.
5. Add key rotation strategy and secret management policy.

---

## 15. Known Issues, Technical Debt, and Risks

1. No EF migrations in source
   - Risk: schema drift and hard-to-track DB evolution.
2. Uses EnsureCreatedAsync at startup
   - Risk: not suitable for controlled production migration pipelines.
3. Startup migration block swallows exceptions silently
   - Risk: boot failures may become opaque.
4. Limited FluentValidation coverage
   - Risk: inconsistent validation across endpoint inputs.
5. Mixed architectural style (service/repository instead of explicit command/query handlers)
   - Risk: terminology mismatch for teams expecting full CQRS.
6. Refresh endpoint auth coupling
   - Risk: token refresh UX constraints if access token is expired.

No TODO/FIXME markers were found in source files.

---

## 16. Change Impact Map

## 16.1 If changing User entity
Likely impacted:
- Domain: User.cs
- Infrastructure: AppDbContext.cs, AuthRepository.cs, MeRepository.cs
- Application: Auth DTOs/services, Me DTOs/services
- API: AuthController.cs, MeController.cs

## 16.2 If changing JWT behavior/options
Likely impacted:
- Infrastructure: JwtTokenService.cs, JwtOptions section in same file
- API: Program.cs (token validation params)
- Application: IJwtTokenService.cs, AuthService.cs
- Config: appsettings.json, appsettings.Development.json

## 16.3 If changing DbContext schema mapping
Likely impacted:
- Infrastructure: AppDbContext.cs
- All repositories that query mapped columns
- Deployment/schema scripts and environment DB setup

## 16.4 If changing product APIs/model
Likely impacted:
- Domain: Product.cs, Category.cs, SubCategory.cs
- Infrastructure: AppDbContext.cs, CatalogRepository.cs, CartRepository.cs, OrderRepository.cs, MeRepository.cs
- Application: CatalogDtos.cs, CartDtos.cs, OrderDtos.cs, BootstrapDtos.cs
- API: CatalogController.cs, CartController.cs, OrdersController.cs

## 16.5 If changing address model
Likely impacted:
- Domain: UserAddress.cs, Order.cs (AddressId link)
- Infrastructure: AppDbContext.cs, AddressRepository.cs, OrderRepository.cs, MeRepository.cs
- Application: AddressDtos.cs, AddressService.cs, PlaceOrderRequest
- API: AddressesController.cs, OrdersController.cs

---

## 17. Project Health Summary

| Metric | Count |
|---|---|
| Total Projects | 4 |
| Total Controllers | 7 |
| Total Endpoints | 21 |
| Total Entities | 10 |
| Total Services | 8 (6 application + 2 security implementations) |
| Total Repositories | 6 |
| Total Validators | 2 |
| Total Middleware | 1 custom global exception handler |

## 17.1 Architecture Assessment
- Layering: Good foundational separation
- Dependency direction: Mostly aligned with Clean Architecture
- Contract clarity: Good (DTO and interface boundaries present)

## 17.2 Maintainability Assessment
- Moderate to good
- Strengths: clear modules, explicit mappings, consistent response envelope
- Constraints: limited validation coverage, no migration history, some silent failure handling

## 17.3 Scalability Assessment
- Moderate
- Strengths: stateless API, repository abstractions, JWT-based auth
- Constraints: no caching, no explicit pagination on most list endpoints, minimal session/device controls

## 17.4 Production Readiness Assessment
- Functional baseline ready for controlled environments
- Before broad production use, prioritize:
  1. Migration strategy and CI/CD schema control
  2. Expanded validation and security hardening
  3. Operational observability and failure surfacing improvements

---

## 18. Quick Orientation for Different Audiences

### New Developers
Start in Program.cs, then follow controllers -> application services -> repositories -> AppDbContext/entity mappings.

### Architects
Review DependencyInjection, project references, and DbContext mappings to validate boundary and persistence strategy.

### AI Assistants
Primary impact hubs:
- AppDbContext for schema
- AuthService + JwtTokenService for auth
- OrderRepository for transaction/business-critical flow

### Recruiters
Project demonstrates:
- Multi-layer backend architecture
- Auth/token lifecycle handling
- EF Core relational modeling
- Real-world commerce workflows and API contract discipline
