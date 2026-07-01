# BlinkDemoApi - API Collection

Source of truth: controller routes, DTOs, validators, services, repositories, and middleware from the current solution source.

## API Overview

### Base URL
Development profile URLs:
- https://localhost:7261
- http://localhost:5261

Primary API prefix:
- /api

### Authentication Type
- JWT Bearer Authentication
- Access token required for all protected endpoints

### Response Format
All controller responses use a common envelope from ApiResponse<T>.

~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Success",
  "ErrorCode": null,
  "TraceId": "0HN...",
  "Data": {}
}
~~~

### Error Format
~~~json
{
  "Success": false,
  "StatusCode": 400,
  "Message": "Validation failed.",
  "ErrorCode": "VALIDATION_FAILED",
  "TraceId": "0HN...",
  "Data": {
    "errors": {
      "Mobile": ["Mobile must be 10 digits."]
    }
  }
}
~~~

For non-validation failures, Data may be null.

---

## Authentication

### JWT Flow
~~~mermaid
flowchart TD
  A[Register or Login] --> B[Receive AccessToken and RefreshToken]
  B --> C[Call protected APIs using Bearer AccessToken]
  C --> D[When needed, call /api/auth/refresh with RefreshToken]
  D --> E[Receive rotated AccessToken and RefreshToken]
~~~

### Access Token
- Contains user id in sub claim
- Also includes email, mobile, jti
- Used to authorize protected endpoints

### Refresh Token
- Random 64-byte token returned to client
- Only SHA-256 hash is stored in DB
- Rotated on successful refresh

### Authorization Header Format
Authorization: Bearer <token>

Example:
~~~http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
~~~

---

## Standard Response Format

### Success Envelope
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Categories loaded.",
  "ErrorCode": null,
  "TraceId": "00-abc123...",
  "Data": []
}
~~~

### Error Envelope
~~~json
{
  "Success": false,
  "StatusCode": 401,
  "Message": "Invalid token.",
  "ErrorCode": "UNAUTHORIZED",
  "TraceId": "00-def456...",
  "Data": null
}
~~~

### Validation Error Envelope
~~~json
{
  "Success": false,
  "StatusCode": 400,
  "Message": "Validation failed.",
  "ErrorCode": "VALIDATION_FAILED",
  "TraceId": "00-ghi789...",
  "Data": {
    "errors": {
      "Email": ["'Email' is not a valid email address."],
      "Password": ["'Password' must be at least 6 characters."]
    }
  }
}
~~~

---

## Auth APIs

### Register User
Route: /api/auth/register  
Method: POST  
Authorization Required: No

Description:
Creates a user after validation and duplicate checks, then returns access and refresh tokens.

Request:
~~~json
{
  "Name": "Rahul Tiwari",
  "Mobile": "9876543210",
  "Email": "rahul@example.com",
  "Password": "secret123"
}
~~~

Sample Success Response:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Registration successful.",
  "ErrorCode": null,
  "TraceId": "00-...",
  "Data": {
    "UserId": "8b58e6a0-8bdb-4c2b-a2ab-cc9c1f3c2d11",
    "Name": "Rahul Tiwari",
    "Mobile": "9876543210",
    "Email": "rahul@example.com",
    "AccessToken": "eyJ...",
    "AccessTokenExpiresAt": "2026-07-01T10:20:00Z",
    "RefreshToken": "8f3...",
    "RefreshTokenExpiresAt": "2026-07-31T09:20:00Z"
  }
}
~~~

Validation Rules:
- Name required, max length 100
- Mobile required, exactly 10 digits
- Email required, valid email
- Password required, minimum length 6
- Mobile must be unique
- Email must be unique

Possible Error Responses:
- 400 VALIDATION_FAILED
- 409 DUPLICATE_MOBILE
- 409 DUPLICATE_EMAIL

---

### Login User
Route: /api/auth/login  
Method: POST  
Authorization Required: No

Description:
Authenticates by mobile and password, then issues access and refresh tokens.

Request:
~~~json
{
  "Mobile": "9876543210",
  "Password": "secret123"
}
~~~

Sample Success Response:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Login successful.",
  "ErrorCode": null,
  "TraceId": "00-...",
  "Data": {
    "UserId": "8b58e6a0-8bdb-4c2b-a2ab-cc9c1f3c2d11",
    "Name": "Rahul Tiwari",
    "Mobile": "9876543210",
    "Email": "rahul@example.com",
    "AccessToken": "eyJ...",
    "AccessTokenExpiresAt": "2026-07-01T10:20:00Z",
    "RefreshToken": "8f3...",
    "RefreshTokenExpiresAt": "2026-07-31T09:20:00Z"
  }
}
~~~

