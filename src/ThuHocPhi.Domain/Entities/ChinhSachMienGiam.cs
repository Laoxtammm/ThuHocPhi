using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class ChinhSachMienGiam
{
    public string MaChinhSach { get; set; } = string.Empty;
    public string TenChinhSach { get; set; } = string.Empty;
    public string? DoiTuongApDung { get; set; }
    public string LoaiMienGiam { get; set; } = string.Empty;
    public decimal GiaTriMienGiam { get; set; }
    public DateTime? NgayApDung { get; set; }
    public DateTime? NgayHetHan { get; set; }
    public bool TrangThai { get; set; }
}
