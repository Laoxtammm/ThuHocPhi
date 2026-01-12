using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThuHocPhi.Application.DTOs.BaoCao;

namespace ThuHocPhi.Application.Interfaces.Finance;

public interface IBaoCaoService
{
    Task<IReadOnlyList<BaoCaoCongNoKhoaDto>> BaoCaoCongNoTheoKhoaAsync(string maHocKy, CancellationToken cancellationToken);
    Task<IReadOnlyList<DoanhThuNgayDto>> BaoCaoDoanhThuAsync(DateTime? tuNgay, DateTime? denNgay, CancellationToken cancellationToken);
}
