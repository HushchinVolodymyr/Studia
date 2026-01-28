using System.Security.Cryptography;

namespace API.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt == null && !IsExpired;

    public static string GenerateTokenString()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}