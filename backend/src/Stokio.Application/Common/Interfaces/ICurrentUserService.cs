namespace Stokio.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    int? TenantId { get; }
    string? Email { get; }
}
