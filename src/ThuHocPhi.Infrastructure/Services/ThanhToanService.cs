using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.ThanhToan;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Services;

public sealed class ThanhToanService : IThanhToanService
{
    private readonly ThuHocPhiDbContext _dbContext;

    public ThanhToanService(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ThanhToanResult> ThanhToanAsync(ThanhToanRequest request, CancellationToken cancellationToken)
    {
        var maGiaoDichNganHangParam = new SqlParameter("@MaGiaoDichNganHang", (object?)request.MaGiaoDichNganHang ?? DBNull.Value);
        var nguoiThuTienParam = new SqlParameter("@NguoiThuTien", (object?)request.NguoiThuTien ?? DBNull.Value);

        var data = await _dbContext.Set<ThanhToanResult>()
            .FromSqlRaw(
                "EXEC dbo.sp_XuLyThanhToan @MaSV, @MaHocKy, @SoTien, @MaPhuongThuc, @MaGiaoDichNganHang, @NguoiThuTien, @XuatBienLai",
                new SqlParameter("@MaSV", request.MaSV),
                new SqlParameter("@MaHocKy", request.MaHocKy),
                new SqlParameter("@SoTien", request.SoTien),
                new SqlParameter("@MaPhuongThuc", request.MaPhuongThuc),
                maGiaoDichNganHangParam,
                nguoiThuTienParam,
                new SqlParameter("@XuatBienLai", request.XuatBienLai ? 1 : 0))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return data.FirstOrDefault() ?? new ThanhToanResult();
    }

    public async Task<IReadOnlyList<GiaoDichDto>> GetGiaoDichAsync(string? maSv, string? maHocKy, CancellationToken cancellationToken)
    {
        var query = from gd in _dbContext.GiaoDich.AsNoTracking()
                    join cn in _dbContext.CongNo.AsNoTracking() on gd.MaCongNo equals cn.MaCongNo
                    select new { gd, cn };

        if (!string.IsNullOrWhiteSpace(maSv))
        {
            query = query.Where(x => x.cn.MaSV == maSv);
        }

        if (!string.IsNullOrWhiteSpace(maHocKy))
        {
            query = query.Where(x => x.cn.MaHocKy == maHocKy);
        }

        return await query
            .OrderByDescending(x => x.gd.NgayGiaoDich)
            .Select(x => new GiaoDichDto
            {
                MaGiaoDich = x.gd.MaGiaoDich,
                MaCongNo = x.gd.MaCongNo,
                SoTien = x.gd.SoTien,
                MaPhuongThuc = x.gd.MaPhuongThuc,
                MaGiaoDichNganHang = x.gd.MaGiaoDichNganHang,
                NgayGiaoDich = x.gd.NgayGiaoDich,
                NoiDung = x.gd.NoiDung,
                NguoiNop = x.gd.NguoiNop,
                NguoiThuTien = x.gd.NguoiThuTien,
                TrangThai = x.gd.TrangThai,
                GhiChu = x.gd.GhiChu
            })
            .ToListAsync(cancellationToken);
    }
}
