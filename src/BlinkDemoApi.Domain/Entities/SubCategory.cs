namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// Sub-category under a Category (your RN model: { id, categoryId, name }).
/// </summary>
public class SubCategory : BaseEntity
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;

    public Category? Category { get; set; }

    // ✅ NEW (recommended)
    public ICollection<Product> Products { get; set; } = new List<Product>();
}