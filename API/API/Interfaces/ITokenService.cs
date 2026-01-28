using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
    Task<AuthTokensDto> CreateTokensAsync(User user);
    Task<AuthTokensDto?> RefreshAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(User user, string refreshToken);
}