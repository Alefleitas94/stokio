using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Stokio.Infrastructure.Persistence;
using Stokio.Infrastructure.Services;

namespace Stokio.Api.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, CurrentTenantService currentTenantService)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirst("tenantId")?.Value;

            if (!string.IsNullOrWhiteSpace(tenantIdClaim) && int.TryParse(tenantIdClaim, out var tenantId))
            {
                var tenant = await dbContext.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);

                if (tenant is not null)
                {
                    currentTenantService.SetTenant(tenant);
                    context.Items["Tenant"] = tenant;
                }
            }
        }

        await _next(context);
    }
}
