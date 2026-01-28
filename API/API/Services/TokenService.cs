using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;

    public TokenService(UserManager<User> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<AuthTokensDto> CreateTokensAsync(User user)
    {
        var accessToken = await CreateAccessTokenAsync(user);

        var refresh = CreateRefreshToken();
        user.RefreshTokens.Add(refresh);

        TrimOldRefreshTokens(user, maxActiveTokens: 5);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to save refresh token.");

        return new AuthTokensDto
        {
            AccessToken = accessToken,
            RefreshToken = refresh.Token
        };
    }

    public async Task<AuthTokensDto?> RefreshAsync(string refreshToken)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user == null) return null;

        var token = user.RefreshTokens.Single(t => t.Token == refreshToken);

        if (!token.IsActive) return null;

        token.RevokedAt = DateTime.UtcNow;
        var newRefresh = CreateRefreshToken();
        token.ReplacedByToken = newRefresh.Token;
        user.RefreshTokens.Add(newRefresh);

        TrimOldRefreshTokens(user, maxActiveTokens: 5);

        var accessToken = await CreateAccessTokenAsync(user);

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
            throw new InvalidOperationException("Failed to rotate refresh token.");

        return new AuthTokensDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefresh.Token
        };
    }

    public async Task RevokeRefreshTokenAsync(User user, string refreshToken)
    {
        if (user.RefreshTokens == null || user.RefreshTokens.Count == 0)
        {
            user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleAsync(u => u.Id == user.Id);
        }

        var token = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken);
        if (token == null) return;

        if (token.RevokedAt == null)
            token.RevokedAt = DateTime.UtcNow;

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
            throw new InvalidOperationException("Failed to revoke refresh token.");
    }

    private async Task<string> CreateAccessTokenAsync(User user)
    {
        var jwtSection = _config.GetSection("Jwt");

        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var minutes = int.Parse(jwtSection["AccessTokenMinutes"] ?? "15");

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? "")
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        if (!string.IsNullOrWhiteSpace(user.FirstName))
            claims.Add(new Claim("firstName", user.FirstName));
        if (!string.IsNullOrWhiteSpace(user.LastName))
            claims.Add(new Claim("lastName", user.LastName));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private RefreshToken CreateRefreshToken()
    {
        var days = int.Parse(_config["Jwt:RefreshTokenDays"] ?? "14");

        return new RefreshToken
        {
            Token = RefreshToken.GenerateTokenString(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(days)
        };
    }

    private static void TrimOldRefreshTokens(User user, int maxActiveTokens)
    {
        user.RefreshTokens.RemoveAll(t => !t.IsActive && t.ExpiresAt < DateTime.UtcNow.AddDays(-7));

        var active = user.RefreshTokens.Where(t => t.IsActive).OrderByDescending(t => t.CreatedAt).ToList();
        if (active.Count <= maxActiveTokens) return;

        var toRemove = active.Skip(maxActiveTokens).ToList();
        foreach (var t in toRemove)
            user.RefreshTokens.Remove(t);
    }
}