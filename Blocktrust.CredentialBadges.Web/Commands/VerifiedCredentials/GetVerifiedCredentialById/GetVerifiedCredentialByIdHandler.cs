namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using FluentResults;

public class GetVerifiedCredentialByIdHandler : IRequestHandler<GetVerifiedCredentialByIdRequest, Result<VerifiedCredential>>
{
    private readonly ApplicationDbContext _context;

    public GetVerifiedCredentialByIdHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VerifiedCredential>> Handle(GetVerifiedCredentialByIdRequest request, CancellationToken cancellationToken)
    {
        var credential = await _context.VerifiedCredentials.FindAsync(new object[] { request.CredentialId }, cancellationToken);
            
        if (credential == null)
        {
            return Result.Fail(new Error($"Credential with ID {request.CredentialId} not found."));
        }

        return Result.Ok(VerifiedCredential.FromEntity(credential));
    }
}