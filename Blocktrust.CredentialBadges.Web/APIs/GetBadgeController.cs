using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using Blocktrust.CredentialBadges.Core.Common;
using Blocktrust.CredentialBadges.OpenBadges;
using Microsoft.AspNetCore.Mvc;
using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;
using Blocktrust.CredentialBadges.Web.Services.TemplatesService;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;
using MediatR;
using FluentResults;

namespace Blocktrust.CredentialBadges.Web.APIs;

[ApiController]
[Route("api/[controller]")]

public class GetBadgeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly TemplatesService _templatesService;

    public GetBadgeController(IMediator mediator, TemplatesService templatesService)
    {
        _mediator = mediator;
        _templatesService = templatesService;
    }

    [HttpGet("{id}/{templateId}/{theme}")]
    public async Task<IActionResult> GetBadge(Guid id, string templateId, string theme)
    {
        // Get the credential from the database by calling the mediator with GetVerifiedCredentialByIdRequest
        Result<VerifiedCredential> credentialResult = await _mediator.Send(new GetVerifiedCredentialByIdRequest(id));

        if (credentialResult.IsFailed)
        {
            return NotFound(new { Message = "Credential not found" });
        }

        var credential = credentialResult.Value;

        // Check if the credential has a domain restriction
        if (!string.IsNullOrEmpty(credential.Domain))
        {
            // Get the origin of the request
            var origin = Request.Headers["Origin"].ToString();
            if (string.IsNullOrEmpty(origin))
            {
                // If no origin header, try to get the referer
                origin = Request.Headers["Referer"].ToString();
            }

            // Parse the origin to get just the domain
            if (!string.IsNullOrEmpty(origin))
            {
                Uri uri = new Uri(origin);
                string requestDomain = uri.Host;

                // Compare the request domain with the credential's domain
                if (!requestDomain.Equals(credential.Domain, StringComparison.OrdinalIgnoreCase))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, 
                        new { Message = "Access to this badge is restricted to the specified domain" });
                }
            }
            else
            {
                // If we can't determine the origin, deny access
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new { Message = "Unable to verify request origin" });
            }
        }

            
        var parserResult = CredentialParser.Parse(credential.Credential);
        if (parserResult.IsFailed)
        {
            return BadRequest(new { Message = "Invalid credential", Details = parserResult.Errors });
        }

        // Call the verification command to re-verify the credential
        Result<VerifyOpenBadgeResponse> verifyResult = await _mediator.Send(new VerifyOpenBadgeRequest(parserResult.Value));

        if (verifyResult.IsFailed)
        {
            return BadRequest(new { Message = "Verification failed", Details = verifyResult.Errors });
        }
        var verifyResponse = verifyResult.Value;
        var status = EVerificationStatus.Verified;

        if (verifyResponse.CredentialIsNotExpired.HasValue && !verifyResponse.CredentialIsNotExpired.Value)
        {
            status = EVerificationStatus.Expired;
        }
        else if (verifyResponse.CredentialIsNotRevoked.HasValue && !verifyResponse.CredentialIsNotRevoked.Value)
        {
            status = EVerificationStatus.Revoked;
        }
                
        if (!verifyResponse.SignatureIsValid)
        {
            status = EVerificationStatus.Invalid;
        }
        else if (!verifyResponse.CredentialIssuanceDateIsNotInFuture)
        {
            status = EVerificationStatus.NotDue;
        }
            
        AchievementCredential achievementCredential = parserResult.Value as AchievementCredential;

        var verifiedCredential = new VerifiedCredential
        {
            Id = id,
            Name = achievementCredential.CredentialSubject.Achievement.Name,
            Issuer = achievementCredential.Issuer.Id.ToString(),
            Description = achievementCredential.CredentialSubject.Achievement.Description,
            Image = achievementCredential.CredentialSubject.Achievement?.Image?.Id?.ToString(),
            Status = status,
            ValidFrom = achievementCredential.ValidFrom ?? achievementCredential.IssuanceDate.Value,
        };

        // Get the populated template
        var populatedTemplate = _templatesService.GetPopulatedTemplate(templateId, theme, verifiedCredential);

        // Return the populated template
        return Ok(populatedTemplate);
    }
}