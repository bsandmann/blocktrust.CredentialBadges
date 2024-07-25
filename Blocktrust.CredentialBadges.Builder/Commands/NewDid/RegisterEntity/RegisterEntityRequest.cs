namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid.RegisterEntity;

using FluentResults;
using MediatR;

public class RegisterEntityRequest : IRequest<Result<Guid>>
{
    public string Name { get; set; }
    public Guid WalletId { get; set; }
}