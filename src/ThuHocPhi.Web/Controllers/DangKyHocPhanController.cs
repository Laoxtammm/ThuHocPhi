using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.CongNo;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/dang-ky-hoc-phan")]
[Authorize(Roles = "SinhVien")]
public sealed class DangKyHocPhanController : ControllerBase
{
    private readonly ThuHocPhiDbContext _dbContext;
    private readonly ICongNoService _congNoService;

    public DangKyHocPhanController(ThuHocPhiDbContext dbContext, ICongNoService congNoService)
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

        var data = await (from dk in _dbContext.DangKyHocPhan.AsNoTracking()
                          join hp in _dbContext.HocPhan.AsNoTracking() on dk.MaHocPhan equals hp.MaHocPhan
                          where dk.MaSV == maSv && dk.MaHocKy == maHocKy
                          orderby dk.NgayDangKy descending
                          select new
                          {
                              maHocPhan = dk.MaHocPhan,
                              tenHocPhan = hp.TenHocPhan,
                              soTinChi = hp.SoTinChi,
                              loaiHocPhan = hp.LoaiHocPhan,
                              moTa = hp.MoTa,
                              trangThai = dk.TrangThai
                          })
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DangKyHocPhanRequest request, CancellationToken cancellationToken)
    {
        var maSv = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(maSv))
        {
            return Unauthorized();
        }

        if (request is null || string.IsNullOrWhiteSpace(request.MaHocKy) || string.IsNullOrWhiteSpace(request.MaHocPhan))
        {
            return BadRequest(new { message = "MaHocKy and MaHocPhan are required." });
        }

        var hocPhanExists = await _dbContext.HocPhan
            .AnyAsync(x => x.MaHocPhan == request.MaHocPhan && x.TrangThai, cancellationToken);
        if (!hocPhanExists)
        {
            return NotFound(new { message = "Hoc phan khong ton tai." });
        }

        var exists = await _dbContext.DangKyHocPhan
            .AnyAsync(x => x.MaSV == maSv && x.MaHocKy == request.MaHocKy && x.MaHocPhan == request.MaHocPhan,
                cancellationToken);
        if (!exists)
        {
            _dbContext.DangKyHocPhan.Add(new DangKyHocPhan
            {
                MaSV = maSv,
                MaHocKy = request.MaHocKy,
                MaHocPhan = request.MaHocPhan,
                TrangThai = "Da dang ky",
                NgayDangKy = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await _congNoService.TinhCongNoAsync(new CongNoTinhRequest
        {
            MaSV = maSv,
            MaHocKy = request.MaHocKy
        }, cancellationToken);

        return Ok();
    }

    [HttpDelete("{maHocPhan}")]
    public async Task<IActionResult> Delete(string maHocPhan, [FromQuery] string maHocKy, CancellationToken cancellationToken)
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

        var entity = await _dbContext.DangKyHocPhan
            .FirstOrDefaultAsync(x => x.MaSV == maSv && x.MaHocKy == maHocKy && x.MaHocPhan == maHocPhan,
                cancellationToken);

        if (entity is null)
        {
            return NotFound();
        }

        _dbContext.DangKyHocPhan.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _congNoService.TinhCongNoAsync(new CongNoTinhRequest
        {
            MaSV = maSv,
            MaHocKy = maHocKy
        }, cancellationToken);

        return Ok();
    }

    public sealed class DangKyHocPhanRequest
    {
        public string MaHocKy { get; set; } = string.Empty;
        public string MaHocPhan { get; set; } = string.Empty;
    }
}
