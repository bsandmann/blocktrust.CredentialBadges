using System;
using System.Text;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Services.TemplatesService;

public class TemplatesService
{
    public string GetPopulatedTemplate(string templateId, string theme, VerifiedCredential credential)
    {
        StringBuilder templateBuilder = new StringBuilder();
        string hostDomain = "https://credentialbadges.azurewebsites.net";
        string linkUrl = $"{hostDomain}/verifier/{credential.Id}";

        string themeClass = theme == "dark" ? "dark-theme" : "";
        string logoClass = theme == "dark" ? "dark-logo" : "";
        string titleClass = theme == "dark" ? "dark-title" : "";
        string subtitleClass = theme == "dark" ? "dark-subtitle" : "";
        string descriptionClass = theme == "dark" ? "dark-description" : "";
        string validityLabelClass = theme == "dark" ? "dark-validity-label" : "";
        string validityDateClass = theme == "dark" ? "dark-validity-date" : "";

        switch (templateId)
        {
            case "image_no_description_light":
            case "image_no_description_dark":
                templateBuilder.Append($@"
                        <a href='{linkUrl}' class='credential-container {themeClass}' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-logo {logoClass}'>
                                    <img src='{credential.Image}' alt='{credential.Name}' style='width: 100%; height: 100%; object-fit: contain;' />
                                </div>
                                <div class='credential-details'>
                                    <h2 class='credential-title {titleClass}'>{credential.Name}</h2>
                                    <p class='credential-subtitle {subtitleClass}'>{credential.Issuer}</p>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label {validityLabelClass}'>Valid from</span>
                                            <span class='credential-validity-date {validityDateClass}'>{((DateTime)credential.ValidFrom).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        {GetStatusButton(theme, credential.Status)}
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                break;

            case "image_description_light":
            case "image_description_dark":
                templateBuilder.Append($@"
                        <a href='{linkUrl}' class='credential-container {themeClass}' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-logo {logoClass}'>
                                    <img src='{credential.Image}' alt='{credential.Name}' style='width: 100%; height: 100%; object-fit: contain;' />
                                </div>
                                <div class='credential-details'>
                                    <h2 class='credential-title {titleClass}'>{credential.Name}</h2>
                                    <p class='credential-subtitle {subtitleClass}'>{credential.Issuer}</p>
                                    <p class='credential-description {descriptionClass}'>{credential.Description}</p>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label {validityLabelClass}'>Valid from</span>
                                            <span class='credential-validity-date {validityDateClass}'>{((DateTime)credential.ValidFrom).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        {GetStatusButton(theme, credential.Status)}
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                break;

            case "noimage_description_light":
            case "noimage_description_dark":
                templateBuilder.Append($@"
                        <a href='{linkUrl}' class='credential-container {themeClass}' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-details'>
                                    <h2 class='credential-title {titleClass}'>{credential.Name}</h2>
                                    <p class='credential-subtitle {subtitleClass}'>{credential.Issuer}</p>
                                    <p class='credential-description {descriptionClass}'>{credential.Description}</p>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label {validityLabelClass}'>Valid from</span>
                                            <span class='credential-validity-date {validityDateClass}'>{((DateTime)credential.ValidFrom).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        {GetStatusButton(theme, credential.Status)}
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                break;

            case "noimage_no_description_light":
            case "noimage_no_description_dark":
                templateBuilder.Append($@"
                        <a href='{linkUrl}' class='credential-container {themeClass}' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-details'>
                                    <h2 class='credential-title {titleClass}'>{credential.Name}</h2>
                                    <p class='credential-subtitle {subtitleClass}'>{credential.Issuer}</p>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label {validityLabelClass}'>Valid from</span>
                                            <span class='credential-validity-date {validityDateClass}'>{((DateTime)credential.ValidFrom).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        {GetStatusButton(theme, credential.Status)}
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                break;

            default:
                throw new ArgumentException($"Invalid template ID: {templateId}", nameof(templateId));
        }

        return templateBuilder.ToString();
    }

    private string GetStatusButton(string theme, EVerificationStatus status)
    {
        string buttonClass = GetStatusButtonClass(theme, status);
        string iconName = GetStatusIcon(status);
        string statusText = GetStatusText(status);
        string textClass = theme == "dark" ? "credential-verified-text-dark" : "credential-verified-text";

        return $@"
                <button class='{buttonClass}'>
                    <span class='material-icons credential-verified-icon'>{iconName}</span>
                    <span class='{textClass}'>{statusText}</span>
                </button>
            ";
    }

    private string GetStatusButtonClass(string theme, EVerificationStatus status)
    {
        string baseClass = "credential-button-";
        string statusClass = status switch
        {
            EVerificationStatus.Verified => "verified",
            EVerificationStatus.Revoked => "revoked",
            EVerificationStatus.Expired => "expired",
            _ => "unknown"
        };
        return $"{baseClass}{statusClass}";
    }

    private string GetStatusIcon(EVerificationStatus status)
    {
        return status switch
        {
            EVerificationStatus.Verified => "verified",
            EVerificationStatus.Revoked => "block",
            EVerificationStatus.Expired => "access_time",
            _ => "help"
        };
    }

    private string GetStatusText(EVerificationStatus status)
    {
        return status switch
        {
            EVerificationStatus.Verified => "Verified",
            EVerificationStatus.Revoked => "Revoked",
            EVerificationStatus.Expired => "Expired",
            _ => "Unknown"
        };
    }
}