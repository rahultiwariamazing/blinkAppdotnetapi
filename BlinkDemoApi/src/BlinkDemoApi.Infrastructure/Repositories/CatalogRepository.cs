using BlinkDemoApi.Application.Catalog.Dtos;
using BlinkDemoApi.Application.Catalog.Services;
using BlinkDemoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlinkDemoApi.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation for catalog read APIs.
/// </summary>
public sealed class CatalogRepository : ICatalogRepository
{
    private readonly AppDbContext _db;

    public CatalogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct)
    {
        return await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.IconUrl
            ))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<SubCategoryDto>> GetSubCategoriesAsync(Guid categoryId, CancellationToken ct)
    {
        return await _db.SubCategories
            .AsNoTracking()
            .Where(sc => sc.CategoryId == categoryId)
            .OrderBy(sc => sc.Name)
            .Select(sc => new SubCategoryDto(
                sc.Id,
                sc.CategoryId,
                sc.Name
            ))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(Guid? categoryId, CancellationToken ct)
    {
        var query = _db.Products.AsNoTracking();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        return await query
            .OrderByDescending(p => p.Id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Mrp,
                p.ImageUrl,
                p.CategoryId,
                p.SubcategoryId,
                p.StockQty,
                p.Unit
            ))
            .ToListAsync(ct);
    }

        // ✅ PostgreSQL: case-insensitive search using ILIKE (no ToLower, keeps indexes where possible)
        public async Task<IReadOnlyList<ProductDto>> SearchProductsAsync(string q, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Array.Empty<ProductDto>();

            q = q.Trim();
            var like = $"%{q}%";

            return await _db.Products
                .AsNoTracking()
                .Where(p =>
                    EF.Functions.ILike(p.Name, like) ||
                    (p.Description != null && EF.Functions.ILike(p.Description, like))
                )
                .OrderBy(p => p.Name)
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.Mrp,
                    p.ImageUrl,
                    p.CategoryId,
                    p.SubcategoryId,
                    p.StockQty,
                    p.Unit
                ))
                .ToListAsync(ct);
        }

}
