using Blocktrust.CredentialBadges.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blocktrust.CredentialBadges.Web;

public class ApplicationDbContext : DbContext 
{
    protected readonly IConfiguration Configuration;

    // Constructor that accepts DbContextOptions
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, object? o)
        :base(options)
    {
    }

    // Constructor that accepts IConfiguration and initializes the Configuration field
    public ApplicationDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }


    // Override the OnConfiguring method to set up the PostgreSQL database provider
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Connect to PostgreSQL using the connection string from app settings
        options.UseNpgsql(Configuration.GetConnectionString("CredentialBadgesDatabase"));
    }

    // DbSet property
    public DbSet<VerifiedCredentialEntity> VerifiedCredentials { get; set; }
  
}