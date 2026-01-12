using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.DoiSoat;

public sealed class DoiSoatRequest
{
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }

    [MaxLength(100)]
    public string? NguonDuLieu { get; set; }

    public int? NguoiThucHien { get; set; }

    [Required]
    public List<DoiSoatItemDto> GiaoDichNganHang { get; set; } = new();
}
