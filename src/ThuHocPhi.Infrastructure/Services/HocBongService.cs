using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.HocBong;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Services;

public sealed class HocBongService : IHocBongService
{
    private readonly ThuHocPhiDbContext _dbContext;

    public HocBongService(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<HocBongDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.HocBong.AsNoTracking()
            .OrderBy(x => x.MaHocBong)
            .Select(x => new HocBongDto
            {
                MaHocBong = x.MaHocBong,
                TenHocBong = x.TenHocBong,
                LoaiGiaTri = x.LoaiGiaTri,
                GiaTri = x.GiaTri,
                NamHoc = x.NamHoc,
                MoTa = x.MoTa,
                TrangThai = x.TrangThai
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<HocBongDto?> GetByIdAsync(string maHocBong, CancellationToken cancellationToken)
    {
        return await _dbContext.HocBong.AsNoTracking()
            .Where(x => x.MaHocBong == maHocBong)
            .Select(x => new HocBongDto
            {
                MaHocBong = x.MaHocBong,
                TenHocBong = x.TenHocBong,
                LoaiGiaTri = x.LoaiGiaTri,
                GiaTri = x.GiaTri,
                NamHoc = x.NamHoc,
                MoTa = x.MoTa,
                TrangThai = x.TrangThai
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> CreateAsync(HocBongCreateRequest request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.HocBong.AnyAsync(x => x.MaHocBong == request.MaHocBong, cancellationToken);
        if (exists)
        {
            return false;
        }

        var entity = new HocBong
        {
            MaHocBong = request.MaHocBong,
            TenHocBong = request.TenHocBong,
            LoaiGiaTri = request.LoaiGiaTri,
            GiaTri = request.GiaTri,
            NamHoc = request.NamHoc,
            MoTa = request.MoTa,
            TrangThai = true
        };

        _dbContext.HocBong.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateAsync(string maHocBong, HocBongUpdateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.HocBong.FirstOrDefaultAsync(x => x.MaHocBong == maHocBong, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.TenHocBong))
        {
            entity.TenHocBong = request.TenHocBong;
        }

        if (request.LoaiGiaTri is not null)
        {
            entity.LoaiGiaTri = request.LoaiGiaTri;
        }

        if (request.GiaTri.HasValue)
        {
            entity.GiaTri = request.GiaTri.Value;
        }

        if (request.NamHoc is not null)
        {
            entity.NamHoc = request.NamHoc;
        }

        if (request.MoTa is not null)
        {
            entity.MoTa = request.MoTa;
        }

        if (request.TrangThai.HasValue)
        {
            entity.TrangThai = request.TrangThai.Value;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(string maHocBong, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.HocBong.FirstOrDefaultAsync(x => x.MaHocBong == maHocBong, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.TrangThai = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ApDungAsync(ApDungHocBongRequest request, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SinhVienHocBong
            .FirstOrDefaultAsync(x => x.MaSV == request.MaSV
                                      && x.MaHocBong == request.MaHocBong
                                      && x.MaHocKy == request.MaHocKy, cancellationToken);

        if (record is null)
        {
            record = new SinhVienHocBong
            {
                MaSV = request.MaSV,
                MaHocBong = request.MaHocBong,
                MaHocKy = request.MaHocKy,
                NguoiPheDuyet = request.NguoiPheDuyet,
                NgayPheDuyet = System.DateTime.UtcNow,
                GhiChu = request.GhiChu,
                TrangThai = true
            };
            _dbContext.SinhVienHocBong.Add(record);
        }
        else
        {
            record.NguoiPheDuyet = request.NguoiPheDuyet;
            record.NgayPheDuyet = System.DateTime.UtcNow;
            record.GhiChu = request.GhiChu;
            record.TrangThai = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_TinhCongNo @MaSV, @MaHocKy",
            new SqlParameter("@MaSV", request.MaSV),
            new SqlParameter("@MaHocKy", request.MaHocKy));

        return true;
    }
}
