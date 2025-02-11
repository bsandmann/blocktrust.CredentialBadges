using Blocktrust.CredentialBadges.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blocktrust.CredentialBadges.Web;

public class ApplicationDbContext : DbContext 
{
    protected readonly IConfiguration Configuration;

    // Constructor that accepts DbContextOptions
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    // DbSet property
    public DbSet<VerifiedCredentialEntity> VerifiedCredentials { get; set; }
  
}