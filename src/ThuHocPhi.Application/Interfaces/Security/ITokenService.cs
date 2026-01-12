using System;
using System.Collections.Generic;
using ThuHocPhi.Domain.Entities;

namespace ThuHocPhi.Application.Interfaces.Security;

public interface ITokenService
{
    string CreateToken(NguoiDung user, IReadOnlyList<string> roles, out DateTime expiresAtUtc);
}
