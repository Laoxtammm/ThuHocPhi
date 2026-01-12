using System;
using System.Security.Cryptography;
using System.Text;
using ThuHocPhi.Application.Interfaces.Security;

namespace ThuHocPhi.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const string Prefix = "PBKDF2";

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return $"{Prefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string storedHash, out string? upgradedHash)
    {
        upgradedHash = null;

        if (storedHash.StartsWith($"{Prefix}$", StringComparison.Ordinal))
        {
            return VerifyPbkdf2(password, storedHash);
        }

        if (IsLegacyMd5(storedHash))
        {
            var md5 = ComputeMd5(password);
            if (string.Equals(md5, storedHash, StringComparison.OrdinalIgnoreCase))
            {
                upgradedHash = HashPassword(password);
                return true;
            }

            return false;
        }

        return false;
    }

    private static bool VerifyPbkdf2(string password, string storedHash)
    {
        var parts = storedHash.Split('$');
        if (parts.Length != 4)
        {
            return false;
        }

        if (!int.TryParse(parts[1], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[2]);
        var expectedHash = Convert.FromBase64String(parts[3]);

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static bool IsLegacyMd5(string storedHash)
    {
        if (storedHash.Length != 32)
        {
            return false;
        }

        foreach (var ch in storedHash)
        {
            var isHex = (ch >= '0' && ch <= '9') ||
                        (ch >= 'a' && ch <= 'f') ||
                        (ch >= 'A' && ch <= 'F');
            if (!isHex)
            {
                return false;
            }
        }

        return true;
    }

    private static string ComputeMd5(string input)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = md5.ComputeHash(bytes);

        var builder = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
