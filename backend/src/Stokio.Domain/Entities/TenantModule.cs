using Stokio.Domain.Common;

namespace Stokio.Domain.Entities;

public class TenantModule : BaseEntity, ITenantEntity
{
    public int TenantId { get; set; }
    public int ModuleId { get; set; }
    public bool IsEnabled { get; set; } = true;

    public Tenant Tenant { get; set; } = null!;
    public Module Module { get; set; } = null!;
}
