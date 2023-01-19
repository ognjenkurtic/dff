using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace dffbackend.Models;

public class DffContext : DbContext
{
    public DffContext(DbContextOptions<DffContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.Ignore<IdentityRole>();
        // modelBuilder.Ignore<IdentityUserToken<string>>();
        // modelBuilder.Ignore<IdentityUserRole<string>>();
        // modelBuilder.Ignore<IdentityUserLogin<string>>();
        // modelBuilder.Ignore<IdentityUserClaim<string>>();
        // modelBuilder.Ignore<IdentityRoleClaim<string>>();
    }
}