using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.CongNo;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/sinh-vien/dashboard")]
[Authorize(Roles = "SinhVien")]
public sealed class StudentDashboardController : ControllerBase
{
    private readonly ThuHocPhiDbContext _dbContext;
    private readonly ICongNoService _congNoService;

    public StudentDashboardController(ThuHocPhiDbContext dbContext, ICongNoService congNoService)
    {
        _dbContext = dbContext;
        _congNoService = congNoService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string maHocKy, CancellationToken cancellationToken)
    {
        var maSv = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(maSv))
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(maHocKy))
        {
            return BadRequest(new { message = "maHocKy is required." });
        }

        var congNo = await _dbContext.CongNo.AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaSV == maSv && x.MaHocKy == maHocKy, cancellationToken);

        if (congNo is null)
        {
            await _congNoService.TinhCongNoAsync(new CongNoTinhRequest
            {
                MaSV = maSv,
                MaHocKy = maHocKy
            }, cancellationToken);

            congNo = await _dbContext.CongNo.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaSV == maSv && x.MaHocKy == maHocKy, cancellationToken);
        }

        var tongTinChi = await (from dk in _dbContext.DangKyHocPhan.AsNoTracking()
                                join hp in _dbContext.HocPhan.AsNoTracking() on dk.MaHocPhan equals hp.MaHocPhan
                                where dk.MaSV == maSv
                                select (int?)hp.SoTinChi)
            .SumAsync(cancellationToken) ?? 0;

        var hocKy = await _dbContext.HocKy.AsNoTracking()
            .Where(x => x.MaHocKy == maHocKy)
            .Select(x => new { x.MaHocKy, x.TenHocKy, x.TrangThai })
            .FirstOrDefaultAsync(cancellationToken);

        return Ok(new
        {
            maHocKy,
            tenHocKy = hocKy?.TenHocKy ?? maHocKy,
            trangThaiHocKy = hocKy?.TrangThai,
            tongPhaiNop = congNo?.TongPhaiNop ?? 0,
            tienMienGiam = congNo?.TienMienGiam ?? 0,
            tongDaNop = congNo?.TongDaNop ?? 0,
            conNo = congNo?.ConNo ?? 0,
            trangThaiCongNo = congNo?.TrangThai ?? 1,
            tongTinChi
        });
    }
}
