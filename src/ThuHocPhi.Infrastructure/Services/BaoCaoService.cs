using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.BaoCao;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Services;

public sealed class BaoCaoService : IBaoCaoService
{
    private readonly ThuHocPhiDbContext _dbContext;

    public BaoCaoService(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<BaoCaoCongNoKhoaDto>> BaoCaoCongNoTheoKhoaAsync(string maHocKy, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<BaoCaoCongNoKhoaDto>()
            .FromSqlRaw("EXEC dbo.sp_BaoCaoCongNo_TheoKhoa @MaHocKy = {0}", maHocKy)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DoanhThuNgayDto>> BaoCaoDoanhThuAsync(DateTime? tuNgay, DateTime? denNgay, CancellationToken cancellationToken)
    {
        var tuNgayParam = new SqlParameter("@TuNgay", tuNgay ?? (object)DBNull.Value);
        var denNgayParam = new SqlParameter("@DenNgay", denNgay ?? (object)DBNull.Value);

        const string sql = @"
SELECT
    CAST(NgayGiaoDich AS date) AS Ngay,
    SUM(SoTien) AS TongThu,
    COUNT(1) AS SoGiaoDich
FROM dbo.GiaoDich
WHERE TrangThai = 2
  AND (@TuNgay IS NULL OR NgayGiaoDich >= @TuNgay)
  AND (@DenNgay IS NULL OR NgayGiaoDich < DATEADD(day, 1, @DenNgay))
GROUP BY CAST(NgayGiaoDich AS date)
ORDER BY CAST(NgayGiaoDich AS date)";

        return await _dbContext.Set<DoanhThuNgayDto>()
            .FromSqlRaw(sql, tuNgayParam, denNgayParam)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
