using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class SinhVienMienGiam
{
    public long Id { get; set; }
    public string MaSV { get; set; } = string.Empty;
    public string MaChinhSach { get; set; } = string.Empty;
    public string MaHocKy { get; set; } = string.Empty;
    public DateTime? NgayPheDuyet { get; set; }
    public int? NguoiPheDuyet { get; set; }
    public string? GhiChu { get; set; }
    public bool TrangThai { get; set; }
}
