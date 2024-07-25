namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid.GetDid;

using FluentResults;
using MediatR;

public class GetDidRequest : IRequest<Result<string>>
{
    public string ApiKey { get; set; }
}