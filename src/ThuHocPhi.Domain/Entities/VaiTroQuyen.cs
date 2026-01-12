namespace ThuHocPhi.Domain.Entities;

public sealed class VaiTroQuyen
{
    public int MaVaiTro { get; set; }
    public int MaQuyen { get; set; }

    public VaiTro VaiTro { get; set; } = null!;
    public Quyen Quyen { get; set; } = null!;
}
