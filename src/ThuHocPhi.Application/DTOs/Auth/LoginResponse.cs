using System;

namespace ThuHocPhi.Application.DTOs.Auth;

public sealed class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public UserInfoDto User { get; set; } = new UserInfoDto();
}
