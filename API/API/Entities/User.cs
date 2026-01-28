using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    // Refresh Tokens
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}