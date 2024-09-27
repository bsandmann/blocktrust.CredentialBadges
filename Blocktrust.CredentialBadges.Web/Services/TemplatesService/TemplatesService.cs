using System.Text;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Services.TemplatesService;

public class TemplatesService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TemplatesService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string GetPopulatedTemplate(string templateId, string theme, VerifiedCredential credential)
    {
        StringBuilder templateBuilder = new StringBuilder();
        string hostDomain = GetHostDomain();
        string linkUrl = $"{hostDomain}/verifier/{credential.Id}";

        bool isDarkTheme = theme.ToLower() == "dark";
        string backgroundColor = isDarkTheme ? "#020617" : "#F8FAFC";
        string textColor = isDarkTheme ? "#F1F5F9" : "#1E293B";
        string titleColor = isDarkTheme ? "#ffffff" : "#1E293B";
        string subtitleColor = isDarkTheme ? "#cbd5e0" : "#718096";
        string descriptionColor = isDarkTheme ? "#e2e8f0" : "#4B5563";
        string validityLabelColor = isDarkTheme ? "#94A3B8" : "#718096";
        string validityDateColor = isDarkTheme ? "#ffffff" : "#2d3748";
        string borderColor = isDarkTheme ? "#1e293b" : "#e2e8f0";
        string logoBackgroundColor = isDarkTheme ? "#4a5568" : "#ffffff";

        string truncatedName = TruncateString(credential.Name, 60);
        string truncatedIssuer = TruncateString(credential.Issuer, 20);
        string truncatedDescription = TruncateString(credential.Description, 150);

        string containerStyles = $@"
                display: block;
                width: 30rem;
                font-family: 'Poppins', sans-serif;
                border: 1px solid {borderColor};
                border-radius: 0.5rem;
                padding: 1rem;
                margin: 1rem auto;
                background-color: {backgroundColor};
                box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);
                text-decoration: none;
                color: {textColor};
                transition: box-shadow 0.2s ease-in-out, filter 0.2s ease-in-out, background-color 0.2s ease-in-out;
            ";

        string hoverEffect = $"this.style.boxShadow='0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)'; this.style.filter='brightness(1.05)';";
        string resetEffect = $"this.style.boxShadow='0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)'; this.style.filter='brightness(1)';";

        switch (templateId)
        {
            case "image_no_description_light":
            case "image_no_description_dark":
            case "image_description_light":
            case "image_description_dark":
            case "noimage_description_light":
            case "noimage_description_dark":
            case "noimage_no_description_light":
            case "noimage_no_description_dark":
                templateBuilder.Append($@"
                        <a href='{linkUrl}' style='{containerStyles}' onmouseover=""{hoverEffect}"" onmouseout=""{resetEffect}"" id='{credential.Id}' data-credential-id='{credential.Id}' data-template-id='{templateId}'>
                            <div style='display: flex; align-items: flex-start; gap: 1rem;'>
                                {(templateId.StartsWith("image") ? $@"
                                <div style='width: 7rem; height: 7rem; flex-shrink: 0; background-color: {logoBackgroundColor}; border-radius: 0.5rem; overflow: hidden;'>
                                    <img src='{GetImage(credential.Image)}' alt='{truncatedName}' style='width: 100%; height: 100%; object-fit: contain;' />
                                </div>" : "")}
                                <div style='flex-grow: 1; min-width: 0;'>
                                    <h2 style='font-size: 1.25rem; margin: 0 0 0.5rem 0; line-height: 1.5; color: {titleColor}; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;'>{truncatedName}</h2>
                                    <p style='color: {subtitleColor}; font-size: 0.875rem; font-weight: 500; margin: 0 0 0.5rem 0; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;'>{truncatedIssuer}</p>
                                    {(templateId.Contains("description") ? $@"
                                    <p style='font-size: 0.875rem; margin: 0 0 1rem 0; color: {descriptionColor}; overflow: hidden; text-overflow: ellipsis; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical;'>{truncatedDescription}</p>" : "")}
                                    <div style='display: flex; align-items: center; justify-content: space-between; margin-top: 0.25rem;'>
                                        <div>
                                            <span style='color: {validityLabelColor}; font-size: 0.75rem; font-weight: 400;'>Valid from</span>
                                            <span style='color: {validityDateColor}; font-size: 0.875rem; font-weight: 500; margin-left: 0.25rem;'>{((DateTime)credential.ValidFrom).ToString("dd MMMM, yyyy")}</span>
                                        </div>
                                        {GetStatusButton(isDarkTheme, credential.Status)}
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

    private string TruncateString(string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
            return input;
            
        return input.Substring(0, maxLength - 3) + "...";
    }

    private string GetStatusButton(bool isDarkTheme, EVerificationStatus status)
    {
        var (backgroundColor, textColor, iconColor) = GetStatusStyles(isDarkTheme, status);
        string statusText = GetStatusText(status);

        return $@"
                <button style='background-color: {backgroundColor}; color: {textColor}; padding: 0.375rem 0.75rem; border-radius: 0.25rem; display: flex; align-items: center; font-size: 0.875rem; font-weight: 500; border: none; outline: none; cursor: pointer;'>
                    {GetStatusIcon(status, iconColor)}
                    <span style='margin-left: 0.25rem;'>{statusText}</span>
                </button>
            ";
    }

    private (string backgroundColor, string textColor, string iconColor) GetStatusStyles(bool isDarkTheme, EVerificationStatus status)
    {
        return status switch
        {
            EVerificationStatus.Verified => isDarkTheme ? ("#718096", "#0F172A", "#0F172A") : ("#334155", "#ffffff", "#ffffff"),
            EVerificationStatus.Revoked => ("#FEE2E2", "#DC2626", "#DC2626"),
            EVerificationStatus.Expired => ("#FEF9C3", "#713F12", "#713F12"),
            _ => ("#E2E8F0", "#4A5568", "#4A5568")
        };
    }

    private string GetStatusIcon(EVerificationStatus status, string color)
    {
        string iconPath = status switch
        {
            EVerificationStatus.Verified => "M22 11.08V12a10 10 0 1 1-5.93-9.14 M22 4L12 14.01l-3-3",
            EVerificationStatus.Revoked => "M12 22C6.47715 22 2 17.5228 2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12C22 17.5228 17.5228 22 12 22ZM9 9L15 15 M15 9L9 15",
            EVerificationStatus.Expired => "M12 22C6.47715 22 2 17.5228 2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12C22 17.5228 17.5228 22 12 22ZM12 6V12L16 14",
            _ => "M12 22C6.47715 22 2 17.5228 2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12C22 17.5228 17.5228 22 12 22ZM12 16V18 M12 6V14"
        };

        return $@"<svg xmlns='http://www.w3.org/2000/svg' width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='{color}' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><path d='{iconPath}'></path></svg>";
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
        
    private string GetHostDomain()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            // Fallback to a default domain if HttpContext is not available
            return "https://credentialbadges.azurewebsites.net";
        }

        var scheme = request.Scheme;
        var host = request.Host.Value;

        return $"{scheme}://{host}";
    }
        
    private string GetImage(string? image)
    {
        if (image == null)
        {
            return "https://via.placeholder.com/150";
        }
        if (image.StartsWith("/"))
        {
            return $"data:image;base64,{image}";
        }
        return image;
    }
}