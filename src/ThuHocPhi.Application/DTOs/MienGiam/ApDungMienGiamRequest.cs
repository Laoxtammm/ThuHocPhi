using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.MienGiam;

public sealed class ApDungMienGiamRequest
{
    [Required]
    [MaxLength(20)]
    public string MaSV { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string MaChinhSach { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string MaHocKy { get; set; } = string.Empty;

    public int? NguoiPheDuyet { get; set; }

    [MaxLength(500)]
    public string? GhiChu { get; set; }
}
