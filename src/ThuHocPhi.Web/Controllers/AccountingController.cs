using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThuHocPhi.Web.Controllers;

[Authorize(Roles = "PhongTaiChinh")]
public sealed class AccountingController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Dashboard));
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult MienGiam()
    {
        return View();
    }

    public IActionResult HocBong()
    {
        return View();
    }

    public IActionResult ThanhToanSinhVien()
    {
        return View();
    }
}
