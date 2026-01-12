using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.HocBong;

public sealed class HocBongCreateRequest
{
    [Required]
    [MaxLength(20)]
    public string MaHocBong { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string TenHocBong { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LoaiGiaTri { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal GiaTri { get; set; }

    [MaxLength(20)]
    public string? NamHoc { get; set; }

    [MaxLength(500)]
    public string? MoTa { get; set; }
}
