using Microsoft.AspNetCore.Mvc;

namespace Blocktrust.CredentialBadges.Web.APIs;

[ApiController]
[Route("api/[controller]")]
public class VerifyCredentialController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetCredentialById(Guid id)
    {
        var defaultCredential = new
        {
            Id = id,
            Name = "Sample Credential",
            Description = "This is a sample credential for demonstration purposes.",
            Status = "Verified"
        };

        return Ok(defaultCredential);
    }
}