using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThuHocPhi.Web.Controllers;

[Authorize(Roles = "Administrator")]
public sealed class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult QuanLyNguoiDung()
    {
        return View();
    }

    public IActionResult BieuPhi()
    {
        return View();
    }

    public IActionResult ThongBaoHocPhi()
    {
        return View();
    }

    public IActionResult MienGiamHocBong()
    {
        return View();
    }

    public IActionResult DoiSoatGiaoDich()
    {
        return View();
    }

    public IActionResult BaoCao()
    {
        return View();
    }
}
