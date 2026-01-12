using System;

namespace ThuHocPhi.Application.DTOs.ThanhToan;

public sealed class GiaoDichDto
{
    public string MaGiaoDich { get; set; } = string.Empty;
    public long MaCongNo { get; set; }
    public decimal SoTien { get; set; }
    public string MaPhuongThuc { get; set; } = string.Empty;
    public string? MaGiaoDichNganHang { get; set; }
    public DateTime NgayGiaoDich { get; set; }
    public string? NoiDung { get; set; }
    public string? NguoiNop { get; set; }
    public int? NguoiThuTien { get; set; }
    public byte TrangThai { get; set; }
    public string? GhiChu { get; set; }
}
