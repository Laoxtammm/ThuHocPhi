using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.ThongBao;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Services;

public sealed class ThongBaoHocPhiService : IThongBaoHocPhiService
{
    private readonly ThuHocPhiDbContext _dbContext;

    public ThongBaoHocPhiService(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ThongBaoHocPhiDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ThongBaoHocPhi.AsNoTracking()
            .OrderByDescending(x => x.NgayTao)
            .Select(x => new ThongBaoHocPhiDto
            {
                MaThongBao = x.MaThongBao,
                MaHocKy = x.MaHocKy,
                NamHoc = x.NamHoc,
                TieuDe = x.TieuDe,
                NoiDung = x.NoiDung,
                NgayPhatHanh = x.NgayPhatHanh,
                HanNop = x.HanNop,
                DoiTuongApDung = x.DoiTuongApDung,
                TongSoSinhVien = x.TongSoSinhVien,
                TongSoTienCongNo = x.TongSoTienCongNo,
                TrangThai = x.TrangThai,
                HinhThucGui = x.HinhThucGui,
                ThoiDiemGui = x.ThoiDiemGui,
                NguoiPhatHanh = x.NguoiPhatHanh,
                NgayTao = x.NgayTao,
                NgayCapNhat = x.NgayCapNhat
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ThongBaoHocPhiDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _dbContext.ThongBaoHocPhi.AsNoTracking()
            .Where(x => x.MaThongBao == id)
            .Select(x => new ThongBaoHocPhiDto
            {
                MaThongBao = x.MaThongBao,
                MaHocKy = x.MaHocKy,
                NamHoc = x.NamHoc,
                TieuDe = x.TieuDe,
                NoiDung = x.NoiDung,
                NgayPhatHanh = x.NgayPhatHanh,
                HanNop = x.HanNop,
                DoiTuongApDung = x.DoiTuongApDung,
                TongSoSinhVien = x.TongSoSinhVien,
                TongSoTienCongNo = x.TongSoTienCongNo,
                TrangThai = x.TrangThai,
                HinhThucGui = x.HinhThucGui,
                ThoiDiemGui = x.ThoiDiemGui,
                NguoiPhatHanh = x.NguoiPhatHanh,
                NgayTao = x.NgayTao,
                NgayCapNhat = x.NgayCapNhat
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<long> CreateAsync(ThongBaoHocPhiCreateRequest request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var entity = new ThongBaoHocPhi
        {
            MaHocKy = request.MaHocKy,
            NamHoc = request.NamHoc,
            TieuDe = request.TieuDe,
            NoiDung = request.NoiDung,
            NgayPhatHanh = request.NgayPhatHanh ?? now.Date,
            HanNop = request.HanNop,
            DoiTuongApDung = request.DoiTuongApDung,
            HinhThucGui = request.HinhThucGui,
            NguoiPhatHanh = request.NguoiPhatHanh,
            TrangThai = 1,
            NgayTao = now
        };

        _dbContext.ThongBaoHocPhi.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.MaThongBao;
    }

    public async Task<bool> UpdateAsync(long id, ThongBaoHocPhiUpdateRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ThongBaoHocPhi.FirstOrDefaultAsync(x => x.MaThongBao == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.MaHocKy))
        {
            entity.MaHocKy = request.MaHocKy;
        }

        if (request.NamHoc is not null)
        {
            entity.NamHoc = request.NamHoc;
        }

        if (!string.IsNullOrWhiteSpace(request.TieuDe))
        {
            entity.TieuDe = request.TieuDe;
        }

        if (request.NoiDung is not null)
        {
            entity.NoiDung = request.NoiDung;
        }

        if (request.NgayPhatHanh.HasValue)
        {
            entity.NgayPhatHanh = request.NgayPhatHanh.Value;
        }

        if (request.HanNop.HasValue)
        {
            entity.HanNop = request.HanNop.Value;
        }

        if (request.DoiTuongApDung is not null)
        {
            entity.DoiTuongApDung = request.DoiTuongApDung;
        }

        if (request.HinhThucGui is not null)
        {
            entity.HinhThucGui = request.HinhThucGui;
        }

        if (request.TrangThai.HasValue)
        {
            entity.TrangThai = request.TrangThai.Value;
        }

        entity.NgayCapNhat = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ThongBaoHocPhi.FirstOrDefaultAsync(x => x.MaThongBao == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.TrangThai = 0;
        entity.NgayCapNhat = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ThongBaoHocPhiRecipientDto>> GetRecipientsAsync(long maThongBao, CancellationToken cancellationToken)
    {
        return await _dbContext.ThongBaoHocPhiSinhVien.AsNoTracking()
            .Where(x => x.MaThongBao == maThongBao)
            .OrderBy(x => x.MaSV)
            .Select(x => new ThongBaoHocPhiRecipientDto
            {
                Id = x.Id,
                MaThongBao = x.MaThongBao,
                MaSV = x.MaSV,
                TrangThaiDoc = x.TrangThaiDoc,
                TrangThaiThanhToan = x.TrangThaiThanhToan,
                NgayGui = x.NgayGui,
                NgayXem = x.NgayXem,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<int> AssignRecipientsAsync(long maThongBao, ThongBaoHocPhiAssignRequest request, CancellationToken cancellationToken)
    {
        var thongBao = await _dbContext.ThongBaoHocPhi
            .FirstOrDefaultAsync(x => x.MaThongBao == maThongBao, cancellationToken);
        if (thongBao is null)
        {
            return 0;
        }

        var now = DateTime.UtcNow;
        var distinctSv = request.MaSVs
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct()
            .ToList();

        var existing = await _dbContext.ThongBaoHocPhiSinhVien
            .Where(x => x.MaThongBao == maThongBao)
            .Select(x => x.MaSV)
            .ToListAsync(cancellationToken);

        var newRecipients = distinctSv.Except(existing).ToList();
        foreach (var maSv in newRecipients)
        {
            _dbContext.ThongBaoHocPhiSinhVien.Add(new ThongBaoHocPhiSinhVien
            {
                MaThongBao = maThongBao,
                MaSV = maSv,
                TrangThaiDoc = 0,
                TrangThaiThanhToan = 1,
                NgayGui = now
            });
        }

        if (newRecipients.Count > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var tongSoSinhVien = await _dbContext.ThongBaoHocPhiSinhVien
            .Where(x => x.MaThongBao == maThongBao)
            .CountAsync(cancellationToken);

        var tongSoTien = await (from tb in _dbContext.ThongBaoHocPhiSinhVien.AsNoTracking()
                                join cn in _dbContext.CongNo.AsNoTracking()
                                    on new { tb.MaSV, thongBao.MaHocKy } equals new { cn.MaSV, cn.MaHocKy }
                                where tb.MaThongBao == maThongBao
                                select cn.ConNo)
            .SumAsync(cancellationToken);

        thongBao.TongSoSinhVien = tongSoSinhVien;
        thongBao.TongSoTienCongNo = tongSoTien;
        thongBao.ThoiDiemGui = now;
        thongBao.NgayCapNhat = now;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return newRecipients.Count;
    }
}
