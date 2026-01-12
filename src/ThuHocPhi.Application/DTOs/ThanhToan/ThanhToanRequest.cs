using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.ThanhToan;

public sealed class ThanhToanRequest
{
    [Required]
    [MaxLength(20)]
    public string MaSV { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string MaHocKy { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal SoTien { get; set; }

    [Required]
    [MaxLength(20)]
    public string MaPhuongThuc { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MaGiaoDichNganHang { get; set; }

    public int? NguoiThuTien { get; set; }

    public bool XuatBienLai { get; set; } = true;
}
