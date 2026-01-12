using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.QuanTri;

public sealed class NguoiDungCreateRequest
{
    [Required]
    [MaxLength(50)]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string HoTen { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string MatKhau { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(15)]
    public string? SoDienThoai { get; set; }

    public bool TrangThai { get; set; } = true;
}
