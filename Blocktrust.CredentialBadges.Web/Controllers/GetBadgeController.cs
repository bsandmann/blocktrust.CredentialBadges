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

        // Extract or create claims
        var claims = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Add existing claims if available
        if (achievementCredential.CredentialSubject?.Claims != null)
        {
            foreach (var claim in achievementCredential.CredentialSubject.Claims)
            {
                claims[claim.Key] = claim.Value;
            }
        }

        // Get name and description - prioritize from claims first, then from achievement properties
        var name = claims.TryGetValue("name", out var nameFromClaims) && !string.IsNullOrEmpty(nameFromClaims)
            ? nameFromClaims
            : achievementCredential.CredentialSubject?.Achievement?.Name ?? "Achievement";

        var description = claims.TryGetValue("description", out var descFromClaims) && !string.IsNullOrEmpty(descFromClaims)
            ? descFromClaims
            : achievementCredential.CredentialSubject?.Achievement?.Description ?? "";

        var issuer = achievementCredential.Issuer?.Id?.ToString() ?? "Unknown Issuer";

        // Update claims with final values (in case they weren't in claims originally)
        claims["name"] = name;
        claims["description"] = description;

        // Extract criteria if available
        if (achievementCredential.CredentialSubject?.Achievement?.Criteria != null)
        {
            var criteria = achievementCredential.CredentialSubject.Achievement.Criteria;
            if (!string.IsNullOrEmpty(criteria.Narrative))
            {
                claims["criteria"] = criteria.Narrative;
            }
        }

        // Extract additional fields from achievement
        if (achievementCredential.CredentialSubject?.Achievement != null)
        {
            var achievement = achievementCredential.CredentialSubject.Achievement;

            // Check for field of study or specialization in claims or Achievement object
            var fieldOfStudyProps = new[] { "fieldOfStudy", "field_of_study", "fieldofstudy", "field" };
            var specializationProps = new[] { "specialization", "specializationArea", "specialization_area" };

            foreach (var prop in fieldOfStudyProps)
            {
                if (claims.ContainsKey(prop) && !string.IsNullOrEmpty(claims[prop]))
                {
                    claims["fieldOfStudy"] = claims[prop];
                    break;
                }
            }

            foreach (var prop in specializationProps)
            {
                if (claims.ContainsKey(prop) && !string.IsNullOrEmpty(claims[prop]))
                {
                    claims["specialization"] = claims[prop];
                    break;
                }
            }
        }

        // Get validUntil from credential
        var validUntil = achievementCredential.ValidUntil ?? achievementCredential.ExpirationDate ?? null;

        // Get image from either Achievement image or Subject image
        string? imageUrl = null;
        if (achievementCredential.CredentialSubject?.Achievement?.Image?.Id != null)
        {
            imageUrl = achievementCredential.CredentialSubject.Achievement.Image.Id.ToString();
        }
        else if (achievementCredential.CredentialSubject?.Image?.Id != null)
        {
            imageUrl = achievementCredential.CredentialSubject.Image.Id.ToString();
        }

        return new VerifiedCredential
        {
            Id = id,
            Name = name,
            Types = filteredTypes,
            Issuer = issuer,
            Claims = claims,
            Description = description,
            Image = imageUrl,
            Status = status,
            ValidFrom = achievementCredential.ValidFrom ?? achievementCredential.IssuanceDate ?? DateTime.UtcNow,
            ValidUntil = validUntil, // Set the ValidUntil property explicitly
            SubjectId = achievementCredential.CredentialSubject?.Id?.ToString(),
            SubjectName = GetSubjectNameFromAchievement(achievementCredential),
            Credential = achievementCredential.RawData // Make sure the credential data is passed through
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

        // Extract or create claims
        var claims = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Add existing claims if available
        if (endorsementCredential.CredentialSubject?.Claims != null)
        {
            foreach (var claim in endorsementCredential.CredentialSubject.Claims)
            {
                claims[claim.Key] = claim.Value;
            }
        }

        // Get name and description - prioritize from claims first, then from endorsement properties
        var name = claims.TryGetValue("name", out var nameFromClaims) && !string.IsNullOrEmpty(nameFromClaims)
            ? nameFromClaims
            : endorsementCredential.Name ?? "Endorsement";

        var description = claims.TryGetValue("description", out var descFromClaims) && !string.IsNullOrEmpty(descFromClaims)
            ? descFromClaims
            : endorsementCredential.Description ?? "";

        var issuer = endorsementCredential.Issuer?.Id?.ToString() ?? "Unknown Issuer";

        // Update claims with final values (in case they weren't in claims originally)
        claims["name"] = name;
        claims["description"] = description;

        // Add endorsement comment as a claim if available
        if (!string.IsNullOrEmpty(endorsementCredential.CredentialSubject?.EndorsementComment))
        {
            claims["endorsementComment"] = endorsementCredential.CredentialSubject.EndorsementComment;
        }

        // Get validUntil from credential
        var validUntil = endorsementCredential.ValidUntil ?? endorsementCredential.ExpirationDate ?? null;

        return new VerifiedCredential
        {
            Id = id,
            Name = name,
            Types = filteredTypes,
            Issuer = issuer,
            Claims = claims,
            Description = description,
            // Endorsement credentials may not have an image so we leave it null
            Image = null,
            Status = status,
            ValidFrom = endorsementCredential.ValidFrom ?? endorsementCredential.IssuanceDate ?? DateTime.UtcNow,
            ValidUntil = validUntil, // Set the ValidUntil property explicitly
            SubjectId = endorsementCredential.CredentialSubject?.Id?.ToString(),
            SubjectName = GetEndorsementComment(endorsementCredential),
            Credential = endorsementCredential.RawData // Make sure the credential data is passed through
        };
    }

    private string GetSubjectNameFromAchievement(AchievementCredential credential)
    {
        if (!string.IsNullOrEmpty(credential.CredentialSubject.Identifier))
        {
            return credential.CredentialSubject.Identifier;
        }

        // Try to find properties in Claims with specific priority order
        if (credential.CredentialSubject?.Claims != null)
        {
            // First try to get identifier
            foreach (var claim in credential.CredentialSubject.Claims)
            {
                if (claim.Key.Contains("identifier", StringComparison.OrdinalIgnoreCase))
                {
                    return claim.Value;
                }
            }

            // Second try to get name
            foreach (var claim in credential.CredentialSubject.Claims)
            {
                if (claim.Key.Contains("name", StringComparison.OrdinalIgnoreCase))
                {
                    return claim.Value;
                }
            }

            // Third try to get subject
            foreach (var claim in credential.CredentialSubject.Claims)
            {
                if (claim.Key.Contains("subject", StringComparison.OrdinalIgnoreCase))
                {
                    return claim.Value;
                }
            }
        }

        // Fallback to credential identifier or id if available
        return credential.CredentialSubject?.Identifier ?? credential.CredentialSubject?.Id?.ToString() ?? "Unknown Subject";
    }

    private string GetEndorsementComment(EndorsementCredential credential)
    {
        // First check for EndorsementComment
        if (!string.IsNullOrEmpty(credential.CredentialSubject?.EndorsementComment))
        {
            return credential.CredentialSubject.EndorsementComment;
        }

        // Then check for endorsement_comment in claims
        if (credential.CredentialSubject?.Claims != null)
        {
            foreach (var claim in credential.CredentialSubject.Claims)
            {
                if (claim.Key.Contains("endorsement_comment", StringComparison.OrdinalIgnoreCase))
                {
                    return claim.Value;
                }
            }
        }

        // Return empty string as fallback
        return "";
    }
}