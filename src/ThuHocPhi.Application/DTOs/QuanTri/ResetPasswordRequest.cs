using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.QuanTri;

public sealed class ResetPasswordRequest
{
    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string MatKhauMoi { get; set; } = string.Empty;
}
