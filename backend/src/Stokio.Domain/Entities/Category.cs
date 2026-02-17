using Stokio.Domain.Common;

namespace Stokio.Domain.Entities;

public class Category : BaseEntity, ITenantEntity
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
