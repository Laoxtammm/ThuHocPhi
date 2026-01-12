using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.CongNo;

public sealed class CongNoTinhRequest
{
    [Required]
    [MaxLength(20)]
    public string MaSV { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string MaHocKy { get; set; } = string.Empty;
}
