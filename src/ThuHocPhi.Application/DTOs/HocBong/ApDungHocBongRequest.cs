using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.HocBong;

public sealed class ApDungHocBongRequest
{
    [Required]
    [MaxLength(20)]
    public string MaSV { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string MaHocBong { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string MaHocKy { get; set; } = string.Empty;

    public int? NguoiPheDuyet { get; set; }

    [MaxLength(500)]
    public string? GhiChu { get; set; }
}
