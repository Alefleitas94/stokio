using System.ComponentModel.DataAnnotations;

namespace Stokio.Application.Common.Models;

public class LoginRequest
{
    [Required]
    [MaxLength(100)]
    public string Subdomain { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;
}
