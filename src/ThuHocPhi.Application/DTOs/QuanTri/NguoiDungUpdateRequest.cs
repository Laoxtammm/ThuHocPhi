using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.QuanTri;

public sealed class NguoiDungUpdateRequest
{
    [MaxLength(255)]
    public string? HoTen { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(15)]
    public string? SoDienThoai { get; set; }

    public bool? TrangThai { get; set; }
}
