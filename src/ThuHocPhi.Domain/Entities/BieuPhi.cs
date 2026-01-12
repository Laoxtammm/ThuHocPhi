using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class BieuPhi
{
    public long MaBieuPhi { get; set; }
    public string MaLoaiPhi { get; set; } = string.Empty;
    public string MaHocKy { get; set; } = string.Empty;
    public string? HeDaoTao { get; set; }
    public string? LoaiHinhDaoTao { get; set; }
    public decimal DonGia { get; set; }
    public DateTime NgayApDung { get; set; }
    public DateTime? NgayHetHan { get; set; }
    public string? GhiChu { get; set; }
    public bool TrangThai { get; set; }
}
