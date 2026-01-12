using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.DTOs.ThanhToan;
using ThuHocPhi.Application.Interfaces.Finance;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/thanh-toan")]
public sealed class ThanhToanController : ControllerBase
{
    private readonly IThanhToanService _thanhToanService;

    public ThanhToanController(IThanhToanService thanhToanService)
    {
        _thanhToanService = thanhToanService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ThanhToan([FromBody] ThanhToanRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _thanhToanService.ThanhToanAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("giao-dich")]
    public async Task<IActionResult> GetGiaoDich([FromQuery] string? maSv, [FromQuery] string? maHocKy, CancellationToken cancellationToken)
    {
        var result = await _thanhToanService.GetGiaoDichAsync(maSv, maHocKy, cancellationToken);
        return Ok(result);
    }
}