Validation Rules:
- Mobile required, exactly 10 digits
- Password required
- Credentials must match stored user/password hash

Possible Error Responses:
- 400 VALIDATION_FAILED
- 401 INVALID_CREDENTIALS

---

### Refresh Token
Route: /api/auth/refresh  
Method: POST  
Authorization Required: Yes

Description:
Rotates refresh token and issues a new access token.

Request:
~~~json
{
  "RefreshToken": "current-refresh-token"
}
~~~

Sample Success Response:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Token refreshed.",
  "ErrorCode": null,
  "TraceId": "00-...",
  "Data": {
    "UserId": "8b58e6a0-8bdb-4c2b-a2ab-cc9c1f3c2d11",
    "Name": "Rahul Tiwari",
    "Mobile": "9876543210",
    "Email": "rahul@example.com",
    "AccessToken": "eyJ-new...",
    "AccessTokenExpiresAt": "2026-07-01T11:20:00Z",
    "RefreshToken": "new-refresh-token",
    "RefreshTokenExpiresAt": "2026-07-31T10:20:00Z"
  }
}
~~~

Validation Rules:
- Valid bearer token required
- sub claim must parse to Guid
- RefreshToken must be non-empty
- Refresh token hash must exist, be active, and not expired

Possible Error Responses:
- 400 VALIDATION_FAILED
- 401 UNAUTHORIZED

---

## Catalog APIs

### Get Categories
Route: /api/catalog/categories  
Method: GET  
Authorization Required: Yes

Purpose:
Loads top-level categories.

Query Parameters:
- None

Request Example:
~~~http
GET /api/catalog/categories
~~~

Response Example:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Categories loaded.",
  "TraceId": "00-...",
  "Data": [
    {
      "Id": "0ec2ef8f-53fd-4f6b-ab9d-e907c84a9adf",
      "Name": "Fruits",
      "IconUrl": "https://cdn/icons/fruits.png"
    }
  ]
}
~~~

Validation Rules:
- Auth required

---

### Get Subcategories
Route: /api/catalog/subcategories  
Method: GET  
Authorization Required: Yes

Purpose:
Loads subcategories for a selected category.

Query Parameters:
- categoryId: Guid, required

Request Example:
~~~http
GET /api/catalog/subcategories?categoryId=0ec2ef8f-53fd-4f6b-ab9d-e907c84a9adf
~~~

Response Example:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Subcategories loaded.",
  "TraceId": "00-...",
  "Data": [
    {
      "Id": "d9488f5d-3f45-4976-8df3-4dceeb5ce1a1",
      "CategoryId": "0ec2ef8f-53fd-4f6b-ab9d-e907c84a9adf",
      "Name": "Citrus"
    }
  ]
}
~~~

Validation Rules:
- categoryId must be non-empty Guid

---

### Get Products
Route: /api/catalog/products  
Method: GET  
Authorization Required: Yes

Purpose:
Loads products, optionally filtered by category.

Query Parameters:
- categoryId: Guid, optional

Request Examples:
~~~http
GET /api/catalog/products
GET /api/catalog/products?categoryId=0ec2ef8f-53fd-4f6b-ab9d-e907c84a9adf
~~~

Response Example:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Products loaded.",
  "TraceId": "00-...",
  "Data": [
    {
      "Id": "f40e41ad-9345-4709-a23b-dfcadf7d6f6d",
      "Name": "Orange",
      "Description": "Fresh oranges",
      "Price": 80.0,
      "Mrp": 100.0,
      "ImageUrl": "https://cdn/products/orange.png",
      "CategoryId": "0ec2ef8f-53fd-4f6b-ab9d-e907c84a9adf",
      "SubcategoryId": "d9488f5d-3f45-4976-8df3-4dceeb5ce1a1",
      "StockQty": 120,
      "Unit": "kg"
    }
  ]
}
~~~

Validation Rules:
- Auth required

---

### Search Products
Route: /api/catalog/search  
Method: GET  
Authorization Required: Yes

Purpose:
Searches by product name or description using case-insensitive DB search.

Query Parameters:
- q: string, required

Request Example:
~~~http
GET /api/catalog/search?q=orange
~~~

Response Example:
- Same ProductDto list as products endpoint

Validation Rules:
- q must be non-empty

