using Microsoft.AspNetCore.Mvc;
using MediatR;
using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;
using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using System.Text.Json;
using Blocktrust.CredentialBadges.Core.Common;
using Blocktrust.CredentialBadges.OpenBadges;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;

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
        
        var parserResult = CredentialParser.Parse(credential.Credential);
        if (parserResult.IsFailed)
        {
            return BadRequest(new { Message = "Invalid credential", Details = parserResult.Errors });
        }

        AchievementCredential achievementCredential = parserResult.Value as AchievementCredential;


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
  
        VerificationResponse response = new VerificationResponse
        {
            Id = id,
            Status = EVerificationStatus.Verified,
            Name = achievementCredential.CredentialSubject.Achievement.Name,
            Description = achievementCredential.CredentialSubject.Achievement.Description,
            Image = achievementCredential.CredentialSubject.Achievement?.Image?.Id!=null?achievementCredential.CredentialSubject.Achievement.Image.Id.ToString():"",
            VerificationChecks = new VerifyOpenBadgeResponse
            {
                SignatureIsValid = verifyResponse.SignatureIsValid,
                CredentialIsNotRevoked = verifyResponse.CredentialIsNotRevoked,
                CredentialIsNotExpired = verifyResponse.CredentialIsNotExpired,
                CredentialIssuanceDateIsNotInFuture = verifyResponse.CredentialIssuanceDateIsNotInFuture
            }
         
        };
        
        if(verifyResponse.CredentialIsNotExpired == false)
        {
            response.Status = EVerificationStatus.Expired;
        }
        else if(verifyResponse.CredentialIsNotRevoked == false)
        {
            response.Status = EVerificationStatus.Revoked;
        }
     
        else if(verifyResponse.SignatureIsValid == false)
        {
            response.Status = EVerificationStatus.Invalid;
        }
        else if (verifyResponse.CredentialIssuanceDateIsNotInFuture == false)
        {
            response.Status = EVerificationStatus.NotDue;
        }
        // Return the id and status of the verification
        return Ok(response);
    }
}


