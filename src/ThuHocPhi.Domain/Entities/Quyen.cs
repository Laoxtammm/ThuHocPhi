using System.Collections.Generic;

namespace ThuHocPhi.Domain.Entities;

public sealed class Quyen
{
    public int MaQuyen { get; set; }
    public string TenQuyen { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public string? Module { get; set; }

    public ICollection<VaiTroQuyen> VaiTroQuyen { get; set; } = new List<VaiTroQuyen>();
}