Possible Error Responses:
- 400 VALIDATION_FAILED

---

## Cart APIs

### Get My Cart
Route: /api/cart  
Method: GET  
Authorization Required: Yes

Business Purpose:
Fetch current user cart with product details.

Request Payload:
- None

Success Response:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Cart loaded.",
  "TraceId": "00-...",
  "Data": [
    {
      "Id": "8bfa9d44-7e31-4b56-8ee8-ed2e6da865cd",
      "ProductId": "f40e41ad-9345-4709-a23b-dfcadf7d6f6d",
      "ProductName": "Orange",
      "ImageUrl": "https://cdn/products/orange.png",
      "Price": 80.0,
      "Quantity": 2,
      "Unit": "kg"
    }
  ]
}
~~~

Error Scenarios:
- 401 UNAUTHORIZED for invalid token subject

Validation Rules:
- JWT sub must parse to Guid

---

### Upsert Cart Item
Route: /api/cart/upsert  
Method: POST  
Authorization Required: Yes

Business Purpose:
Add/increment/decrement/remove an item from cart based on quantity delta.

Request Payload:
~~~json
{
  "ProductId": "f40e41ad-9345-4709-a23b-dfcadf7d6f6d",
  "Quantity": 1
}
~~~

Business behavior:
- If no existing row and Quantity > 0: creates cart row
- If existing row: new quantity = current + Quantity
- If new quantity <= 0: removes row

Success Response:
- Returns full updated cart list

Error Scenarios:
- 401 UNAUTHORIZED
- 409 CONFLICT when product missing, invalid quantity for create, or insufficient stock

Validation Rules:
- ProductId must be non-empty and exist
- Quantity logic enforced by repository
- Stock must be sufficient

---

### Remove Cart Item
Route: /api/cart/{productId}  
Method: DELETE  
Authorization Required: Yes

Business Purpose:
Remove one product from cart by product id.

Request Example:
~~~http
DELETE /api/cart/f40e41ad-9345-4709-a23b-dfcadf7d6f6d
~~~

Success Response:
- Returns updated cart list

Error Scenarios:
- 404 NOT_FOUND if cart item does not exist
- 401 UNAUTHORIZED if token invalid

Validation Rules:
- productId must match existing user cart row

---

### Clear Cart
Route: /api/cart/clear  
Method: POST  
Authorization Required: Yes

Business Purpose:
Remove all cart rows for current user.

Success Response:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Cart cleared.",
  "Data": { "cleared": true }
}
~~~

Error Scenarios:
- 401 UNAUTHORIZED

Validation Rules:
- JWT subject validation

---

## Order APIs

### Order Lifecycle
OrderStatus enum values:
- PLACED
- CONFIRMED
- DELIVERED
- CANCELLED

State transition currently enforced in source:
- PLACED -> CANCELLED allowed
- CONFIRMED -> CANCELLED allowed
- DELIVERED -> CANCELLED blocked
- CANCELLED -> CANCELLED idempotent success

~~~mermaid
stateDiagram-v2
  [*] --> PLACED
  PLACED --> CANCELLED
  CONFIRMED --> CANCELLED
  DELIVERED --> DELIVERED: cancel blocked
  CANCELLED --> CANCELLED: idempotent cancel
~~~

### Get My Orders
Route: /api/orders  
Method: GET  
Authorization Required: Yes

Description:
Returns up to 50 most recent orders including order items.

Success Response:
- Data: OrderDto[]

---

### Get Order Detail
Route: /api/orders/{id}  
Method: GET  
Authorization Required: Yes

Description:
Returns one order detail if owned by current user.

Error Scenarios:
- 404 NOT_FOUND when not found/unauthorized ownership
- 401 UNAUTHORIZED for invalid token

---

### Place Order
Route: /api/orders/place  
Method: POST  
Authorization Required: Yes

Description:
Creates order from current cart in transaction, validates stock, optionally validates AddressId ownership, decrements stock, clears cart.

Request Payload:
~~~json
{
  "AddressId": "35f95cc8-5742-42ff-ac14-a7d50c3f9f41"
}
~~~

Success Response:
- Data: created OrderDto

Error Scenarios:
- 409 CONFLICT
  - Cart is empty
  - Cart contains invalid product
  - Not enough stock for product
  - Invalid or unauthorized address

Validation Rules:
- User must have cart items
- Each cart item product must exist
- Each cart item quantity <= stock
- If AddressId provided, it must belong to same user

---

### Cancel Order
Route: /api/orders/{id}/cancel  
Method: POST  
Authorization Required: Yes

