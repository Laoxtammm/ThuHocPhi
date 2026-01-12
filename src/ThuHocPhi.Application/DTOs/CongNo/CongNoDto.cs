using System;
using System.Collections.Generic;

namespace ThuHocPhi.Application.DTOs.CongNo;

public sealed class CongNoDto
{
    public long MaCongNo { get; set; }
    public string MaSV { get; set; } = string.Empty;
    public string MaHocKy { get; set; } = string.Empty;
    public decimal TongPhaiNop { get; set; }
    public decimal TienMienGiam { get; set; }
    public decimal TongDaNop { get; set; }
    public decimal ConNo { get; set; }
    public DateTime? HanNop { get; set; }
    public DateTime NgayTao { get; set; }
    public DateTime? NgayCapNhat { get; set; }
    public byte TrangThai { get; set; }
    public IReadOnlyList<CongNoChiTietDto> ChiTiet { get; set; } = new List<CongNoChiTietDto>();
}
