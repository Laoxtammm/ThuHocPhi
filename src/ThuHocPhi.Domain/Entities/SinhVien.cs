using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class SinhVien
{
    public string MaSV { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    public string? CCCD { get; set; }
    public string? DiaChi { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public string MaLopCN { get; set; } = string.Empty;
    public string MaKhoaHoc { get; set; } = string.Empty;
    public string HeDaoTao { get; set; } = string.Empty;
    public string LoaiHinhDaoTao { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
    public DateTime? NgayNhapHoc { get; set; }
    public DateTime NgayTao { get; set; }
}
