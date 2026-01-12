using System;

namespace ThuHocPhi.Application.DTOs.ThongBao;

public sealed class ThongBaoHocPhiDto
{
    public long MaThongBao { get; set; }
    public string MaHocKy { get; set; } = string.Empty;
    public string? NamHoc { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public DateTime NgayPhatHanh { get; set; }
    public DateTime? HanNop { get; set; }
    public string? DoiTuongApDung { get; set; }
    public int? TongSoSinhVien { get; set; }
    public decimal? TongSoTienCongNo { get; set; }
    public byte TrangThai { get; set; }
    public string? HinhThucGui { get; set; }
    public DateTime? ThoiDiemGui { get; set; }
    public int? NguoiPhatHanh { get; set; }
    public DateTime NgayTao { get; set; }
    public DateTime? NgayCapNhat { get; set; }
}
