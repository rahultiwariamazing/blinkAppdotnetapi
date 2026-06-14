using BlinkDemoApi.Application.Catalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlinkDemoApi.Api.Controllers;

[Route("api/catalog")]
[Authorize]
public sealed class CatalogController : BaseApiController
{
    private readonly CatalogService _catalog;

    public CatalogController(CatalogService catalog)
    {
        _catalog = catalog;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var data = await _catalog.GetCategoriesAsync(ct);
        return OkResponse(data, "Categories loaded.");
    }

    [HttpGet("subcategories")]
    public async Task<IActionResult> GetSubCategories([FromQuery] Guid categoryId, CancellationToken ct)
    {
        if (categoryId == Guid.Empty)
            return Fail("categoryId is required.", StatusCodes.Status400BadRequest, "VALIDATION_FAILED");

        var data = await _catalog.GetSubCategoriesAsync(categoryId, ct);
        return OkResponse(data, "Subcategories loaded.");
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] Guid? categoryId, CancellationToken ct)
    {
        var data = await _catalog.GetProductsAsync(categoryId, ct);
        return OkResponse(data, "Products loaded.");
    }

    // ----------------------------------------------------
    // ✅ NEW: Search API
    // ----------------------------------------------------
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Fail("q is required.", StatusCodes.Status400BadRequest, "VALIDATION_FAILED");

        var data = await _catalog.SearchProductsAsync(q, ct);
        return OkResponse(data, "Search results loaded.");
    }
}