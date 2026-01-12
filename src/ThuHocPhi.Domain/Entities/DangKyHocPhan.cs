using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class DangKyHocPhan
{
    public long MaDangKy { get; set; }
    public string MaSV { get; set; } = string.Empty;
    public string MaHocPhan { get; set; } = string.Empty;
    public string MaHocKy { get; set; } = string.Empty;
    public DateTime NgayDangKy { get; set; }
    public string TrangThai { get; set; } = string.Empty;
}
