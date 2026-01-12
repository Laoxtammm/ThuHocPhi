using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class SinhVienAnh
{
    public long MaAnh { get; set; }
    public string MaSV { get; set; } = string.Empty;
    public string LoaiAnh { get; set; } = string.Empty;
    public string DuongDanAnh { get; set; } = string.Empty;
    public bool IsAvatar { get; set; }
    public int ThuTu { get; set; }
    public DateTime NgayTao { get; set; }
    public bool TrangThai { get; set; }
}
