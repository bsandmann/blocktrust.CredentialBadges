using Blocktrust.CredentialBadges.Web.Domain;
using System.Text;

namespace Blocktrust.CredentialBadges.Web.Services.GenerateSnippetService;

public class SnippetsResult
{
    public string HtmlSnippet { get; set; }
    public string CssSnippet { get; set; }
    public string JsSnippet { get; set; }
}

public class GenerateSnippetService
{
    public SnippetsResult GenerateSnippets(VerifiedCredential credential)
    {
        var htmlSnippet = GenerateHtmlSnippet(credential);
        var cssSnippet = GenerateCssSnippet();
        var jsSnippet = GenerateJavaScriptSnippet(credential);

        return new SnippetsResult
        {
            HtmlSnippet = htmlSnippet,
            CssSnippet = cssSnippet,
            JsSnippet = jsSnippet
        };
    }

    private string GenerateHtmlSnippet(VerifiedCredential credential)
    {
        string statusColor = GetStatusColor(credential.Status);
        string statusIcon = GetStatusIcon(credential.Status);

        // Construct the URL or route based on the credential ID
        string url = $"/credentials/{credential.Id}";

        var htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine($"<link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css\">");
        htmlBuilder.AppendLine($"<a href=\"{url}\" class=\"card\">");
        htmlBuilder.AppendLine($"  <div class=\"card-body\">");
        htmlBuilder.AppendLine($"    <h5 class=\"card-title\">{credential.Name}</h5>");
        htmlBuilder.AppendLine($"    <p class=\"card-text\">{credential.Description}</p>");
        htmlBuilder.AppendLine($"    <p id=\"credential-status\" class=\"{statusColor}\">Status: <i id=\"credential-status-icon\" class=\"bi {statusIcon}\"></i> {credential.Status}</p>");
        htmlBuilder.AppendLine($"  </div>");
        htmlBuilder.AppendLine($"</a>");

        return htmlBuilder.ToString();
    }

    private string GenerateCssSnippet()
    {
        var cssBuilder = new StringBuilder();
        cssBuilder.AppendLine(".card {");
        cssBuilder.AppendLine("  border: 1px solid #ccc;");
        cssBuilder.AppendLine("  border-radius: 5px;");
        cssBuilder.AppendLine("  padding: 10px;");
        cssBuilder.AppendLine("  box-shadow: 0 0 5px rgba(0, 0, 0, 0.1);");
        cssBuilder.AppendLine("  margin-bottom: 10px;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".card-title {");
        cssBuilder.AppendLine("  font-size: 1.2rem;");
        cssBuilder.AppendLine("  font-weight: bold;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".card-text {");
        cssBuilder.AppendLine("  margin-top: 10px;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".text-success {");
        cssBuilder.AppendLine("  color: green;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".text-danger {");
        cssBuilder.AppendLine("  color: red;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".text-warning {");
        cssBuilder.AppendLine("  color: orange;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".text-info {");
        cssBuilder.AppendLine("  color: blue;");
        cssBuilder.AppendLine("}");

        return cssBuilder.ToString();
    }

    private string GenerateJavaScriptSnippet(VerifiedCredential credential)
    {
        var jsBuilder = new StringBuilder();
        jsBuilder.AppendLine($"const credentialId = '{credential.Id}';");
        jsBuilder.AppendLine("document.addEventListener('DOMContentLoaded', function() {");
        jsBuilder.AppendLine("  updateStatus();");
        jsBuilder.AppendLine("});");
        jsBuilder.AppendLine("function updateStatus() {");
        // jsBuilder.AppendLine("  fetch(`/api/credentials/status/${credentialId}`)");
        // jsBuilder.AppendLine("    .then(response => response.json())");
        // jsBuilder.AppendLine("    .then(data => {");
        // jsBuilder.AppendLine("      const statusElement = document.getElementById('credential-status');");
        // jsBuilder.AppendLine("      const statusIconElement = document.getElementById('credential-status-icon');");
        // jsBuilder.AppendLine("      let statusColor, statusIcon;");
        // jsBuilder.AppendLine("      switch(data.status) {");
        // jsBuilder.AppendLine("        case 'Verified':");
        // jsBuilder.AppendLine("          statusColor = 'text-success';");
        // jsBuilder.AppendLine("          statusIcon = 'bi-check-circle-fill';");
        // jsBuilder.AppendLine("          break;");
        // jsBuilder.AppendLine("        case 'Revoked':");
        // jsBuilder.AppendLine("          statusColor = 'text-danger';");
        // jsBuilder.AppendLine("          statusIcon = 'bi-x-circle-fill';");
        // jsBuilder.AppendLine("          break;");
        // jsBuilder.AppendLine("        case 'Expired':");
        // jsBuilder.AppendLine("          statusColor = 'text-warning';");
        // jsBuilder.AppendLine("          statusIcon = 'bi-exclamation-circle-fill';");
        // jsBuilder.AppendLine("          break;");
        // jsBuilder.AppendLine("        case 'NotDue':");
        // jsBuilder.AppendLine("          statusColor = 'text-info';");
        // jsBuilder.AppendLine("          statusIcon = 'bi-info-circle-fill';");
        // jsBuilder.AppendLine("          break;");
        // jsBuilder.AppendLine("        default:");
        // jsBuilder.AppendLine("          statusColor = 'text-secondary';");
        // jsBuilder.AppendLine("          statusIcon = 'bi-question-circle-fill';");
        // jsBuilder.AppendLine("      }");
        // jsBuilder.AppendLine("      statusElement.className = statusColor;");
        // jsBuilder.AppendLine("      statusIconElement.className = `bi ${statusIcon}`;");
        // jsBuilder.AppendLine("      statusElement.innerHTML = `Status: <i class='${statusIcon}'></i> ${data.status}`;");
        // jsBuilder.AppendLine("    })");
        // jsBuilder.AppendLine("    .catch(error => console.error('Error fetching status:', error));");
        jsBuilder.AppendLine("}");

        return jsBuilder.ToString();
    }

    private string GetStatusColor(VerifiedCredential.CredentialStatus status)
    {
        return status switch
        {
            VerifiedCredential.CredentialStatus.Verified => "text-success",
            VerifiedCredential.CredentialStatus.Revoked => "text-danger",
            VerifiedCredential.CredentialStatus.Expired => "text-warning",
            VerifiedCredential.CredentialStatus.NotDue => "text-info",
            _ => "text-secondary",
        };
    }

    private string GetStatusIcon(VerifiedCredential.CredentialStatus status)
    {
        return status switch
        {
            VerifiedCredential.CredentialStatus.Verified => "bi-check-circle-fill",
            VerifiedCredential.CredentialStatus.Revoked => "bi-x-circle-fill",
            VerifiedCredential.CredentialStatus.Expired => "bi-exclamation-circle-fill",
            VerifiedCredential.CredentialStatus.NotDue => "bi-info-circle-fill",
            _ => "bi-question-circle-fill",
        };
    }
}