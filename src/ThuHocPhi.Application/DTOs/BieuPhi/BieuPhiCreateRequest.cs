using System;
using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.BieuPhi;

public sealed class BieuPhiCreateRequest
{
    [Required]
    [MaxLength(20)]
    public string MaLoaiPhi { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string MaHocKy { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? HeDaoTao { get; set; }

    [MaxLength(50)]
    public string? LoaiHinhDaoTao { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DonGia { get; set; }

    [Required]
    public DateTime NgayApDung { get; set; }

    public DateTime? NgayHetHan { get; set; }

    [MaxLength(500)]
    public string? GhiChu { get; set; }
}
