using Blocktrust.CredentialBadges.Builder.Domain;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.CreateBuilderCredential;

public class CreateBuilderCredentialRequest : IRequest, IRequest<Result<BuilderCredential>>
{
    public DateTime Date { get; set; }
    public string Label { get; set; }
    public string SubjectDid { get; set; }
    public string IssuerDid { get; set; }
    public EBuilderCredentialStatus Status { get; set; }
    public Guid IssuerConnectionId { get; set; }
    public Guid SubjectConnectionId { get; set; }
    public string CredentialSubject { get; set; }
}