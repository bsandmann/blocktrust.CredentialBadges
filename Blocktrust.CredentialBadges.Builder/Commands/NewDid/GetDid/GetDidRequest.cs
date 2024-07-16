using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid
{
    public class GetDidRequest : IRequest<Result<string>>
    {
        public string ApiKey { get; set; }
    }
}