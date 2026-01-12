using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class HocKy
{
    public string MaHocKy { get; set; } = string.Empty;
    public string TenHocKy { get; set; } = string.Empty;
    public string? NamHoc { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public DateTime? NgayBatDauDangKy { get; set; }
    public DateTime? NgayKetThucDangKy { get; set; }
    public string TrangThai { get; set; } = string.Empty;
}
