using System.Collections.Generic;

namespace ThuHocPhi.Application.DTOs.Auth;

public sealed class UserInfoDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = new List<string>();
}
