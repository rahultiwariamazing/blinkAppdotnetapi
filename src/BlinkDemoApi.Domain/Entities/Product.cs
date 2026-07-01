namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// Catalog product.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Price { get; set; }
    public decimal Mrp { get; set; }

    public string? ImageUrl { get; set; }

    public Guid CategoryId { get; set; }
    public Guid SubcategoryId { get; set; }   // ✅ NEW

    public int StockQty { get; set; }
    public string Unit { get; set; } = string.Empty;

    public Category? Category { get; set; }
    public SubCategory? Subcategory { get; set; }  // ✅ NEW
}
