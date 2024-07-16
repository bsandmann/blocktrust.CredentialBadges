using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterEntityRequest : IRequest<Result<string>>, IRequest<Result<Guid>>
{
    public string Name { get; set; }
    public Guid WalletId { get; set; }
}