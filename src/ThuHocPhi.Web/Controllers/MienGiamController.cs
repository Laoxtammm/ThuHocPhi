using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.DTOs.MienGiam;
using ThuHocPhi.Application.Interfaces.Finance;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/mien-giam")]
public sealed class MienGiamController : ControllerBase
{
    private readonly IMienGiamService _service;

    public MienGiamController(IMienGiamService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpGet("{maChinhSach}")]
    public async Task<IActionResult> GetById(string maChinhSach, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(maChinhSach, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ChinhSachMienGiamCreateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var created = await _service.CreateAsync(request, cancellationToken);
        if (!created)
        {
            return Conflict(new { message = "MaChinhSach already exists." });
        }

        return Ok(new { message = "Created" });
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPut("{maChinhSach}")]
    public async Task<IActionResult> Update(string maChinhSach, [FromBody] ChinhSachMienGiamUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _service.UpdateAsync(maChinhSach, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpDelete("{maChinhSach}")]
    public async Task<IActionResult> Delete(string maChinhSach, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(maChinhSach, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPost("ap-dung")]
    public async Task<IActionResult> ApDung([FromBody] ApDungMienGiamRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _service.ApDungAsync(request, cancellationToken);
        return Ok(new { success = result });
    }
}
