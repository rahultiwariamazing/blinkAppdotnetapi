namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// Product category shown in the app.
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }

    public List<Product> Products { get; set; } = new();
}
