using Microsoft.EntityFrameworkCore;
using Stokio.Application.Common.Interfaces;
using Stokio.Application.Common.Models;
using Stokio.Infrastructure.Persistence;

namespace Stokio.Infrastructure.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Avoid leaking which field is wrong
        const string invalidCredentialsMessage = "Credenciales inválidas.";

        var subdomain = request.Subdomain?.Trim();
        var email = request.Email?.Trim();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(subdomain) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = invalidCredentialsMessage
            };
        }

        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.IsActive && EF.Functions.ILike(t.Subdomain, subdomain), cancellationToken);

        if (tenant is null)
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = invalidCredentialsMessage
            };
        }

        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.TenantId == tenant.Id && u.IsActive && EF.Functions.ILike(u.Email, email), cancellationToken);

        if (user is null)
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = invalidCredentialsMessage
            };
        }

        if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, password))
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = invalidCredentialsMessage
            };
        }

        var roles = await _dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.TenantId == tenant.Id && ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user.Id, tenant.Id, user.Email, roles);

        return new AuthenticationResult
        {
            Success = true,
            Token = token,
            TenantId = tenant.Id,
            UserId = user.Id,
            Email = user.Email,
            Roles = roles
        };
    }
}
