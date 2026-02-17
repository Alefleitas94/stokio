using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stokio.Application.Common.Interfaces;
using Stokio.Infrastructure.Authentication;
using Stokio.Infrastructure.Persistence;
using Stokio.Infrastructure.Services;

namespace Stokio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<CurrentTenantService>();
        services.AddScoped<ICurrentTenantService>(sp => sp.GetRequiredService<CurrentTenantService>());
        services.AddScoped<IFeatureFlagService, FeatureFlagService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
