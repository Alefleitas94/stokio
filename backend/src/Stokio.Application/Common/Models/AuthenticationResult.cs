namespace Stokio.Application.Common.Models;

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }

    public int? TenantId { get; set; }
    public int? UserId { get; set; }
    public string? Email { get; set; }
    public IReadOnlyList<string>? Roles { get; set; }
}
