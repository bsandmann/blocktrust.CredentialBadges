using Microsoft.AspNetCore.Mvc;
using MediatR;
using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;
using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using System.Text.Json;
using Blocktrust.CredentialBadges.OpenBadges;

namespace Blocktrust.CredentialBadges.Web.APIs;

[ApiController]
[Route("api/[controller]")]
public class VerifyCredentialController : ControllerBase
{
    private readonly IMediator _mediator;

    public VerifyCredentialController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCredentialById(Guid id)
    {
        // Get the credential from the database by calling the mediator with GetVerifiedCredentialByIdRequest
        var credentialResult = await _mediator.Send(new GetVerifiedCredentialByIdRequest(id));
        
        if (credentialResult.IsFailed)
        {
            return NotFound(new { Message = "Credential not found" });
        }

        var credential = credentialResult.Value;

        // Deserialize the credential to an AchievementCredential
        var achievementCredential = JsonSerializer.Deserialize<AchievementCredential>(credential.Credential);

        // Call the verification command to re-verify the credential
        
        if (achievementCredential is null)
        {
            return BadRequest(new { Message = "Invalid credential" });
        }
        
        var verifyResult = await _mediator.Send(new VerifyOpenBadgeRequest(achievementCredential));

        if (verifyResult.IsFailed)
        {
            return BadRequest(new { Message = "Verification failed", Details = verifyResult.Errors });
        }

        var verifyResponse = verifyResult.Value;

        // Return the id and status of the verification
        return Ok(new
        {
            Id = id,
            Status = verifyResponse.VerificationIsSuccessfull()
        });
    }
}