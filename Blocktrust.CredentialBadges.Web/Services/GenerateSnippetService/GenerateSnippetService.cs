using Blocktrust.CredentialBadges.Web.Domain;
using System.Text;

namespace Blocktrust.CredentialBadges.Web.Services.GenerateSnippetService;

public class SnippetsResult
{
    public string Snippet { get; set; }
}

public class GenerateSnippetService
{
    private readonly string _jsSnippetCdnUrl = "http://localhost:5159/credential-status-update.js";

    public SnippetsResult GenerateSnippet(VerifiedCredential credential)
    {
        var snippet = GenerateHtmlSnippet(credential);
        return new SnippetsResult { Snippet = snippet };
    }

    private string GenerateHtmlSnippet(VerifiedCredential credential)
    {
        string statusColor = GetStatusColor(credential.Status);
        string statusIcon = GetStatusIcon(credential.Status);

        // Construct the URL or route based on the credential ID
        string url = $"/credentials/{credential.Id}";

        var htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine($"<link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css\">");
        htmlBuilder.AppendLine($"<a id=\"credential-{credential.Id}\" href=\"{url}\" class=\"credential-card\">");
        htmlBuilder.AppendLine($"  <div class=\"credential-card-body\">");
        htmlBuilder.AppendLine($"    <h5 class=\"credential-card-title\">{credential.Name}</h5>");
        htmlBuilder.AppendLine($"    <p class=\"credential-card-text\">{credential.Description}</p>");
        htmlBuilder.AppendLine($"    <p id=\"credential-status-{credential.Id}\" class=\"{statusColor}\">Status: <i class=\"bi-clock-fill \"></i> </p>");
        htmlBuilder.AppendLine($"  </div>");
        htmlBuilder.AppendLine($"</a>");
        htmlBuilder.AppendLine($"<script src=\"{_jsSnippetCdnUrl}\"></script>");
        return htmlBuilder.ToString();
    }

    private string GetStatusColor(VerifiedCredential.CredentialStatus status)
    {
        return status switch
        {
            VerifiedCredential.CredentialStatus.Verified => "credential-text-success",
            VerifiedCredential.CredentialStatus.Revoked => "credential-text-danger",
            VerifiedCredential.CredentialStatus.Expired => "credential-text-warning",
            VerifiedCredential.CredentialStatus.NotDue => "credential-text-info",
            _ => "credential-text-secondary",
        };
    }

    private string GetStatusIcon(VerifiedCredential.CredentialStatus status)
    {
        return status switch
        {
            VerifiedCredential.CredentialStatus.Verified => "bi-check-circle-fill",
            VerifiedCredential.CredentialStatus.Revoked => "bi-x-circle-fill",
            VerifiedCredential.CredentialStatus.Expired => "bi-exclamation-circle-fill",
            VerifiedCredential.CredentialStatus.NotDue => "bi-clock-fill",
            _ => "bi-question-circle-fill",
        };
    }
}