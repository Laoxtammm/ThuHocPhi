using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.DTOs.QuanTri;
using ThuHocPhi.Application.Interfaces.QuanTri;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/quan-tri")]
public sealed class QuanTriNguoiDungController : ControllerBase
{
    private readonly IQuanTriNguoiDungService _service;

    public QuanTriNguoiDungController(IQuanTriNguoiDungService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("nguoi-dung")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("nguoi-dung/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("nguoi-dung")]
    public async Task<IActionResult> Create([FromBody] NguoiDungCreateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var id = await _service.CreateAsync(request, cancellationToken);
        if (id == 0)
        {
            return Conflict(new { message = "TenDangNhap already exists." });
        }

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("nguoi-dung/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] NguoiDungUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _service.UpdateAsync(id, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("nguoi-dung/{id:int}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _service.ResetPasswordAsync(id, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return Ok(new { success = true });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("nguoi-dung/{id:int}/vai-tro")]
    public async Task<IActionResult> UpdateRoles(int id, [FromBody] CapNhatVaiTroRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _service.UpdateRolesAsync(id, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return Ok(new { success = true });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("nguoi-dung/{id:int}/trang-thai")]
    public async Task<IActionResult> SetActive(int id, [FromQuery] bool active, CancellationToken cancellationToken)
    {
        var updated = await _service.SetActiveAsync(id, active, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return Ok(new { success = true });
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("vai-tro")]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        var roles = await _service.GetRolesAsync(cancellationToken);
        return Ok(roles);
    }
}
