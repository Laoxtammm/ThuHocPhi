using System;
using System.Collections.Generic;

namespace ThuHocPhi.Domain.Entities;

public sealed class VaiTro
{
    public int MaVaiTro { get; set; }
    public string TenVaiTro { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public DateTime NgayTao { get; set; }
    public bool TrangThai { get; set; }

    public ICollection<NguoiDungVaiTro> NguoiDungVaiTro { get; set; } = new List<NguoiDungVaiTro>();
    public ICollection<VaiTroQuyen> VaiTroQuyen { get; set; } = new List<VaiTroQuyen>();
}
