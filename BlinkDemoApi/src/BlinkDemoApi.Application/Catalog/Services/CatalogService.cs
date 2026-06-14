using BlinkDemoApi.Application.Catalog.Dtos;

namespace BlinkDemoApi.Application.Catalog.Services;

public sealed class CatalogService
{
    private readonly ICatalogRepository _repo;

    public CatalogService(ICatalogRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct)
        => _repo.GetCategoriesAsync(ct);

    public Task<IReadOnlyList<SubCategoryDto>> GetSubCategoriesAsync(Guid categoryId, CancellationToken ct)
        => _repo.GetSubCategoriesAsync(categoryId, ct);

    public Task<IReadOnlyList<ProductDto>> GetProductsAsync(Guid? categoryId, CancellationToken ct)
        => _repo.GetProductsAsync(categoryId, ct);

    // ✅ NEW: search API
    public Task<IReadOnlyList<ProductDto>> SearchProductsAsync(string q, CancellationToken ct)
        => _repo.SearchProductsAsync(q, ct);
}

public interface ICatalogRepository
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct);
    Task<IReadOnlyList<SubCategoryDto>> GetSubCategoriesAsync(Guid categoryId, CancellationToken ct);
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(Guid? categoryId, CancellationToken ct);

    // ✅ NEW
    Task<IReadOnlyList<ProductDto>> SearchProductsAsync(string q, CancellationToken ct);
}
