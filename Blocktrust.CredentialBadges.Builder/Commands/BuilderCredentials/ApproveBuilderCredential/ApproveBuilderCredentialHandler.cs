using Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;
using Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.UpdateBuilderCredential;
using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Domain;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.ApproveBuilderCredential;

public class ApproveBuilderCredentialHandler : IRequestHandler<ApproveBuilderCredentialRequest, Result<BuilderCredential>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<ApproveBuilderCredentialHandler> _logger;
    private readonly AppSettings _appSettings;


    public ApproveBuilderCredentialHandler(ApplicationDbContext context, IMediator mediator, ILogger<ApproveBuilderCredentialHandler> logger,IOptions<AppSettings> appSettings)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appSettings = appSettings.Value;
    }

    public async Task<Result<BuilderCredential>> Handle(ApproveBuilderCredentialRequest request, CancellationToken cancellationToken)
    {
        try
        {


            // Retrieve the credential entity
            var entity = await _context.BuilderCredentials
                .FirstOrDefaultAsync(c => c.CredentialId == request.CredentialId, cancellationToken);

            if (entity == null)
            {
                return Result.Fail<BuilderCredential>("Credential not found");
            }

            // Create offer
            var offerRequest = new CreateOfferRequest
            {
                Claims = new
                {
                    type = new List<string> { "AchievementSubject" },
                    achievement = new
                    {
                        id = "urn:uuid:" + Guid.NewGuid(),
                        type = new List<string> { "Achievement" },
                        achievementType = ParseCredentialSubject(entity.CredentialSubject)["AchievementType"],
                        name = ParseCredentialSubject(entity.CredentialSubject)["Name"],
                        description = ParseCredentialSubject(entity.CredentialSubject)["Description"],
                        criteria = new
                        {
                            type = "Criteria",
                            narrative = ParseCredentialSubject(entity.CredentialSubject)["Criteria"]
                        },
                        image = new
                        {
                            id = ParseCredentialSubject(entity.CredentialSubject)["Image"],
                            type = "Image"
                        }
                    }
                },
                CredentialFormat = "JWT",
                IssuingDID = _appSettings.IssuingDID,
                ConnectionId = entity.IssuerConnectionId,
                AutomaticIssuance = true,
                ValidityPeriod = 300000000
            };

            var offerResult = await _mediator.Send(offerRequest, cancellationToken);

            if (offerResult.IsFailed)
            {
                return Result.Fail<BuilderCredential>("Failed to create offer");
            }

            var offerResponse = offerResult.Value;
     

        // Update credential status to WaitingAcceptance and set ThId
        var updateRequest = new UpdateBuilderCredentialRequest
        {
            CredentialId = request.CredentialId,
            Status = EBuilderCredentialStatus.WaitingAcceptance,
            ThId = Guid.Parse(offerResponse.Thid), 
            RecordIdOnAgent = entity.RecordIdOnAgent
        };

        var updateResult = await _mediator.Send(updateRequest, cancellationToken);

        if (updateResult.IsFailed)
        {
            return Result.Fail<BuilderCredential>("Failed to update credential");
        }

        return Result.Ok(updateResult.Value);
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ApproveBuilderCredentialRequest");
            return Result.Fail<BuilderCredential>("An error occurred while processing the request.");
        }
    }

    private Dictionary<string, string> ParseCredentialSubject(string credentialSubject)
    {
        // Assuming credentialSubject is a JSON string, deserialize it into a dictionary
        // Using Newtonsoft.Json
        var json = Newtonsoft.Json.Linq.JObject.Parse(credentialSubject);
        var fields = new Dictionary<string, string>();

        foreach (var property in json.Properties())
        {
            fields[property.Name] = property.Value.ToString();
        }

        return fields;
    }
}