namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Blocktrust.CredentialBadges.Web.Entities;

public class GetVerifiedCredentialByIdHandler : IRequestHandler<GetVerifiedCredentialByIdRequest, Result<VerifiedCredential>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GetVerifiedCredentialByIdHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Result<VerifiedCredential>> Handle(GetVerifiedCredentialByIdRequest request, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var credentialEntity = await context.VerifiedCredentials.FindAsync(new object[] { request.CredentialId }, cancellationToken);

        if (credentialEntity == null)
        {
            return Result.Fail(new Error($"Credential with ID {request.CredentialId} not found."));
        }

        // The FromEntity() method now also handles Claims deserialization.
        var credential = VerifiedCredential.FromEntity(credentialEntity);
        return Result.Ok(credential);
    }
}