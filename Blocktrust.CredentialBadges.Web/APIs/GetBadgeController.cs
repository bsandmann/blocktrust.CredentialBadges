using System.Text.Json;
using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using Blocktrust.CredentialBadges.OpenBadges;
using Microsoft.AspNetCore.Mvc;
using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;
using Blocktrust.CredentialBadges.Web.Services.TemplatesService;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;
using MediatR;
using FluentResults;

namespace Blocktrust.CredentialBadges.Web.APIs
{
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

            // Deserialize the credential to an AchievementCredential
            var achievementCredential = JsonSerializer.Deserialize<AchievementCredential>(credential.Credential);

            if (achievementCredential == null)
            {
                return BadRequest(new { Message = "Invalid credential" });
            }

            // Call the verification command to re-verify the credential
            Result<VerifyOpenBadgeResponse> verifyResult = await _mediator.Send(new VerifyOpenBadgeRequest(achievementCredential));

            if (verifyResult.IsFailed)
            {
                return BadRequest(new { Message = "Verification failed", Details = verifyResult.Errors });
            }

            var verifyResponse = verifyResult.Value;
            var status = EVerificationStatus.Verified;

            if (!verifyResponse.CredentialIsNotExpired != null)
            {
                status = EVerificationStatus.Expired;
            }
            else if (!verifyResponse.CredentialIsNotRevoked != null)
            {
                status = EVerificationStatus.Revoked;
            }
            else if (!verifyResponse.SignatureIsValid)
            {
                status = EVerificationStatus.Invalid;
            }
            else if (!verifyResponse.CredentialIssuanceDateIsNotInFuture)
            {
                status = EVerificationStatus.NotDue;
            }

            var verifiedCredential = new VerifiedCredential
            {
                Id = id,
                Name = achievementCredential.CredentialSubject.Achievement.Name,
                Issuer = achievementCredential.Issuer.ToString(),
                Description = achievementCredential.CredentialSubject.Achievement.Description,
                Image = achievementCredential.CredentialSubject.Achievement?.Image?.Id?.ToString(),
                Status = status,
                ValidFrom = achievementCredential.ValidFrom
            };

            // Get the populated template
            var populatedTemplate = _templatesService.GetPopulatedTemplate(templateId, theme, verifiedCredential);

            // Return the populated template
            return Ok(populatedTemplate);
        }
    }
}
