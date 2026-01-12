using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class DoiSoatGiaoDich
{
    public long MaDoiSoat { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public string? NguonDuLieu { get; set; }
    public int TongGiaoDich { get; set; }
    public int TongTrungKhop { get; set; }
    public int TongSaiLech { get; set; }
    public int TongThieu { get; set; }
    public int? NguoiThucHien { get; set; }
    public DateTime NgayThucHien { get; set; }
    public string? GhiChu { get; set; }
}
