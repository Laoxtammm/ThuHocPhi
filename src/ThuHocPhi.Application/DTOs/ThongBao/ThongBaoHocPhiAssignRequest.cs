using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThuHocPhi.Application.DTOs.ThongBao;

public sealed class ThongBaoHocPhiAssignRequest
{
    [Required]
    public List<string> MaSVs { get; set; } = new();
}
