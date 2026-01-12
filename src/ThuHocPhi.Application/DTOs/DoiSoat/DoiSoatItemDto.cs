using System;
using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.DoiSoat;

public sealed class DoiSoatItemDto
{
    [Required]
    [MaxLength(100)]
    public string MaGiaoDichNganHang { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal SoTien { get; set; }

    public DateTime? NgayGiaoDich { get; set; }
}
