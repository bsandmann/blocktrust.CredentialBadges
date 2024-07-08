using System;
using System.Text;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Services.TemplatesService
{
    /// <summary>
    /// Service for generating HTML templates for credentials based on template ID and theme.
    /// </summary>
    public class TemplatesService
    {
        /// <summary>
        /// Generates a populated HTML template based on the provided template ID, theme, and credential data.
        /// </summary>
        /// <param name="templateId">Template ID specifying the structure of the credential badge.</param>
        /// <param name="theme">Theme ('light' or 'dark') for the template.</param>
        /// <param name="credential">Credential data to populate the template.</param>
        /// <param name="hostDomain">Domain where the service is running.</param>
        /// <returns>Populated HTML template as a string.</returns>
        public string GetPopulatedTemplate(string templateId, string theme, VerifiedCredential credential)
        {
            StringBuilder templateBuilder = new StringBuilder();
            string hostDomain = "https://credentialbadges.azurewebsites.net";

            // Construct link URL with host domain and credential ID
            string linkUrl = $"{hostDomain}/verifier/{credential.Id}";

            // Select the template based on template ID and theme
            switch (templateId)
            {
                // Image, No Description (Light Theme)
                case "image_no_description_light":
                    templateBuilder.Append($@"
                        <!-- Image, No Description (Light Theme) -->
                        <a href='{linkUrl}' class='credential-container light-theme' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-logo light-logo'>
                                    <img src='{credential.Image}' alt='{credential.Name}' />
                                </div>
                                <div class='credential-details'>
                                    <h2 class='credential-name light-name'>{credential.Name}</h2>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label light-validity-label'>Issue date</span>
                                            <span class='credential-validity-date light-validity-date'>{((DateTime)credential.ValidUntil).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        <button class='{GetStatusButtonClass(theme, credential.Status)}'>
                                            <span class='{GetStatusIconClass(theme, credential.Status)}'></span>
                                            <span class='{GetStatusTextClass(theme)}'>{GetStatusText(credential.Status)}</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                    break;

                // Image, No Description (Dark Theme)
                case "image_no_description_dark":
                    templateBuilder.Append($@"
                        <!-- Image, No Description (Dark Theme) -->
                        <a href='{linkUrl}' class='credential-container dark-theme' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-logo dark-logo'>
                                    <img src='{credential.Image}' alt='{credential.Name}' />
                                </div>
                                <div class='credential-details'>
                                    <h2 class='credential-name dark-name'>{credential.Name}</h2>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label dark-validity-label'>Issue date</span>
                                            <span class='credential-validity-date dark-validity-date'>{((DateTime)credential.ValidUntil).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        <button class='{GetStatusButtonClass(theme, credential.Status)}'>
                                            <span class='{GetStatusIconClass(theme, credential.Status)}'></span>
                                            <span class='{GetStatusTextClass(theme)}'>{GetStatusText(credential.Status)}</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                    break;

                // Image, Description (Light Theme)
                case "image_description_light":
                    templateBuilder.Append($@"
                        <!-- Image, Description (Light Theme) -->
                        <a href='{linkUrl}' class='credential-container light-theme' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-logo light-logo'>
                                    <img src='{credential.Image}' alt='{credential.Name}' />
                                </div>
                                <div class='credential-details'>
                                    <h2 class='credential-name light-name'>{credential.Name}</h2>
                                    <p class='credential-description light-description'>{credential.Description}</p>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label light-validity-label'>Issue date</span>
                                            <span class='credential-validity-date light-validity-date'>{((DateTime)credential.ValidUntil).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        <button class='{GetStatusButtonClass(theme, credential.Status)}'>
                                            <span class='{GetStatusIconClass(theme, credential.Status)}'></span>
                                            <span class='{GetStatusTextClass(theme)}'>{GetStatusText(credential.Status)}</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                    break;

                // Image, Description (Dark Theme)
                case "image_description_dark":
                    templateBuilder.Append($@"
                        <!-- Image, Description (Dark Theme) -->
                        <a href='{linkUrl}' class='credential-container dark-theme' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-logo dark-logo'>
                                    <img src='{credential.Image}' alt='{credential.Name}' />
                                </div>
                                <div class='credential-details'>
                                    <h2 class='credential-name dark-name'>{credential.Name}</h2>
                                    <p class='credential-description dark-description'>{credential.Description}</p>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label dark-validity-label'>Issue date</span>
                                            <span class='credential-validity-date dark-validity-date'>{((DateTime)credential.ValidUntil).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        <button class='{GetStatusButtonClass(theme, credential.Status)}'>
                                            <span class='{GetStatusIconClass(theme, credential.Status)}'></span>
                                            <span class='{GetStatusTextClass(theme)}'>{GetStatusText(credential.Status)}</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                    break;

                // No Image, Description (Light Theme)
                case "noimage_description_light":
                    templateBuilder.Append($@"
                        <!-- No Image, Description (Light Theme) -->
                        <a href='{linkUrl}' class='credential-container light-theme' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-details'>
                                    <h2 class='credential-name light-name'>{credential.Name}</h2>
                                    <p class='credential-description light-description'>{credential.Description}</p>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label light-validity-label'>Issue date</span>
                                            <span class='credential-validity-date light-validity-date'>{((DateTime)credential.ValidUntil).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        <button class='{GetStatusButtonClass(theme, credential.Status)}'>
                                            <span class='{GetStatusIconClass(theme, credential.Status)}'></span>
                                            <span class='{GetStatusTextClass(theme)}'>{GetStatusText(credential.Status)}</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                    break;

                // No Image, Description (Dark Theme)
                case "noimage_description_dark":
                    templateBuilder.Append($@"
                        <!-- No Image, Description (Dark Theme) -->
                        <a href='{linkUrl}' class='credential-container dark-theme' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-details'>
                                    <h2 class='credential-name dark-name'>{credential.Name}</h2>
                                    <p class='credential-description dark-description'>{credential.Description}</p>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label dark-validity-label'>Issue date</span>
                                            <span class='credential-validity-date dark-validity-date'>{((DateTime)credential.ValidUntil).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        <button class='{GetStatusButtonClass(theme, credential.Status)}'>
                                            <span class='{GetStatusIconClass(theme, credential.Status)}'></span>
                                            <span class='{GetStatusTextClass(theme)}'>{GetStatusText(credential.Status)}</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                    break;

                // No Image, No Description (Light Theme)
                case "noimage_no_description_light":
                    templateBuilder.Append($@"
                        <!-- No Image, No Description (Light Theme) -->
                        <a href='{linkUrl}' class='credential-container light-theme' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-details'>
                                    <h2 class='credential-name light-name'>{credential.Name}</h2>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label light-validity-label'>Issue date</span>
                                            <span class='credential-validity-date light-validity-date'>{((DateTime)credential.ValidUntil).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        <button class='{GetStatusButtonClass(theme, credential.Status)}'>
                                            <span class='{GetStatusIconClass(theme, credential.Status)}'></span>
                                            <span class='{GetStatusTextClass(theme)}'>{GetStatusText(credential.Status)}</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                    break;

                // No Image, No Description (Dark Theme)
                case "noimage_no_description_dark":
                    templateBuilder.Append($@"
                        <!-- No Image, No Description (Dark Theme) -->
                        <a href='{linkUrl}' class='credential-container dark-theme' id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div class='credential-header'>
                                <div class='credential-details'>
                                    <h2 class='credential-name dark-name'>{credential.Name}</h2>
                                    <div class='credential-validity'>
                                        <div>
                                            <span class='credential-validity-label dark-validity-label'>Issue date</span>
                                            <span class='credential-validity-date dark-validity-date'>{((DateTime)credential.ValidUntil).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        <button class='{GetStatusButtonClass(theme, credential.Status)}'>
                                            <span class='{GetStatusIconClass(theme, credential.Status)}'></span>
                                            <span class='{GetStatusTextClass(theme)}'>{GetStatusText(credential.Status)}</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </a>
                    ");
                    break;

                // Invalid template ID
                default:
                    throw new ArgumentException($"Invalid template ID: {templateId}", nameof(templateId));
            }

            return templateBuilder.ToString();
        }

        /// <summary>
        /// Retrieves the button class for the status based on the theme and verification status.
        /// </summary>
        /// <param name="theme">Theme ('light' or 'dark') for the template.</param>
        /// <param name="status">Verification status of the credential.</param>
        /// <returns>Button class representing the status.</returns>
        private string GetStatusButtonClass(string theme, EVerificationStatus status)
        {
            // Determine class based on theme and status
            string themeClass = theme == "dark" ? "dark-theme" : "light-theme";
            string statusClass = status switch
            {
                EVerificationStatus.Verified => $"{themeClass}-verified-button",
                EVerificationStatus.Revoked => $"{themeClass}-revoked-button",
                EVerificationStatus.Expired => $"{themeClass}-expired-button",
                _ => $"{themeClass}-unknown-button"
            };
            return $"credential-button {statusClass}";
        }

        /// <summary>
        /// Retrieves the icon class for the status based on the theme and verification status.
        /// </summary>
        /// <param name="theme">Theme ('light' or 'dark') for the template.</param>
        /// <param name="status">Verification status of the credential.</param>
        /// <returns>Icon class representing the status.</returns>
        private string GetStatusIconClass(string theme, EVerificationStatus status)
        {
            // Determine icon class based on theme and status
            string themeClass = theme == "dark" ? "dark-theme" : "light-theme";
            string iconClass = status switch
            {
                EVerificationStatus.Verified => $"{themeClass}-verified-icon",
                EVerificationStatus.Revoked => $"{themeClass}-revoked-icon",
                EVerificationStatus.Expired => $"{themeClass}-expired-icon",
                _ => $"{themeClass}-unknown-icon"
            };
            return $"credential-{status.ToString().ToLower()}-icon {iconClass}";
        }

        /// <summary>
        /// Retrieves the text class for the status based on the theme.
        /// </summary>
        /// <param name="theme">Theme ('light' or 'dark') for the template.</param>
        /// <returns>Text class for the status.</returns>
        private string GetStatusTextClass(string theme)
        {
            return theme == "dark" ? "credential-verified-text-dark" : "credential-verified-text-light";
        }

        /// <summary>
        /// Retrieves the text for the status based on the verification status.
        /// </summary>
        /// <param name="status">Verification status of the credential.</param>
        /// <returns>Text describing the status of the credential.</returns>
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
}
