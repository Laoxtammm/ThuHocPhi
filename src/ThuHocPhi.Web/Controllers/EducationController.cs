using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThuHocPhi.Web.Controllers;

[Authorize(Roles = "PhongDaoTao")]
public sealed class EducationController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction(nameof(QuanLyHocPhan));
    }

    public IActionResult QuanLyHocPhan()
    {
        return View();
    }

    public IActionResult HocPhanBatBuoc()
    {
        return View();
    }

    public IActionResult HocPhanDieuKien()
    {
        return View();
    }

    public IActionResult HocPhanTuChon()
    {
        return View();
    }

    public IActionResult TinChiToiThieu()
    {
        return View();
    }

    public IActionResult ThongBaoKetQua()
    {
        return View();
    }
}
