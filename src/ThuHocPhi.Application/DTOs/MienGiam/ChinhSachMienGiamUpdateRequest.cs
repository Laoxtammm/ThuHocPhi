using System;
using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.MienGiam;

public sealed class ChinhSachMienGiamUpdateRequest
{
    [MaxLength(255)]
    public string? TenChinhSach { get; set; }

    [MaxLength(500)]
    public string? DoiTuongApDung { get; set; }

    [MaxLength(50)]
    public string? LoaiMienGiam { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? GiaTriMienGiam { get; set; }

    public DateTime? NgayApDung { get; set; }
    public DateTime? NgayHetHan { get; set; }
    public bool? TrangThai { get; set; }
}
