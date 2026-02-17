using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Stokio.Application.Common.Interfaces;

namespace Stokio.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? int.Parse(userIdClaim) : null;
        }
    }

    public int? TenantId
    {
        get
        {
            var tenantIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst("tenantId")?.Value;
            return tenantIdClaim != null ? int.Parse(tenantIdClaim) : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User
        .FindFirst(ClaimTypes.Email)?.Value;
}