Description:
Cancels user order unless already delivered.

Success Response:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Order cancelled.",
  "Data": { "cancelled": true }
}
~~~

Error Scenarios:
- 409 CONFLICT for delivered order or cancellation failure
- 401 UNAUTHORIZED for invalid token

Validation Rules:
- Order must belong to user
- Delivered orders cannot be cancelled

---

## Address APIs

### Get My Addresses
Route: /api/addresses  
Method: GET  
Authorization Required: Yes

Purpose:
Fetch all addresses for current user (default first).

Response Data:
- AddressDto[]

---

### Create Address
Route: /api/addresses  
Method: POST  
Authorization Required: Yes

Request:
~~~json
{
  "Label": "Home",
  "AddressLine": "221B Baker Street",
  "Pincode": "560001",
  "City": "Bengaluru",
  "Lat": 12.9716,
  "Lng": 77.5946,
  "IsDefault": true
}
~~~

Business Notes:
- If IsDefault true, previous defaults for user are unset.

Response Data:
- AddressDto

---

### Update Address
Route: /api/addresses/{id}  
Method: PUT  
Authorization Required: Yes

Request:
- Same shape as UpdateAddressRequest

Business Rules:
- Address must belong to current user
- If IsDefault true, other defaults are unset

Errors:
- 404 NOT_FOUND when address not found for user

---

### Delete Address
Route: /api/addresses/{id}  
Method: DELETE  
Authorization Required: Yes

Business Rules:
- Address must belong to current user

Success:
- Data: { deleted: true }

Errors:
- 404 NOT_FOUND when missing/not owned

---

### Set Default Address
Route: /api/addresses/{id}/set-default  
Method: POST  
Authorization Required: Yes

Business Rules:
- Address must belong to current user
- All other defaults are unset

Success:
- Data: { isDefaultSet: true }

Errors:
- 404 NOT_FOUND when missing/not owned

---

## Me APIs

### Bootstrap Current User
Route: /api/me/bootstrap  
Method: GET  
Authorization Required: Yes

Why it exists:
- Reduces frontend startup round trips
- Returns initial personalized app state in one call

Included payloads:
- UserSummaryDto
- AddressDto[]
- CartItemDto[]
- Recent OrderDto[] (top 10)

Frontend use cases:
- App launch after login
- Home screen hydration
- Local cache warm-up
- Sync user/cart/order snapshot

Sample Success Response:
~~~json
{
  "Success": true,
  "StatusCode": 200,
  "Message": "Bootstrap loaded.",
  "TraceId": "00-...",
  "Data": {
    "User": {
      "Id": "8b58e6a0-8bdb-4c2b-a2ab-cc9c1f3c2d11",
      "Name": "Rahul Tiwari",
      "Mobile": "9876543210",
      "Email": "rahul@example.com"
    },
    "Addresses": [],
    "Cart": [],
    "RecentOrders": []
  }
}
~~~

---

## Data Models (API-facing DTOs)

## Auth DTOs

### RegisterRequest
| Property | Type | Purpose |
|---|---|---|
| Name | string | Full name |
| Mobile | string | Login mobile number |
| Email | string | Unique email |
| Password | string | Plain password sent by client |

### LoginRequest
| Property | Type | Purpose |
|---|---|---|
| Mobile | string | Login mobile |
| Password | string | Login password |

### RefreshRequest
| Property | Type | Purpose |
|---|---|---|
| RefreshToken | string | Refresh token value from client storage |

### AuthResponse
| Property | Type | Purpose |
|---|---|---|
| UserId | Guid | Authenticated user id |
| Name | string | Display name |
| Mobile | string | User mobile |
| Email | string | User email |
| AccessToken | string | JWT bearer token |
| AccessTokenExpiresAt | DateTime | Access token expiry |
| RefreshToken | string | New refresh token |
| RefreshTokenExpiresAt | DateTime | Refresh expiry |

## Catalog DTOs

### CategoryDto
| Property | Type | Purpose |
|---|---|---|
| Id | Guid | Category id |
| Name | string | Category name |
| IconUrl | string? | Optional icon |

### SubCategoryDto
| Property | Type | Purpose |
|---|---|---|
| Id | Guid | Subcategory id |
| CategoryId | Guid | Parent category |
| Name | string | Subcategory name |

