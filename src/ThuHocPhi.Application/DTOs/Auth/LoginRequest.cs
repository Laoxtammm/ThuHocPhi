using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.Auth;

public sealed class LoginRequest
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(4)]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}
