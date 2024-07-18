using Blocktrust.CredentialBadges.Builder.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blocktrust.CredentialBadges.Builder.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public ApplicationDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Name=BuilderDbConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var adminRole = new ApplicationRole
        {
            Id = Guid.NewGuid(),
            Name = "adminRole",
            NormalizedName = "ADMINROLE"
        };

        var nonAdminRole = new ApplicationRole
        {
            Id = Guid.NewGuid(), 
            Name = "nonAdminRole",
            NormalizedName = "NONADMINROLE"
        };

        builder.Entity<ApplicationRole>().HasData(adminRole, nonAdminRole);
    }
    
    public DbSet<BuilderCredentialEntity> BuilderCredentials { get; set; }

}