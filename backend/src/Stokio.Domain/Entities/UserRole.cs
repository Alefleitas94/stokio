using Stokio.Domain.Common;

namespace Stokio.Domain.Entities;

public class UserRole : ITenantEntity
{
    public int TenantId { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
