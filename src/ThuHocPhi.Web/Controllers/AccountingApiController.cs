using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/ke-toan")]
[Authorize(Roles = "PhongTaiChinh,Administrator")]
public sealed class AccountingApiController : ControllerBase
{
    private readonly ThuHocPhiDbContext _dbContext;

    public AccountingApiController(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("cong-no")]
    public async Task<IActionResult> GetCongNoAsync([FromQuery] string maHocKy, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(maHocKy))
        {
            return BadRequest("MaHocKy is required.");
        }

        var creditTotals = await _dbContext.CongNoChiTiet
            .AsNoTracking()
            .Where(x => x.LoaiDong == "HP_TINCHI")
            .GroupBy(x => x.MaCongNo)
            .Select(g => new { g.Key, SoTinChi = g.Sum(x => x.SoLuong) })
            .ToDictionaryAsync(x => x.Key, x => x.SoTinChi, cancellationToken);

        var list = await (from cn in _dbContext.CongNo.AsNoTracking()
                          join sv in _dbContext.SinhVien.AsNoTracking() on cn.MaSV equals sv.MaSV
                          where cn.MaHocKy == maHocKy
                          orderby sv.HoTen
                          select new
                          {
                              maSv = cn.MaSV,
                              hoTen = sv.HoTen,
                              ngaySinh = sv.NgaySinh,
                              lop = sv.MaLopCN,
                              khoaDaoTao = sv.MaKhoaHoc,
                              tongPhaiNop = cn.TongPhaiNop,
                              tienMienGiam = cn.TienMienGiam,
                              tongDaNop = cn.TongDaNop,
                              conNo = cn.ConNo,
                              maCongNo = cn.MaCongNo
                          })
            .ToListAsync(cancellationToken);

        var result = new List<object>();
        foreach (var item in list)
        {
            creditTotals.TryGetValue(item.maCongNo, out var soTinChi);
            result.Add(new
            {
                item.maSv,
                item.hoTen,
                item.ngaySinh,
                item.lop,
                item.khoaDaoTao,
                soTinChi = (int)soTinChi,
                item.tongPhaiNop,
                item.tienMienGiam,
                hocPhiSauMienGiam = item.tongPhaiNop - item.tienMienGiam,
                item.tongDaNop,
                item.conNo
            });
        }

        return Ok(result);
    }

    [HttpGet("giao-dich")]
    public async Task<IActionResult> GetGiaoDichAsync([FromQuery] string maSv, [FromQuery] string? maHocKy, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(maSv))
        {
            return BadRequest("MaSV is required.");
        }

        var query = from gd in _dbContext.GiaoDich.AsNoTracking()
                    join cn in _dbContext.CongNo.AsNoTracking() on gd.MaCongNo equals cn.MaCongNo
                    join bl in _dbContext.BienLai.AsNoTracking() on gd.MaGiaoDich equals bl.MaGiaoDich into bls
                    from bl in bls.DefaultIfEmpty()
                    where cn.MaSV == maSv
                    select new
                    {
                        gd.MaGiaoDich,
                        gd.SoTien,
                        gd.MaPhuongThuc,
                        gd.MaGiaoDichNganHang,
                        gd.NgayGiaoDich,
                        cn.MaHocKy,
                        SoBienLai = bl != null ? bl.SoBienLai : null
                    };

        if (!string.IsNullOrWhiteSpace(maHocKy))
        {
            query = query.Where(x => x.MaHocKy == maHocKy);
        }

        var data = await query
            .OrderByDescending(x => x.NgayGiaoDich)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }
}