### ProductDto
| Property | Type | Purpose |
|---|---|---|
| Id | Guid | Product id |
| Name | string | Product name |
| Description | string? | Product description |
| Price | decimal | Selling price |
| Mrp | decimal | MRP |
| ImageUrl | string? | Product image |
| CategoryId | Guid | Category reference |
| SubcategoryId | Guid | Subcategory reference |
| StockQty | int | Available stock |
| Unit | string | Unit label (kg, pcs, etc.) |

## Cart DTOs

### UpsertCartItemRequest
| Property | Type | Purpose |
|---|---|---|
| ProductId | Guid | Target product |
| Quantity | int | Quantity delta |

### CartItemDto
| Property | Type | Purpose |
|---|---|---|
| Id | Guid | Cart row id |
| ProductId | Guid | Product id |
| ProductName | string | Product display name |
| ImageUrl | string? | Product image |
| Price | decimal | Product price |
| Quantity | int | Selected quantity |
| Unit | string | Product unit |

## Order DTOs

### PlaceOrderRequest
| Property | Type | Purpose |
|---|---|---|
| AddressId | Guid? | Optional selected address id |

### OrderItemDto
| Property | Type | Purpose |
|---|---|---|
| Id | Guid | Order item id |
| ProductId | Guid | Product reference |
| ProductName | string | Product name snapshot |
| Quantity | int | Quantity purchased |
| PriceAtPurchase | decimal | Final item price at order time |
| Subtotal | decimal | Quantity x price |

### OrderDto
| Property | Type | Purpose |
|---|---|---|
| Id | Guid | Order id |
| TotalAmount | decimal | Final order amount |
| Status | OrderStatus | PLACED, CONFIRMED, DELIVERED, CANCELLED |
| CreatedAt | DateTime | Order creation time |
| Items | List<OrderItemDto> | Purchased lines |

## Address DTOs

### CreateAddressRequest and UpdateAddressRequest
| Property | Type | Purpose |
|---|---|---|
| Label | string | Address label (Home/Office) |
| AddressLine | string | Full street address |
| Pincode | string | Postal code |
| City | string | City |
| Lat | decimal? | Optional latitude |
| Lng | decimal? | Optional longitude |
| IsDefault | bool | Marks as default |

### AddressDto
| Property | Type | Purpose |
|---|---|---|
| Id | Guid | Address id |
| Label | string | Address label |
| AddressLine | string | Address line |
| Pincode | string | Postal code |
| City | string | City |
| Lat | decimal? | Latitude |
| Lng | decimal? | Longitude |
| IsDefault | bool | Default indicator |

## Bootstrap DTOs

### UserSummaryDto
| Property | Type | Purpose |
|---|---|---|
| Id | Guid | User id |
| Name | string | User name |
| Mobile | string | Mobile number |
| Email | string | Email |

### BootstrapResponse
| Property | Type | Purpose |
|---|---|---|
| User | UserSummaryDto | Current user profile |
| Addresses | List<AddressDto> | Saved addresses |
| Cart | List<CartItemDto> | Current cart |
| RecentOrders | List<OrderDto> | Latest orders |

Note:
AddressDto, CartItemDto, OrderItemDto, and OrderDto also exist in the Me namespace for bootstrap response shape.

---

## Validation Rules

### FluentValidation (explicit)

RegisterRequest:
- Name: required
- Name: max length 100
- Mobile: required
- Mobile: regex 10 digits
- Email: required
- Email: valid format
- Password: required
- Password: min length 6

LoginRequest:
- Mobile: required
- Mobile: regex 10 digits
- Password: required

### Manual validations in endpoints/services/repositories
- Token subject must parse to Guid for protected operations
- RefreshToken must be present for refresh
- categoryId query required and non-empty for subcategories endpoint
- q query required for search endpoint
- Product existence and stock checks in cart/order operations
- Address ownership checks in order placement and address updates

---

## Business Rules

### Registration Rules
- Duplicate mobile blocked
- Duplicate email blocked
- Password stored only as hash

### Login Rules
- Auth by mobile + password
- Invalid mobile/password returns unauthorized

### Cart Rules
- Product must exist
- Create requires positive quantity
- Existing item updates by quantity delta
- Quantity <= 0 removes row
- Cannot exceed product stock

### Order Rules
- Cart must not be empty
- Product in cart must remain valid
- Stock must be enough for each item
- AddressId optional, but if provided must belong to user
- Place order uses DB transaction
- Cancelling delivered order is forbidden

### Address Rules
- User can only read/update/delete/set-default for own addresses
- Setting new default unsets prior default addresses

