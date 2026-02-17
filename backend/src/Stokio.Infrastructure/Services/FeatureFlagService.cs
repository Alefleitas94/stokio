using Microsoft.EntityFrameworkCore;
using Stokio.Application.Common.Interfaces;
using Stokio.Infrastructure.Persistence;

namespace Stokio.Infrastructure.Services;

public class FeatureFlagService : IFeatureFlagService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentTenantService _currentTenantService;

    public FeatureFlagService(ApplicationDbContext dbContext, ICurrentTenantService currentTenantService)
    {
        _dbContext = dbContext;
        _currentTenantService = currentTenantService;
    }

    public Task<bool> IsEnabledAsync(string moduleKey, CancellationToken cancellationToken = default)
    {
        var tenantId = _currentTenantService.TenantId;
        return tenantId == null
            ? Task.FromResult(false)
            : IsEnabledAsync(tenantId.Value, moduleKey, cancellationToken);
    }

    public async Task<bool> IsEnabledAsync(int tenantId, string moduleKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(moduleKey))
        {
            return false;
        }

        moduleKey = moduleKey.Trim();

        return await (from tm in _dbContext.TenantModules.AsNoTracking()
            join m in _dbContext.Modules.AsNoTracking() on tm.ModuleId equals m.Id
            where tm.TenantId == tenantId
                  && tm.IsEnabled
                  && m.IsActive
                  && m.Key == moduleKey
            select tm.Id).AnyAsync(cancellationToken);
    }
}
