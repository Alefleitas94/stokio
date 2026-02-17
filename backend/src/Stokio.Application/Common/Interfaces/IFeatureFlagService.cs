namespace Stokio.Application.Common.Interfaces;

public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(string moduleKey, CancellationToken cancellationToken = default);
    Task<bool> IsEnabledAsync(int tenantId, string moduleKey, CancellationToken cancellationToken = default);
}
