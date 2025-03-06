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
    private IServiceScopeFactory _serviceScopeFactory;
    private readonly IMediator _mediator;
    private readonly ILogger<ApproveBuilderCredentialHandler> _logger;
    private readonly AppSettings _appSettings;


    public ApproveBuilderCredentialHandler(IMediator mediator, ILogger<ApproveBuilderCredentialHandler> logger, IOptions<AppSettings> appSettings, IServiceScopeFactory serviceScopeFactory)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appSettings = appSettings.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Result<BuilderCredential>> Handle(ApproveBuilderCredentialRequest request, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            // Retrieve the credential entity
            var entity = await context.BuilderCredentials
                .FirstOrDefaultAsync(c => c.CredentialId == request.CredentialId, cancellationToken);

            if (entity == null)
            {
                return Result.Fail<BuilderCredential>("Credential not found");
            }

            CreateOfferRequest offerRequest;
            if (ParseCredentialSubject(entity.CredentialSubject)["CredentialType"] == "Endorsement")
            {
                // Create offer
                offerRequest = new CreateOfferRequest
                {
                    Claims = new
                    {
                        identifier = ParseCredentialSubject(entity.CredentialSubject)["Identifier"],
                        type = new List<string> { "EndorsementSubject" },
                        name = ParseCredentialSubject(entity.CredentialSubject)["Name"], // moved from the Credential to the subject
                        description = ParseCredentialSubject(entity.CredentialSubject)["Description"], // moved from the Credential to the subject
                        endorsementComment = ParseCredentialSubject(entity.CredentialSubject)["EndorsementComment"],
                    },
                    CredentialFormat = "JWT",
                    IssuingDID = _appSettings.IssuingDID,
                    ConnectionId = entity.IssuerConnectionId,
                    AutomaticIssuance = true,
                    ValidityPeriod = 300000000
                };
            }
            else
            {
                // Assuming the Credentialtype is Achievement
                // Create offer
                offerRequest = new CreateOfferRequest
                {
                    Claims = new
                    {
                        identifier = ParseCredentialSubject(entity.CredentialSubject)["Identifier"],
                        type = new List<string> { "AchievementSubject" },
                        name = ParseCredentialSubject(entity.CredentialSubject)["Name"], // moved from the Credential to the subject
                        description = ParseCredentialSubject(entity.CredentialSubject)["Description"], // moved from the Credential to the subject
                        achievement = new
                        {
                            achievementType = ParseCredentialSubject(entity.CredentialSubject)["AchievementType"],
                            criteria = new
                            {
                                type = "Criteria",
                                narrative = ParseCredentialSubject(entity.CredentialSubject)["Criteria"]
                            },
                            fieldOfStudy = ParseCredentialSubject(entity.CredentialSubject)["FieldOfStudy"],
                            specialization = ParseCredentialSubject(entity.CredentialSubject)["Specialization"],
                        },
                        image = new
                        {
                            id = ParseCredentialSubject(entity.CredentialSubject)["Image"],
                            type = "Image"
                        },
                    },
                    CredentialFormat = "JWT",
                    IssuingDID = _appSettings.IssuingDID,
                    ConnectionId = entity.IssuerConnectionId,
                    AutomaticIssuance = true,
                    ValidityPeriod = 300000000
                };
            }


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