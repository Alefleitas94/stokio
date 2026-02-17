using Stokio.Application.Common.Interfaces;
using Stokio.Domain.Entities;

namespace Stokio.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    public int? TenantId { get; private set; }
    public Tenant? Tenant { get; private set; }

    public void SetTenant(Tenant tenant)
    {
        Tenant = tenant;
        TenantId = tenant.Id;
    }
}
