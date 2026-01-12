using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThuHocPhi.Web.Controllers;

[Authorize(Roles = "SinhVien")]
public sealed class StudentController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult DangKyTinChi()
    {
        return View();
    }

    public IActionResult DanhSachHocPhan()
    {
        return View();
    }

    public IActionResult TraCuuDiem()
    {
        return View();
    }

    public IActionResult ChuongTrinhDaoTao()
    {
        return View();
    }

    public IActionResult TinhTrangHocTap()
    {
        return View();
    }

    public IActionResult HocPhi()
    {
        return View();
    }

    public IActionResult ThanhToanHocPhi()
    {
        return View();
    }

    public IActionResult ThongTinSinhVien()
    {
        return View();
    }

    public IActionResult CapNhatThongTin()
    {
        return View();
    }

    public IActionResult DoiMatKhau()
    {
        return View();
    }
}
