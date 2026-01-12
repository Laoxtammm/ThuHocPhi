using System;
using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.ThongBao;

public sealed class ThongBaoHocPhiUpdateRequest
{
    [MaxLength(20)]
    public string? MaHocKy { get; set; }

    [MaxLength(20)]
    public string? NamHoc { get; set; }

    [MaxLength(255)]
    public string? TieuDe { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayPhatHanh { get; set; }

    public DateTime? HanNop { get; set; }

    [MaxLength(500)]
    public string? DoiTuongApDung { get; set; }

    [MaxLength(50)]
    public string? HinhThucGui { get; set; }

    public byte? TrangThai { get; set; }
}
