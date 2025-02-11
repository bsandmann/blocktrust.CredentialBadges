namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using FluentResults;

public class GetVerifiedCredentialByIdHandler : IRequestHandler<GetVerifiedCredentialByIdRequest, Result<VerifiedCredential>>
{
    private IServiceScopeFactory _serviceScopeFactory;

    public GetVerifiedCredentialByIdHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Result<VerifiedCredential>> Handle(GetVerifiedCredentialByIdRequest request, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var credential = await context.VerifiedCredentials.FindAsync(new object[] { request.CredentialId }, cancellationToken);
            
        if (credential == null)
        {
            return Result.Fail(new Error($"Credential with ID {request.CredentialId} not found."));
        }

        return Result.Ok(VerifiedCredential.FromEntity(credential));
    }
}