using Microsoft.AspNetCore.Identity;

namespace Blocktrust.CredentialBadges.Builder.Data.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() : base() { }
    public ApplicationRole(string roleName) : base()
    {
        Name = roleName;
        NormalizedName = roleName.ToUpper();
    }
}