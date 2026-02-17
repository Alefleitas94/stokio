using Stokio.Domain.Entities;

namespace Stokio.Application.Common.Interfaces;

public interface ICurrentTenantService
{
    int? TenantId { get; }
    Tenant? Tenant { get; }
}
