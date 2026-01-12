using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuHocPhi.Application.DTOs.Auth;
using ThuHocPhi.Application.Interfaces.Security;

namespace ThuHocPhi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<UserInfoDto> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var fullName = User.FindFirstValue("full_name") ?? string.Empty;
        var roles = User.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList();

        var dto = new UserInfoDto
        {
            UserId = int.TryParse(userId, out var idValue) ? idValue : 0,
            Username = username,
            FullName = fullName,
            Roles = roles
        };

        return Ok(dto);
    }
}
