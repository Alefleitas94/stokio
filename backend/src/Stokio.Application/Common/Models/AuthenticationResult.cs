namespace Stokio.Application.Common.Models;

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}
