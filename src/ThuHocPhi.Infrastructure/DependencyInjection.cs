using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThuHocPhi.Application.Interfaces.Repositories;
using ThuHocPhi.Application.Interfaces.Security;
using ThuHocPhi.Application.Interfaces.Finance;
using ThuHocPhi.Application.Interfaces.QuanTri;
using ThuHocPhi.Infrastructure.Data;
using ThuHocPhi.Infrastructure.Repositories;
using ThuHocPhi.Infrastructure.Security;
using ThuHocPhi.Infrastructure.Services;

namespace ThuHocPhi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ThuHocPhiDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBieuPhiService, BieuPhiService>();
        services.AddScoped<ICongNoService, CongNoService>();
        services.AddScoped<IThanhToanService, ThanhToanService>();
        services.AddScoped<IBaoCaoService, BaoCaoService>();
        services.AddScoped<IThongBaoHocPhiService, ThongBaoHocPhiService>();
        services.AddScoped<IMienGiamService, MienGiamService>();
        services.AddScoped<IHocBongService, HocBongService>();
        services.AddScoped<IDoiSoatService, DoiSoatService>();
        services.AddScoped<IQuanTriNguoiDungService, QuanTriNguoiDungService>();

        return services;
    }
}
