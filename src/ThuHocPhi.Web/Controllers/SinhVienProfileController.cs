using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/sinh-vien")]
[Authorize(Roles = "SinhVien")]
public sealed class SinhVienProfileController : ControllerBase
{
    private readonly ThuHocPhiDbContext _dbContext;

    public SinhVienProfileController(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var maSv = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(maSv))
        {
            return Unauthorized();
        }

        var sinhVien = await _dbContext.SinhVien
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaSV == maSv, cancellationToken);

        if (sinhVien is null)
        {
            return NotFound();
        }

        var avatar = await _dbContext.SinhVienAnh
            .AsNoTracking()
            .Where(x => x.MaSV == maSv && x.TrangThai)
            .OrderByDescending(x => x.IsAvatar)
            .ThenBy(x => x.ThuTu)
            .Select(x => x.DuongDanAnh)
            .FirstOrDefaultAsync(cancellationToken);

        return Ok(new
        {
            maSv = sinhVien.MaSV,
            hoTen = sinhVien.HoTen,
            ngaySinh = sinhVien.NgaySinh,
            lop = sinhVien.MaLopCN,
            heDaoTao = sinhVien.HeDaoTao,
            loaiHinhDaoTao = sinhVien.LoaiHinhDaoTao,
            email = sinhVien.Email,
            soDienThoai = sinhVien.SoDienThoai,
            trangThai = sinhVien.TrangThai,
            avatarUrl = avatar
        });
    }
}
