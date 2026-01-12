using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/hoc-phan")]
public sealed class HocPhanController : ControllerBase
{
    private readonly ThuHocPhiDbContext _dbContext;

    public HocPhanController(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await _dbContext.HocPhan
            .AsNoTracking()
            .Where(x => x.TrangThai)
            .OrderBy(x => x.MaHocPhan)
            .Select(x => new
            {
                maHocPhan = x.MaHocPhan,
                tenHocPhan = x.TenHocPhan,
                soTinChi = x.SoTinChi,
                loaiHocPhan = x.LoaiHocPhan,
                maKhoa = x.MaKhoa,
                moTa = x.MoTa
            })
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [Authorize(Roles = "PhongDaoTao,Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HocPhanUpsertRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.MaHocPhan))
        {
            return BadRequest("MaHocPhan is required.");
        }

        var exists = await _dbContext.HocPhan.AnyAsync(x => x.MaHocPhan == request.MaHocPhan, cancellationToken);
        if (exists)
        {
            return Conflict("Hoc phan da ton tai.");
        }

        var entity = new HocPhan
        {
            MaHocPhan = request.MaHocPhan.Trim(),
            TenHocPhan = request.TenHocPhan?.Trim() ?? string.Empty,
            SoTinChi = request.SoTinChi,
            LoaiHocPhan = request.LoaiHocPhan?.Trim(),
            MoTa = request.MoTa?.Trim(),
            MaKhoa = ResolveMaKhoa(request),
            TrangThai = true
        };

        _dbContext.HocPhan.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(entity);
    }

    [Authorize(Roles = "PhongDaoTao,Administrator")]
    [HttpPut("{maHocPhan}")]
    public async Task<IActionResult> Update(string maHocPhan, [FromBody] HocPhanUpsertRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.HocPhan.FirstOrDefaultAsync(x => x.MaHocPhan == maHocPhan, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.TenHocPhan = request.TenHocPhan?.Trim() ?? entity.TenHocPhan;
        if (request.SoTinChi > 0)
        {
            entity.SoTinChi = request.SoTinChi;
        }
        entity.LoaiHocPhan = request.LoaiHocPhan?.Trim();
        entity.MoTa = request.MoTa?.Trim();
        if (!string.IsNullOrWhiteSpace(request.MaKhoa) || !string.IsNullOrWhiteSpace(request.ChuyenNganh))
        {
            entity.MaKhoa = ResolveMaKhoa(request);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(entity);
    }

    [Authorize(Roles = "PhongDaoTao,Administrator")]
    [HttpDelete("{maHocPhan}")]
    public async Task<IActionResult> Delete(string maHocPhan, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.HocPhan.FirstOrDefaultAsync(x => x.MaHocPhan == maHocPhan, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.TrangThai = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    private static string ResolveMaKhoa(HocPhanUpsertRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.MaKhoa))
        {
            return request.MaKhoa.Trim().ToUpperInvariant();
        }

        var major = (request.ChuyenNganh ?? string.Empty).Trim().ToUpperInvariant();
        if (major is "CNPM" or "ATTT" or "HTTT")
        {
            return "FIT";
        }

        if (major is "CK")
        {
            return "CK";
        }

        if (major is "DTVT")
        {
            return "DTVT";
        }

        return "FIT";
    }

    public sealed class HocPhanUpsertRequest
    {
        public string MaHocPhan { get; set; } = string.Empty;
        public string? TenHocPhan { get; set; }
        public int SoTinChi { get; set; }
        public string? LoaiHocPhan { get; set; }
        public string? MoTa { get; set; }
        public string? MaKhoa { get; set; }
        public string? ChuyenNganh { get; set; }
    }
}
