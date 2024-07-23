using Blocktrust.CredentialBadges.Builder.Domain;
using MediatR;
using FluentResults;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.AcceptBuilderCredential;

public class AcceptBuilderCredentialRequest : IRequest<Result<BuilderCredential>>
{
    public string ThId { get; set; }
    public string SubjectId { get; set; }
    
    public string ApiKey { get; set; }
    
}