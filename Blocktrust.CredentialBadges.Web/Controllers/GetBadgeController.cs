namespace Blocktrust.CredentialBadges.Web.Controllers;

using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using Blocktrust.CredentialBadges.Core.Common;
using Blocktrust.CredentialBadges.OpenBadges;
using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;
using Blocktrust.CredentialBadges.Web.Services.TemplatesService;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        try
        {
            // Get the credential from the database by calling the mediator with GetVerifiedCredentialByIdRequest
            Result<VerifiedCredential> credentialResult = await _mediator.Send(new GetVerifiedCredentialByIdRequest(id, true));

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

            // Create VerifiedCredential based on credential type
            var parsedCredential = parserResult.Value;
            VerifiedCredential verifiedCredential;

            if (parsedCredential is AchievementCredential achievementCredential)
            {
                verifiedCredential = ProcessAchievementCredential(achievementCredential, id, status);
            }
            else if (parsedCredential is EndorsementCredential endorsementCredential)
            {
                verifiedCredential = ProcessEndorsementCredential(endorsementCredential, id, status);
            }
            else
            {
                return BadRequest(new { Message = "Unsupported credential type" });
            }

            // Get the populated template
            var populatedTemplate = _templatesService.GetPopulatedTemplate(templateId, theme, verifiedCredential);

            // Return the populated template
            return Ok(populatedTemplate);
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error processing badge: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while processing the badge.", Details = ex.Message });
        }
    }

    private VerifiedCredential ProcessAchievementCredential(AchievementCredential achievementCredential, Guid id, EVerificationStatus status)
    {
        // Safely access properties with null checks
        var credentialTypes = achievementCredential.Type ?? new List<string>();
        var subjectType = achievementCredential.CredentialSubject?.Type ?? new List<string>();
        var achievementType = achievementCredential.CredentialSubject?.Achievement?.Type ?? new List<string>();
        
        var combinedType = credentialTypes.Concat(subjectType).Concat(achievementType).ToList();
        var filteredTypes = combinedType.Where(x => !string.IsNullOrEmpty(x) &&
                                                   !x.Equals("VerifiableCredential", StringComparison.InvariantCultureIgnoreCase) &&
                                                   !x.Equals("AchievementSubject", StringComparison.InvariantCultureIgnoreCase)).ToList();

        var name = achievementCredential.CredentialSubject?.Identifier ?? achievementCredential.CredentialSubject?.Id?.ToString() ?? "Unknown";
        var description = achievementCredential.CredentialSubject?.Achievement?.Description ?? "";
        var issuer = achievementCredential.Issuer?.Id?.ToString() ?? "Unknown Issuer";
                
        return new VerifiedCredential
        {
            Id = id,
            Name = name,
            Types = filteredTypes,
            Issuer = issuer,
            Claims =  achievementCredential.CredentialSubject?.Claims,
            Description = description,
            Image =  achievementCredential.CredentialSubject?.Image?.Id != null ? achievementCredential.CredentialSubject.Image?.Id?.ToString() : "",
            Status = status,
            ValidFrom = achievementCredential.ValidFrom ?? achievementCredential.IssuanceDate ?? DateTime.UtcNow,
            SubjectId = achievementCredential.CredentialSubject?.Id?.ToString(),
            SubjectName = GetSubjectNameFromAchievement(achievementCredential),
            Credential = achievementCredential.RawData // Make sure the credential data is passed through,
        };
    }

    private VerifiedCredential ProcessEndorsementCredential(EndorsementCredential endorsementCredential, Guid id, EVerificationStatus status)
    {
        // Create combined and filtered types
        var credentialTypes = endorsementCredential.Type ?? new List<string>();
        var subjectType = endorsementCredential.CredentialSubject?.Type ?? new List<string>();
        
        var combinedType = credentialTypes.Concat(subjectType).ToList();
        var filteredTypes = combinedType.Where(x => !string.IsNullOrEmpty(x) &&
                                                   !x.Equals("VerifiableCredential", StringComparison.InvariantCultureIgnoreCase) &&
                                                   !x.Equals("EndorsementSubject", StringComparison.InvariantCultureIgnoreCase)).ToList();

        var name = endorsementCredential.Name ?? "Endorsement";
        var description = endorsementCredential.Description ?? "";
        var issuer = endorsementCredential.Issuer?.Id?.ToString() ?? "Unknown Issuer";
        
        return new VerifiedCredential
        {
            Id = id,
            Name = name,
            Types = filteredTypes,
            Issuer = issuer,
            Claims = endorsementCredential.CredentialSubject?.Claims,
            Description = description,
            // Endorsement credentials may not have an image so we leave it null
            Image = null,
            Status = status,
            ValidFrom = endorsementCredential.ValidFrom ?? endorsementCredential.IssuanceDate ?? DateTime.UtcNow,
            SubjectId = endorsementCredential.CredentialSubject?.Id?.ToString(),
            SubjectName = GetEndorsementComment(endorsementCredential),
            Credential = endorsementCredential.RawData // Make sure the credential data is passed through
        };
    }

    private string GetSubjectNameFromAchievement(AchievementCredential credential)
    {
        // Try to find a name-like property in Claims
        if (credential.CredentialSubject?.Claims != null)
        {
            foreach (var claim in credential.CredentialSubject.Claims)
            {
                if (claim.Key.Contains("name", StringComparison.OrdinalIgnoreCase) || 
                    claim.Key.Contains("recipient", StringComparison.OrdinalIgnoreCase) ||
                    claim.Key.Contains("subject", StringComparison.OrdinalIgnoreCase))
                {
                    return claim.Value;
                }
            }
        }
        
        // Fallback to credential identifier if available
        return credential.CredentialSubject?.Identifier ?? credential.CredentialSubject?.Id?.ToString() ?? "Unknown Subject";
    }

    private string GetEndorsementComment(EndorsementCredential credential)
    {
        // Use endorsement comment if available
        if (!string.IsNullOrEmpty(credential.CredentialSubject?.EndorsementComment))
        {
            return credential.CredentialSubject.EndorsementComment;
        }
        
        // Otherwise try to find a relevant claim
        if (credential.CredentialSubject?.Claims != null)
        {
            foreach (var claim in credential.CredentialSubject.Claims)
            {
                if (claim.Key.Contains("comment", StringComparison.OrdinalIgnoreCase) || 
                    claim.Key.Contains("name", StringComparison.OrdinalIgnoreCase) ||
                    claim.Key.Contains("description", StringComparison.OrdinalIgnoreCase))
                {
                    return claim.Value;
                }
            }
        }
        
        return "Endorsement";
    }
}