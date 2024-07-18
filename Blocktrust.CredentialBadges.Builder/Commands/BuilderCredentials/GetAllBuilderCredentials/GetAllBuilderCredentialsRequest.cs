using MediatR;
using FluentResults;
using Blocktrust.CredentialBadges.Builder.Domain;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetAllBuilderCredentials
{
    public class GetAllBuilderCredentialsRequest : IRequest<Result<List<BuilderCredential>>>
    {
    }
}