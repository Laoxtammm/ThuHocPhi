using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.DTOs.BieuPhi;
using ThuHocPhi.Application.Interfaces.Finance;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/bieu-phi")]
public sealed class BieuPhiController : ControllerBase
{
    private readonly IBieuPhiService _bieuPhiService;

    public BieuPhiController(IBieuPhiService bieuPhiService)
    {
        _bieuPhiService = bieuPhiService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? maHocKy, CancellationToken cancellationToken)
    {
        var result = await _bieuPhiService.GetAllAsync(maHocKy, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _bieuPhiService.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BieuPhiCreateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var id = await _bieuPhiService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] BieuPhiUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _bieuPhiService.UpdateAsync(id, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _bieuPhiService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
