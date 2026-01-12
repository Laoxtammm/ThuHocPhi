using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.DTOs.CongNo;
using ThuHocPhi.Application.Interfaces.Finance;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/cong-no")]
public sealed class CongNoController : ControllerBase
{
    private readonly ICongNoService _congNoService;

    public CongNoController(ICongNoService congNoService)
    {
        _congNoService = congNoService;
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPost("tinh")]
    public async Task<IActionResult> TinhCongNo([FromBody] CongNoTinhRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _congNoService.TinhCongNoAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCongNo([FromQuery] string maSv, [FromQuery] string maHocKy, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(maSv) || string.IsNullOrWhiteSpace(maHocKy))
        {
            return BadRequest(new { message = "maSv and maHocKy are required." });
        }

        var result = await _congNoService.GetCongNoAsync(maSv, maHocKy, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
