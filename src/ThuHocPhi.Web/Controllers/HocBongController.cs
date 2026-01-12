using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.DTOs.HocBong;
using ThuHocPhi.Application.Interfaces.Finance;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/hoc-bong")]
public sealed class HocBongController : ControllerBase
{
    private readonly IHocBongService _service;

    public HocBongController(IHocBongService service)
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
    [HttpGet("{maHocBong}")]
    public async Task<IActionResult> GetById(string maHocBong, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(maHocBong, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HocBongCreateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var created = await _service.CreateAsync(request, cancellationToken);
        if (!created)
        {
            return Conflict(new { message = "MaHocBong already exists." });
        }

        return Ok(new { message = "Created" });
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPut("{maHocBong}")]
    public async Task<IActionResult> Update(string maHocBong, [FromBody] HocBongUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _service.UpdateAsync(maHocBong, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpDelete("{maHocBong}")]
    public async Task<IActionResult> Delete(string maHocBong, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(maHocBong, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPost("ap-dung")]
    public async Task<IActionResult> ApDung([FromBody] ApDungHocBongRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _service.ApDungAsync(request, cancellationToken);
        return Ok(new { success = result });
    }
}