### Product Rules
- Product search uses case-insensitive matching on name/description
- Product DTO includes CategoryId and SubcategoryId

---

## Error Codes

| ErrorCode | Typical HTTP Status | Meaning | Typical Cause |
|---|---|---|---|
| VALIDATION_FAILED | 400 | Request validation failed | FluentValidation or query/body checks |
| DUPLICATE_MOBILE | 409 | Mobile already exists | Register with existing mobile |
| DUPLICATE_EMAIL | 409 | Email already exists | Register with existing email |
| INVALID_CREDENTIALS | 401 | Login credentials invalid | Wrong mobile/password |
| UNAUTHORIZED | 401 | Auth token invalid/unauthorized | Missing or invalid JWT subject/token |
| NOT_FOUND | 404 | Resource not found | Order/address/cart item not found for user |
| CONFLICT | 409 | Business conflict | Stock, cart, or cancel constraints |
| SERVER_ERROR | 500 | Unexpected backend error | Unhandled exception |

---

## Authorization Matrix

| Endpoint | Anonymous | Authenticated |
|---|---:|---:|
| POST /api/auth/register | Yes | No |
| POST /api/auth/login | Yes | No |
| POST /api/auth/refresh | No | Yes |
| GET /api/catalog/categories | No | Yes |
| GET /api/catalog/subcategories | No | Yes |
| GET /api/catalog/products | No | Yes |
| GET /api/catalog/search | No | Yes |
| GET /api/cart | No | Yes |
| POST /api/cart/upsert | No | Yes |
| DELETE /api/cart/{productId} | No | Yes |
| POST /api/cart/clear | No | Yes |
| GET /api/orders | No | Yes |
| GET /api/orders/{id} | No | Yes |
| POST /api/orders/place | No | Yes |
| POST /api/orders/{id}/cancel | No | Yes |
| GET /api/addresses | No | Yes |
| POST /api/addresses | No | Yes |
| PUT /api/addresses/{id} | No | Yes |
| DELETE /api/addresses/{id} | No | Yes |
| POST /api/addresses/{id}/set-default | No | Yes |
| GET /api/me/bootstrap | No | Yes |

---

## Postman Section

### Environment Variables
- baseUrl = https://localhost:7261
- accessToken = empty initially
- refreshToken = empty initially

### Suggested Execution Order
1. Register
   - POST {{baseUrl}}/api/auth/register
2. Login
   - POST {{baseUrl}}/api/auth/login
3. Store tokens
   - Save Data.AccessToken to accessToken
   - Save Data.RefreshToken to refreshToken
4. Call protected APIs with bearer token
   - GET {{baseUrl}}/api/me/bootstrap
   - GET {{baseUrl}}/api/catalog/categories
   - GET {{baseUrl}}/api/cart
5. Create address and set default
   - POST {{baseUrl}}/api/addresses
   - POST {{baseUrl}}/api/addresses/{id}/set-default
6. Add cart item and place order
   - POST {{baseUrl}}/api/cart/upsert
   - POST {{baseUrl}}/api/orders/place
7. Refresh token
   - POST {{baseUrl}}/api/auth/refresh
   - Authorization: Bearer {{accessToken}}
   - Body: { "RefreshToken": "{{refreshToken}}" }

### Header Setup for Protected Calls
- Authorization: Bearer {{accessToken}}

---

## Frontend Integration Guide

Recommended client flow:

~~~mermaid
flowchart TD
  A[Register] --> B[Login]
  B --> C[Store Access and Refresh Tokens]
  C --> D[Call /api/me/bootstrap]
  D --> E[Load catalog endpoints]
  E --> F[Manage cart endpoints]
  F --> G[Place order]
  G --> H[Refresh token when needed]
~~~

Implementation notes:
- Persist both tokens securely on device/app storage policy
- Attach access token to every protected API request
- On 401, attempt refresh flow and retry once
- Use TraceId from response for QA and support logs

---

## Project Summary

| Metric | Value |
|---|---|
| Total Endpoints | 21 |
| Public Endpoints | 2 |
| Protected Endpoints | 19 |
| Controllers with Routes | 6 |
| Controller Classes Total | 7 (includes BaseApiController) |
| DTO Types | 21 API-facing record DTOs |
| FluentValidation Rules | 11 explicit rules across 2 validators |
| Authentication Flow | JWT access + hashed refresh token rotation |

Production readiness note for API consumers:
- API contracts are consistent and structured for frontend/mobile/QA automation.
- Refresh endpoint currently requires bearer access token plus refresh token in body.
