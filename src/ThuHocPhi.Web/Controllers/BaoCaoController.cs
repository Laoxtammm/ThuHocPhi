using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.Interfaces.Finance;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/bao-cao")]
public sealed class BaoCaoController : ControllerBase
{
    private readonly IBaoCaoService _baoCaoService;

    public BaoCaoController(IBaoCaoService baoCaoService)
    {
        _baoCaoService = baoCaoService;
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpGet("cong-no-theo-khoa")]
    public async Task<IActionResult> CongNoTheoKhoa([FromQuery] string maHocKy, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(maHocKy))
        {
            return BadRequest(new { message = "maHocKy is required." });
        }

        var result = await _baoCaoService.BaoCaoCongNoTheoKhoaAsync(maHocKy, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpGet("doanh-thu")]
    public async Task<IActionResult> DoanhThu([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay, CancellationToken cancellationToken)
    {
        var result = await _baoCaoService.BaoCaoDoanhThuAsync(tuNgay, denNgay, cancellationToken);
        return Ok(result);
    }
}
