using Stokio.Application.Common.Models;

namespace Stokio.Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
