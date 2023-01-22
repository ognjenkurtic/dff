using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace dffbackend.Models;

public class DffContext : DbContext
{
    public DffContext(DbContextOptions<DffContext> options) : base(options) { }

    public DbSet<FactoringCompany> FactoringCompanies { get; set; }

    public DbSet<Signature> Signatures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FactoringCompany>().HasData(
            new FactoringCompany()
            {
                Id = new Guid("3d45d3e4-f585-46b8-9e6b-721abf7484e4"),
                Name = "Finspot faktor",
                Email = "test@finspot.rs",
                ApiKey = "P6xSXlImETYMpojIUHE0e7E12byrqIjYUFzDTLzKBzTWf2qWV57fu2lej8CmQElN"
            }
        );
    }
}