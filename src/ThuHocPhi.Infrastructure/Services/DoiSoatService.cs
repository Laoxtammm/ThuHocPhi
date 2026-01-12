using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.DoiSoat;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Services;

public sealed class DoiSoatService : IDoiSoatService
{
    private readonly ThuHocPhiDbContext _dbContext;

    public DoiSoatService(ThuHocPhiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DoiSoatResultDto> DoiSoatAsync(DoiSoatRequest request, CancellationToken cancellationToken)
    {
        var maGiaoDichList = request.GiaoDichNganHang
            .Select(x => x.MaGiaoDichNganHang)
            .Distinct()
            .ToList();

        var giaoDichHeThong = await _dbContext.GiaoDich.AsNoTracking()
            .Where(x => x.MaGiaoDichNganHang != null && maGiaoDichList.Contains(x.MaGiaoDichNganHang))
            .ToListAsync(cancellationToken);

        var mapHeThong = giaoDichHeThong
            .Where(x => x.MaGiaoDichNganHang != null)
            .ToDictionary(x => x.MaGiaoDichNganHang!, x => x);

        var now = DateTime.UtcNow;
        var doiSoat = new DoiSoatGiaoDich
        {
            TuNgay = request.TuNgay,
            DenNgay = request.DenNgay,
            NguonDuLieu = request.NguonDuLieu,
            NguoiThucHien = request.NguoiThucHien,
            NgayThucHien = now
        };

        _dbContext.DoiSoatGiaoDich.Add(doiSoat);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var chiTiet = new List<DoiSoatGiaoDichChiTiet>();
        var tongTrungKhop = 0;
        var tongSaiLech = 0;
        var tongThieu = 0;

        foreach (var item in request.GiaoDichNganHang)
        {
            if (!mapHeThong.TryGetValue(item.MaGiaoDichNganHang, out var gd))
            {
                tongThieu++;
                chiTiet.Add(new DoiSoatGiaoDichChiTiet
                {
                    MaDoiSoat = doiSoat.MaDoiSoat,
                    MaGiaoDichNganHang = item.MaGiaoDichNganHang,
                    SoTienNganHang = item.SoTien,
                    NgayGiaoDich = item.NgayGiaoDich,
                    TrangThai = 3,
                    GhiChu = "Missing in system"
                });
                continue;
            }

            var isMatch = gd.SoTien == item.SoTien;
            if (isMatch)
            {
                tongTrungKhop++;
            }
            else
            {
                tongSaiLech++;
            }

            chiTiet.Add(new DoiSoatGiaoDichChiTiet
            {
                MaDoiSoat = doiSoat.MaDoiSoat,
                MaGiaoDichNganHang = item.MaGiaoDichNganHang,
                MaGiaoDichHeThong = gd.MaGiaoDich,
                SoTienNganHang = item.SoTien,
                SoTienHeThong = gd.SoTien,
                NgayGiaoDich = item.NgayGiaoDich ?? gd.NgayGiaoDich,
                TrangThai = isMatch ? (byte)1 : (byte)2,
                GhiChu = isMatch ? null : "Amount mismatch"
            });
        }

        doiSoat.TongGiaoDich = request.GiaoDichNganHang.Count;
        doiSoat.TongTrungKhop = tongTrungKhop;
        doiSoat.TongSaiLech = tongSaiLech;
        doiSoat.TongThieu = tongThieu;

        _dbContext.DoiSoatGiaoDichChiTiet.AddRange(chiTiet);
        _dbContext.NhatKyHeThong.Add(new NhatKyHeThong
        {
            MaNguoiDung = request.NguoiThucHien,
            HanhDong = "Doi soat giao dich",
            Module = "DoiSoat",
            ChiTiet = $"Tong:{doiSoat.TongGiaoDich}, Trung khop:{doiSoat.TongTrungKhop}, Sai lech:{doiSoat.TongSaiLech}, Thieu:{doiSoat.TongThieu}",
            ThoiGian = now
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DoiSoatResultDto
        {
            MaDoiSoat = doiSoat.MaDoiSoat,
            TuNgay = doiSoat.TuNgay,
            DenNgay = doiSoat.DenNgay,
            NguonDuLieu = doiSoat.NguonDuLieu,
            TongGiaoDich = doiSoat.TongGiaoDich,
            TongTrungKhop = doiSoat.TongTrungKhop,
            TongSaiLech = doiSoat.TongSaiLech,
            TongThieu = doiSoat.TongThieu,
            NguoiThucHien = doiSoat.NguoiThucHien,
            NgayThucHien = doiSoat.NgayThucHien,
            ChiTiet = chiTiet.Select(x => new DoiSoatChiTietDto
            {
                Id = x.Id,
                MaDoiSoat = x.MaDoiSoat,
                MaGiaoDichNganHang = x.MaGiaoDichNganHang,
                MaGiaoDichHeThong = x.MaGiaoDichHeThong,
                SoTienNganHang = x.SoTienNganHang,
                SoTienHeThong = x.SoTienHeThong,
                NgayGiaoDich = x.NgayGiaoDich,
                TrangThai = x.TrangThai,
                GhiChu = x.GhiChu
            }).ToList()
        };
    }

    public async Task<DoiSoatResultDto?> GetByIdAsync(long maDoiSoat, CancellationToken cancellationToken)
    {
        var header = await _dbContext.DoiSoatGiaoDich.AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaDoiSoat == maDoiSoat, cancellationToken);
        if (header is null)
        {
            return null;
        }

        var details = await _dbContext.DoiSoatGiaoDichChiTiet.AsNoTracking()
            .Where(x => x.MaDoiSoat == maDoiSoat)
            .OrderBy(x => x.Id)
            .Select(x => new DoiSoatChiTietDto
            {
                Id = x.Id,
                MaDoiSoat = x.MaDoiSoat,
                MaGiaoDichNganHang = x.MaGiaoDichNganHang,
                MaGiaoDichHeThong = x.MaGiaoDichHeThong,
                SoTienNganHang = x.SoTienNganHang,
                SoTienHeThong = x.SoTienHeThong,
                NgayGiaoDich = x.NgayGiaoDich,
                TrangThai = x.TrangThai,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);

        return new DoiSoatResultDto
        {
            MaDoiSoat = header.MaDoiSoat,
            TuNgay = header.TuNgay,
            DenNgay = header.DenNgay,
            NguonDuLieu = header.NguonDuLieu,
            TongGiaoDich = header.TongGiaoDich,
            TongTrungKhop = header.TongTrungKhop,
            TongSaiLech = header.TongSaiLech,
            TongThieu = header.TongThieu,
            NguoiThucHien = header.NguoiThucHien,
            NgayThucHien = header.NgayThucHien,
            ChiTiet = details
        };
    }
}
