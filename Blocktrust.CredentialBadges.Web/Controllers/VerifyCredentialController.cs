namespace Blocktrust.CredentialBadges.Web.Controllers;

using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using Blocktrust.CredentialBadges.Core.Common;
using Blocktrust.CredentialBadges.OpenBadges;
using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        if (parserResult.Value is AchievementCredential)
        {
            AchievementCredential achievementCredential = parserResult.Value as AchievementCredential;

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
                SubjectId = achievementCredential.CredentialSubject.Id.ToString(),
                SubjectName = achievementCredential.CredentialSubject.Identifier,
                Status = EVerificationStatus.Verified,
                Name = achievementCredential.CredentialSubject?.Achievement?.Name ?? achievementCredential.CredentialSubject?.Id?.ToString() ?? "Unknown",
                Description = achievementCredential.CredentialSubject?.Achievement?.Description ?? "",
                Claims = achievementCredential.CredentialSubject?.Claims,
                Issuer = achievementCredential.Issuer?.Id?.ToString() ?? "Unknown Issuer",
                Image = achievementCredential.CredentialSubject?.Achievement?.Image?.Id != null ? achievementCredential.CredentialSubject.Achievement.Image.Id.ToString() : "",
                VerificationChecks = new VerifyOpenBadgeResponse
                {
                    CheckedOn = DateTime.UtcNow,
                    SignatureIsValid = verifyResponse.SignatureIsValid,
                    CredentialIsNotRevoked = verifyResponse.CredentialIsNotRevoked,
                    CredentialIsNotExpired = verifyResponse.CredentialIsNotExpired,
                    CredentialIssuanceDateIsNotInFuture = verifyResponse.CredentialIssuanceDateIsNotInFuture
                }
            };

            if (verifyResponse.CredentialIsNotExpired == false)
            {
                response.Status = EVerificationStatus.Expired;
            }
            else if (verifyResponse.CredentialIsNotRevoked == false)
            {
                response.Status = EVerificationStatus.Revoked;
            }

            else if (verifyResponse.SignatureIsValid == false)
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
        else if (parserResult.Value is EndorsementCredential)
        {
              EndorsementCredential endorsementCredential = parserResult.Value as EndorsementCredential;

            if (endorsementCredential is null)
            {
                return BadRequest(new { Message = "Invalid credential" });
            }

            var verifyResult = await _mediator.Send(new VerifyOpenBadgeRequest(endorsementCredential));

            if (verifyResult.IsFailed)
            {
                return BadRequest(new { Message = "Verification failed", Details = verifyResult.Errors });
            }

            var verifyResponse = verifyResult.Value;

            VerificationResponse response = new VerificationResponse
            {
                Id = id,
                SubjectId = endorsementCredential.CredentialSubject.Id.ToString(),
                SubjectName = endorsementCredential.CredentialSubject.Claims.TryGetValue("identifier", out var identifier) ? identifier : "",
                Status = EVerificationStatus.Verified,
                Name = endorsementCredential.CredentialSubject.Claims.TryGetValue("name", out var name) ? name : null ?? endorsementCredential.CredentialSubject?.Id?.ToString() ?? "Unknown",
                Description =endorsementCredential.CredentialSubject.Claims.TryGetValue("description", out var description) ? description : "",
                Claims = endorsementCredential.CredentialSubject?.Claims.Where(p=>p.Key.ToLowerInvariant()!="name" && p.Key.ToLowerInvariant()!="description" && p.Key.ToLowerInvariant()!="indentifier").ToDictionary(),
                Issuer = endorsementCredential.Issuer?.Id?.ToString() ?? "Unknown Issuer",
                Image = "",
                VerificationChecks = new VerifyOpenBadgeResponse
                {
                    CheckedOn = DateTime.UtcNow,
                    SignatureIsValid = verifyResponse.SignatureIsValid,
                    CredentialIsNotRevoked = verifyResponse.CredentialIsNotRevoked,
                    CredentialIsNotExpired = verifyResponse.CredentialIsNotExpired,
                    CredentialIssuanceDateIsNotInFuture = verifyResponse.CredentialIssuanceDateIsNotInFuture
                }
            };

            if (verifyResponse.CredentialIsNotExpired == false)
            {
                response.Status = EVerificationStatus.Expired;
            }
            else if (verifyResponse.CredentialIsNotRevoked == false)
            {
                response.Status = EVerificationStatus.Revoked;
            }

            else if (verifyResponse.SignatureIsValid == false)
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

        return BadRequest("Invalid credential");
    }
}