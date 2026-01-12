using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.QuanTri;
using ThuHocPhi.Application.Interfaces.QuanTri;
using ThuHocPhi.Application.Interfaces.Security;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Services;

public sealed class QuanTriNguoiDungService : IQuanTriNguoiDungService
{
    private readonly ThuHocPhiDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public QuanTriNguoiDungService(ThuHocPhiDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyList<NguoiDungDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _dbContext.NguoiDung.AsNoTracking().ToListAsync(cancellationToken);
        var roles = await (from ndvt in _dbContext.NguoiDungVaiTro.AsNoTracking()
                           join vt in _dbContext.VaiTro.AsNoTracking() on ndvt.MaVaiTro equals vt.MaVaiTro
                           select new { ndvt.MaNguoiDung, vt.TenVaiTro })
            .ToListAsync(cancellationToken);

        var roleLookup = roles
            .GroupBy(x => x.MaNguoiDung)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.TenVaiTro).ToList());

        return users.Select(u => new NguoiDungDto
        {
            MaNguoiDung = u.MaNguoiDung,
            TenDangNhap = u.TenDangNhap,
            HoTen = u.HoTen,
            Email = u.Email,
            SoDienThoai = u.SoDienThoai,
            LanDangNhapCuoi = u.LanDangNhapCuoi,
            TrangThai = u.TrangThai,
            NgayTao = u.NgayTao,
            VaiTro = roleLookup.TryGetValue(u.MaNguoiDung, out var r) ? r : new List<string>()
        }).ToList();
    }

    public async Task<NguoiDungDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.NguoiDung.AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaNguoiDung == id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var roles = await (from ndvt in _dbContext.NguoiDungVaiTro.AsNoTracking()
                           join vt in _dbContext.VaiTro.AsNoTracking() on ndvt.MaVaiTro equals vt.MaVaiTro
                           where ndvt.MaNguoiDung == id
                           select vt.TenVaiTro)
            .ToListAsync(cancellationToken);

        return new NguoiDungDto
        {
            MaNguoiDung = user.MaNguoiDung,
            TenDangNhap = user.TenDangNhap,
            HoTen = user.HoTen,
            Email = user.Email,
            SoDienThoai = user.SoDienThoai,
            LanDangNhapCuoi = user.LanDangNhapCuoi,
            TrangThai = user.TrangThai,
            NgayTao = user.NgayTao,
            VaiTro = roles
        };
    }

    public async Task<int> CreateAsync(NguoiDungCreateRequest request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.NguoiDung.AnyAsync(
            x => x.TenDangNhap == request.TenDangNhap, cancellationToken);
        if (exists)
        {
            return 0;
        }

        var entity = new NguoiDung
        {
            TenDangNhap = request.TenDangNhap,
            HoTen = request.HoTen,
            MatKhauHash = _passwordHasher.HashPassword(request.MatKhau),
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            TrangThai = request.TrangThai,
            NgayTao = System.DateTime.UtcNow
        };

        _dbContext.NguoiDung.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.MaNguoiDung;
    }

    public async Task<bool> UpdateAsync(int id, NguoiDungUpdateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.NguoiDung.FirstOrDefaultAsync(x => x.MaNguoiDung == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.HoTen))
        {
            entity.HoTen = request.HoTen;
        }

        if (request.Email is not null)
        {
            entity.Email = request.Email;
        }

        if (request.SoDienThoai is not null)
        {
            entity.SoDienThoai = request.SoDienThoai;
        }

        if (request.TrangThai.HasValue)
        {
            entity.TrangThai = request.TrangThai.Value;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(int id, ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.NguoiDung.FirstOrDefaultAsync(x => x.MaNguoiDung == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.MatKhauHash = _passwordHasher.HashPassword(request.MatKhauMoi);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateRolesAsync(int id, CapNhatVaiTroRequest request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.NguoiDung.AnyAsync(x => x.MaNguoiDung == id, cancellationToken);
        if (!exists)
        {
            return false;
        }

        var validRoles = await _dbContext.VaiTro.AsNoTracking()
            .Where(x => request.MaVaiTro.Contains(x.MaVaiTro))
            .Select(x => x.MaVaiTro)
            .ToListAsync(cancellationToken);

        var current = await _dbContext.NguoiDungVaiTro
            .Where(x => x.MaNguoiDung == id)
            .ToListAsync(cancellationToken);

        _dbContext.NguoiDungVaiTro.RemoveRange(current);

        foreach (var roleId in validRoles.Distinct())
        {
            _dbContext.NguoiDungVaiTro.Add(new NguoiDungVaiTro
            {
                MaNguoiDung = id,
                MaVaiTro = roleId
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.NguoiDung.FirstOrDefaultAsync(x => x.MaNguoiDung == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.TrangThai = isActive;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<VaiTroDto>> GetRolesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.VaiTro.AsNoTracking()
            .OrderBy(x => x.TenVaiTro)
            .Select(x => new VaiTroDto
            {
                MaVaiTro = x.MaVaiTro,
                TenVaiTro = x.TenVaiTro
            })
            .ToListAsync(cancellationToken);
    }
}
