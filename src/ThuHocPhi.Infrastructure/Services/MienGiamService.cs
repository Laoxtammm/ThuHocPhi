using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.MienGiam;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Services;

public sealed class MienGiamService : IMienGiamService
{
    private readonly ThuHocPhiDbContext _dbContext;

    public MienGiamService(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ChinhSachMienGiamDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ChinhSachMienGiam.AsNoTracking()
            .OrderBy(x => x.MaChinhSach)
            .Select(x => new ChinhSachMienGiamDto
            {
                MaChinhSach = x.MaChinhSach,
                TenChinhSach = x.TenChinhSach,
                DoiTuongApDung = x.DoiTuongApDung,
                LoaiMienGiam = x.LoaiMienGiam,
                GiaTriMienGiam = x.GiaTriMienGiam,
                NgayApDung = x.NgayApDung,
                NgayHetHan = x.NgayHetHan,
                TrangThai = x.TrangThai
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ChinhSachMienGiamDto?> GetByIdAsync(string maChinhSach, CancellationToken cancellationToken)
    {
        return await _dbContext.ChinhSachMienGiam.AsNoTracking()
            .Where(x => x.MaChinhSach == maChinhSach)
            .Select(x => new ChinhSachMienGiamDto
            {
                MaChinhSach = x.MaChinhSach,
                TenChinhSach = x.TenChinhSach,
                DoiTuongApDung = x.DoiTuongApDung,
                LoaiMienGiam = x.LoaiMienGiam,
                GiaTriMienGiam = x.GiaTriMienGiam,
                NgayApDung = x.NgayApDung,
                NgayHetHan = x.NgayHetHan,
                TrangThai = x.TrangThai
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> CreateAsync(ChinhSachMienGiamCreateRequest request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.ChinhSachMienGiam.AnyAsync(
            x => x.MaChinhSach == request.MaChinhSach, cancellationToken);
        if (exists)
        {
            return false;
        }

        var entity = new ChinhSachMienGiam
        {
            MaChinhSach = request.MaChinhSach,
            TenChinhSach = request.TenChinhSach,
            DoiTuongApDung = request.DoiTuongApDung,
            LoaiMienGiam = request.LoaiMienGiam,
            GiaTriMienGiam = request.GiaTriMienGiam,
            NgayApDung = request.NgayApDung,
            NgayHetHan = request.NgayHetHan,
            TrangThai = true
        };

        _dbContext.ChinhSachMienGiam.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateAsync(string maChinhSach, ChinhSachMienGiamUpdateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ChinhSachMienGiam
            .FirstOrDefaultAsync(x => x.MaChinhSach == maChinhSach, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.TenChinhSach))
        {
            entity.TenChinhSach = request.TenChinhSach;
        }

        if (request.DoiTuongApDung is not null)
        {
            entity.DoiTuongApDung = request.DoiTuongApDung;
        }

        if (request.LoaiMienGiam is not null)
        {
            entity.LoaiMienGiam = request.LoaiMienGiam;
        }

        if (request.GiaTriMienGiam.HasValue)
        {
            entity.GiaTriMienGiam = request.GiaTriMienGiam.Value;
        }

        if (request.NgayApDung.HasValue)
        {
            entity.NgayApDung = request.NgayApDung.Value;
        }

        if (request.NgayHetHan.HasValue)
        {
            entity.NgayHetHan = request.NgayHetHan.Value;
        }

        if (request.TrangThai.HasValue)
        {
            entity.TrangThai = request.TrangThai.Value;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(string maChinhSach, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ChinhSachMienGiam
            .FirstOrDefaultAsync(x => x.MaChinhSach == maChinhSach, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.TrangThai = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ApDungAsync(ApDungMienGiamRequest request, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SinhVienMienGiam
            .FirstOrDefaultAsync(x => x.MaSV == request.MaSV
                                      && x.MaChinhSach == request.MaChinhSach
                                      && x.MaHocKy == request.MaHocKy, cancellationToken);

        if (record is null)
        {
            record = new SinhVienMienGiam
            {
                MaSV = request.MaSV,
                MaChinhSach = request.MaChinhSach,
                MaHocKy = request.MaHocKy,
                NguoiPheDuyet = request.NguoiPheDuyet,
                NgayPheDuyet = System.DateTime.UtcNow,
                GhiChu = request.GhiChu,
                TrangThai = true
            };
            _dbContext.SinhVienMienGiam.Add(record);
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
