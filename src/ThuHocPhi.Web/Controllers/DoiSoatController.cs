using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.DTOs.DoiSoat;
using ThuHocPhi.Application.Interfaces.Finance;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/doi-soat")]
public sealed class DoiSoatController : ControllerBase
{
    private readonly IDoiSoatService _service;

    public DoiSoatController(IDoiSoatService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Administrator,PhongTaiChinh")]
    [HttpPost]
    public async Task<IActionResult> DoiSoat([FromBody] DoiSoatRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _service.DoiSoatAsync(request, cancellationToken);
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
}
