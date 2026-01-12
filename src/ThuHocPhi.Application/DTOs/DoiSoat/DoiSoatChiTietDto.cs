using System;

namespace ThuHocPhi.Application.DTOs.DoiSoat;

public sealed class DoiSoatChiTietDto
{
    public long Id { get; set; }
    public long MaDoiSoat { get; set; }
    public string MaGiaoDichNganHang { get; set; } = string.Empty;
    public string? MaGiaoDichHeThong { get; set; }
    public decimal SoTienNganHang { get; set; }
    public decimal? SoTienHeThong { get; set; }
    public DateTime? NgayGiaoDich { get; set; }
    public byte TrangThai { get; set; }
    public string? GhiChu { get; set; }
}
