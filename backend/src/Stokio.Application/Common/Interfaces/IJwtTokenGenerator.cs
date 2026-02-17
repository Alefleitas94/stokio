namespace Stokio.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(int userId, int tenantId, string email, IEnumerable<string> roles);
}
