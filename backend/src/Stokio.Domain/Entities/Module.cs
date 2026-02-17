using Stokio.Domain.Common;

namespace Stokio.Domain.Entities;

public class Module : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<TenantModule> TenantModules { get; set; } = new List<TenantModule>();
}
