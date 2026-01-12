using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.CongNo;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Services;

public sealed class CongNoService : ICongNoService
{
    private readonly ThuHocPhiDbContext _dbContext;

    public CongNoService(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CongNoTinhResult> TinhCongNoAsync(CongNoTinhRequest request, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Set<CongNoTinhResult>()
            .FromSqlRaw("EXEC dbo.sp_TinhCongNo @MaSV = {0}, @MaHocKy = {1}", request.MaSV, request.MaHocKy)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? new CongNoTinhResult();
    }

    public async Task<CongNoDto?> GetCongNoAsync(string maSv, string maHocKy, CancellationToken cancellationToken)
    {
        var congNo = await _dbContext.CongNo.AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaSV == maSv && x.MaHocKy == maHocKy, cancellationToken);

        if (congNo is null)
        {
            return null;
        }

        var chiTiet = await _dbContext.CongNoChiTiet.AsNoTracking()
            .Where(x => x.MaCongNo == congNo.MaCongNo)
            .OrderBy(x => x.MaCongNoCT)
            .Select(x => new CongNoChiTietDto
            {
                MaCongNoCT = x.MaCongNoCT,
                MaCongNo = x.MaCongNo,
                LoaiDong = x.LoaiDong,
                RefId = x.RefId,
                MoTa = x.MoTa,
                SoLuong = x.SoLuong,
                DonGia = x.DonGia,
                ThanhTien = x.ThanhTien,
                MienGiam = x.MienGiam,
                PhaiThu = x.PhaiThu
            })
            .ToListAsync(cancellationToken);

        return new CongNoDto
        {
            MaCongNo = congNo.MaCongNo,
            MaSV = congNo.MaSV,
            MaHocKy = congNo.MaHocKy,
            TongPhaiNop = congNo.TongPhaiNop,
            TienMienGiam = congNo.TienMienGiam,
            TongDaNop = congNo.TongDaNop,
            ConNo = congNo.ConNo,
            HanNop = congNo.HanNop,
            NgayTao = congNo.NgayTao,
            NgayCapNhat = congNo.NgayCapNhat,
            TrangThai = congNo.TrangThai,
            ChiTiet = chiTiet
        };
    }
}
