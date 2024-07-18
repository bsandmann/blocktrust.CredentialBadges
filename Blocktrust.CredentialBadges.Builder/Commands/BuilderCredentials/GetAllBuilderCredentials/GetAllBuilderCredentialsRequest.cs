using Blocktrust.CredentialBadges.Builder.Domain;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetAllBuilderCredentials;

public class GetAllBuilderCredentialsRequest : IRequest<List<BuilderCredential>>
{
}