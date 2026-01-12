using System;

namespace ThuHocPhi.Domain.Entities;

public sealed class BienLai
{
    public Guid MaBienLai { get; set; }
    public string MaGiaoDich { get; set; } = string.Empty;
    public string SoBienLai { get; set; } = string.Empty;
    public DateTime NgayXuat { get; set; }
    public int? NguoiXuat { get; set; }
    public string? FilePath { get; set; }
    public bool TrangThai { get; set; }
}
