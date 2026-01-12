using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.HocBong;

public sealed class HocBongUpdateRequest
{
    [MaxLength(255)]
    public string? TenHocBong { get; set; }

    [MaxLength(50)]
    public string? LoaiGiaTri { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? GiaTri { get; set; }

    [MaxLength(20)]
    public string? NamHoc { get; set; }

    [MaxLength(500)]
    public string? MoTa { get; set; }

    public bool? TrangThai { get; set; }
}
