using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuHocPhi.Application.DTOs.Auth;
using ThuHocPhi.Application.Interfaces.Repositories;
using ThuHocPhi.Application.Interfaces.Security;
using ThuHocPhi.Domain.Entities;
using ThuHocPhi.Infrastructure.Data;

namespace ThuHocPhi.Infrastructure.Security;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ThuHocPhiDbContext _dbContext;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ThuHocPhiDbContext dbContext)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        request.Username = request.Username?.Trim().ToLowerInvariant() ?? string.Empty;
        request.Password = request.Password?.Trim().ToLowerInvariant() ?? string.Empty;

        if (request.Username == request.Password)
        {
            var studentLogin = await TryLoginSinhVienAsync(request, cancellationToken);
            if (studentLogin is not null)
            {
                return studentLogin;
            }
        }

        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null)
        {
            return await TryLoginSinhVienAsync(request, cancellationToken);
        }

        if (!user.TrangThai)
        {
            return null;
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.MatKhauHash, out var upgradedHash))
        {
            return await TryLoginSinhVienAsync(request, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(upgradedHash))
        {
            user.MatKhauHash = upgradedHash;
        }

        user.LanDangNhapCuoi = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var roles = user.NguoiDungVaiTro
            .Select(x => x.VaiTro.TenVaiTro)
            .Distinct()
            .ToList();

        var token = _tokenService.CreateToken(user, roles, out var expiresAtUtc);

        return new LoginResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            User = new UserInfoDto
            {
                UserId = user.MaNguoiDung,
                Username = user.TenDangNhap,
                FullName = user.HoTen,
                Email = user.Email,
                Roles = roles
            }
        };
    }

    private async Task<LoginResponse?> TryLoginSinhVienAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        if (!string.Equals(request.Username, request.Password, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var sinhVien = await _dbContext.SinhVien
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaSV.ToLower() == request.Username, cancellationToken);

        if (sinhVien is null)
        {
            return null;
        }

        var pseudoUser = new NguoiDung
        {
            MaNguoiDung = 0,
            TenDangNhap = sinhVien.MaSV,
            HoTen = sinhVien.HoTen,
            Email = sinhVien.Email,
            TrangThai = true
        };

        var roles = new List<string> { "SinhVien" };
        var token = _tokenService.CreateToken(pseudoUser, roles, out var expiresAtUtc);

        return new LoginResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            User = new UserInfoDto
            {
                UserId = 0,
                Username = sinhVien.MaSV,
                FullName = sinhVien.HoTen,
                Email = sinhVien.Email,
                Roles = roles
            }
        };
    }
}
