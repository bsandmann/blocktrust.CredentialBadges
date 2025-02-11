namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.UpdateCredentialTemplateId;

using FluentResults;
using MediatR;

public class UpdateCredentialTemplateIdRequest : IRequest<Result>
{
    public Guid CredentialId { get; set; }
    public string TemplateId { get; set; }

    public UpdateCredentialTemplateIdRequest(Guid credentialId, string templateId)
    {
        CredentialId = credentialId;
        TemplateId = templateId;
    }
}