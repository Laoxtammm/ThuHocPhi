using System;
using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.MienGiam;

public sealed class ChinhSachMienGiamCreateRequest
{
    [Required]
    [MaxLength(20)]
    public string MaChinhSach { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string TenChinhSach { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? DoiTuongApDung { get; set; }

    [Required]
    [MaxLength(50)]
    public string LoaiMienGiam { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal GiaTriMienGiam { get; set; }

    public DateTime? NgayApDung { get; set; }
    public DateTime? NgayHetHan { get; set; }
}
