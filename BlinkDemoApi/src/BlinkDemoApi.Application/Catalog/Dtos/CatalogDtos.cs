using System;

namespace BlinkDemoApi.Application.Catalog.Dtos;

/// <summary>
/// Category DTO returned to client.
/// </summary>
public sealed record CategoryDto(
    Guid Id,
    string Name,
    string? IconUrl
);

/// <summary>
/// SubCategory DTO (maps to your RN model).
/// </summary>
public sealed record SubCategoryDto(
    Guid Id,
    Guid CategoryId,
    string Name
);

/// <summary>
/// Product DTO returned to client.
/// NOW includes SubCategoryId since the Product table contains it.
/// </summary>
public sealed record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    decimal Mrp,
    string? ImageUrl,
    Guid CategoryId,
    Guid SubcategoryId,   // ✅ NEW (important)
    int StockQty,
    string Unit
);