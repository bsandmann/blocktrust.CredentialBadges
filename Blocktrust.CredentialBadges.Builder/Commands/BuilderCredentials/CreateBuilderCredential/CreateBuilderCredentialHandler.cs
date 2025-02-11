using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Data.Entities;
using Blocktrust.CredentialBadges.Builder.Domain;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.CreateBuilderCredential;

public class CreateBuilderCredentialHandler : IRequestHandler<CreateBuilderCredentialRequest, Result<BuilderCredential>>
{
    private IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CreateBuilderCredentialHandler> _logger;

    public CreateBuilderCredentialHandler(ILogger<CreateBuilderCredentialHandler> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Result<BuilderCredential>> Handle(CreateBuilderCredentialRequest request, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var entity = new BuilderCredentialEntity
        {
            CredentialId = Guid.NewGuid(),
            Date = request.Date,
            Label = request.Label,
            SubjectDid = request.SubjectDid,
            IssuerDid = request.IssuerDid,
            Status = request.Status,
            IssuerConnectionId = request.IssuerConnectionId,
            SubjectConnectionId = request.SubjectConnectionId,
            CredentialSubject = request.CredentialSubject,
            UserId = request.UserId,
            ThId = request.ThId,
            RecordIdOnAgent = request.RecordIdOnAgent,
            VerifiableCredential = request.VerifiableCredential
        };

        try
        {
            context.BuilderCredentials.Add(entity);
            await context.SaveChangesAsync(cancellationToken);

            var credential = BuilderCredential.FromEntity(entity);
            return Result.Ok(credential);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating builder credential");
            return Result.Fail<BuilderCredential>("An error occurred while creating the builder credential");
        }
    }
}