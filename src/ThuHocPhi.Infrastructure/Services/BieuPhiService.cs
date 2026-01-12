using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.BieuPhi;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Services;

public sealed class BieuPhiService : IBieuPhiService
{
    private readonly ThuHocPhiDbContext _dbContext;

    public BieuPhiService(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<BieuPhiDto>> GetAllAsync(string? maHocKy, CancellationToken cancellationToken)
    {
        var query = _dbContext.BieuPhi.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(maHocKy))
        {
            query = query.Where(x => x.MaHocKy == maHocKy);
        }

        return await query
            .OrderByDescending(x => x.NgayApDung)
            .Select(x => new BieuPhiDto
            {
                MaBieuPhi = x.MaBieuPhi,
                MaLoaiPhi = x.MaLoaiPhi,
                MaHocKy = x.MaHocKy,
                HeDaoTao = x.HeDaoTao,
                LoaiHinhDaoTao = x.LoaiHinhDaoTao,
                DonGia = x.DonGia,
                NgayApDung = x.NgayApDung,
                NgayHetHan = x.NgayHetHan,
                GhiChu = x.GhiChu,
                TrangThai = x.TrangThai
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<BieuPhiDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _dbContext.BieuPhi.AsNoTracking()
            .Where(x => x.MaBieuPhi == id)
            .Select(x => new BieuPhiDto
            {
                MaBieuPhi = x.MaBieuPhi,
                MaLoaiPhi = x.MaLoaiPhi,
                MaHocKy = x.MaHocKy,
                HeDaoTao = x.HeDaoTao,
                LoaiHinhDaoTao = x.LoaiHinhDaoTao,
                DonGia = x.DonGia,
                NgayApDung = x.NgayApDung,
                NgayHetHan = x.NgayHetHan,
                GhiChu = x.GhiChu,
                TrangThai = x.TrangThai
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<long> CreateAsync(BieuPhiCreateRequest request, CancellationToken cancellationToken)
    {
        var entity = new BieuPhi
        {
            MaLoaiPhi = request.MaLoaiPhi,
            MaHocKy = request.MaHocKy,
            HeDaoTao = request.HeDaoTao,
            LoaiHinhDaoTao = request.LoaiHinhDaoTao,
            DonGia = request.DonGia,
            NgayApDung = request.NgayApDung,
            NgayHetHan = request.NgayHetHan,
            GhiChu = request.GhiChu,
            TrangThai = true
        };

        _dbContext.BieuPhi.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.MaBieuPhi;
    }

    public async Task<bool> UpdateAsync(long id, BieuPhiUpdateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.BieuPhi.FirstOrDefaultAsync(x => x.MaBieuPhi == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.MaLoaiPhi))
        {
            entity.MaLoaiPhi = request.MaLoaiPhi;
        }

        if (!string.IsNullOrWhiteSpace(request.MaHocKy))
        {
            entity.MaHocKy = request.MaHocKy;
        }

        if (request.HeDaoTao is not null)
        {
            entity.HeDaoTao = request.HeDaoTao;
        }

        if (request.LoaiHinhDaoTao is not null)
        {
            entity.LoaiHinhDaoTao = request.LoaiHinhDaoTao;
        }

        if (request.DonGia.HasValue)
        {
            entity.DonGia = request.DonGia.Value;
        }

        if (request.NgayApDung.HasValue)
        {
            entity.NgayApDung = request.NgayApDung.Value;
        }

        if (request.NgayHetHan.HasValue)
        {
            entity.NgayHetHan = request.NgayHetHan.Value;
        }

        if (request.GhiChu is not null)
        {
            entity.GhiChu = request.GhiChu;
        }

        if (request.TrangThai.HasValue)
        {
            entity.TrangThai = request.TrangThai.Value;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.BieuPhi.FirstOrDefaultAsync(x => x.MaBieuPhi == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.TrangThai = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
