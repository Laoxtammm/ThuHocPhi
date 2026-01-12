using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class NhatKyHeThong
{
    public long Id { get; set; }
    public int? MaNguoiDung { get; set; }
    public string HanhDong { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? ChiTiet { get; set; }
    public string? DiaChiIp { get; set; }
    public DateTime ThoiGian { get; set; }

    public NguoiDung? NguoiDung { get; set; }
}
