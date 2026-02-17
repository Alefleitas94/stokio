using Stokio.Domain.Common;

namespace Stokio.Domain.Entities;

public enum MovementType
{
    Purchase,
    Sale,
    Transfer,
    Adjustment
}

public class StockMovement : BaseEntity, ITenantEntity
{
    public int TenantId { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public MovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
    public int? RelatedWarehouseId { get; set; }
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public Warehouse? RelatedWarehouse { get; set; }
}
