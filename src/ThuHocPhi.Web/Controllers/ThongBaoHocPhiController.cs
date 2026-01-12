using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.DTOs.ThongBao;
using ThuHocPhi.Application.Interfaces.Finance;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/thong-bao-hoc-phi")]
public sealed class ThongBaoHocPhiController : ControllerBase
{
    private readonly IThongBaoHocPhiService _service;

    public ThongBaoHocPhiController(IThongBaoHocPhiService service)
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
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ThongBaoHocPhiCreateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var id = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] ThongBaoHocPhiUpdateRequest request, CancellationToken cancellationToken)
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

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpGet("{id:long}/recipients")]
    public async Task<IActionResult> GetRecipients(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetRecipientsAsync(id, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPost("{id:long}/recipients")]
    public async Task<IActionResult> AssignRecipients(long id, [FromBody] ThongBaoHocPhiAssignRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var count = await _service.AssignRecipientsAsync(id, request, cancellationToken);
        if (count == 0)
        {
            return NotFound(new { message = "Thong bao not found or no recipients added." });
        }

        return Ok(new { added = count });
    }
}
