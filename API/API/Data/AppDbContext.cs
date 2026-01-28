using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<User>()
            .OwnsMany(u => u.RefreshTokens);
    }
}