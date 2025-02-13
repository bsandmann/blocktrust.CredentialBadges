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
        string hostDomain = GetHostDomain();
        string linkUrl = $"{hostDomain}/verifier/{credential.Id}";

        bool isDarkTheme = theme == "dark";
        string backgroundColor = isDarkTheme ? "#020617" : "#FDFDFD";
        string textColor = isDarkTheme ? "#F1F5F9" : "inherit";
        string titleColor = isDarkTheme ? "#ffffff" : "inherit";
        string subtitleColor = isDarkTheme ? "#cbd5e0" : "#718096";
        string descriptionColor = isDarkTheme ? "#e2e8f0" : "inherit";
        string validityLabelColor = isDarkTheme ? "#94A3B8" : "#718096";
        string validityDateColor = isDarkTheme ? "#ffffff" : "#2d3748";
        string borderColor = "#dedede";
        string logoBackgroundColor = isDarkTheme ? "#4a5568" : "#ffffff";

        bool showDescription = templateId.Contains("description") && !templateId.Contains("no_description");
        bool showTypes = templateId.Contains("_withTypes");

        string truncatedName = TruncateString(credential.Name, 60);
        string truncatedIssuer = TruncateString(credential.Issuer, 50);
        string truncatedDescription = TruncateString(credential.Description, 170);

        string commonStyles = $@"
            display: inline-block !important;
            width: 30rem !important;
            border: 1px solid {borderColor} !important;
            border-radius: 0.5rem !important;
            background: {backgroundColor} !important;
            margin: 0rem !important;
            padding: 0.7rem !important;
            box-sizing: border-box !important;
            transition: box-shadow 0.2s ease-in-out, filter 0.2s ease-in-out !important;
            font-family: Calibri, Candara, Segoe, Segoe UI, Optima, Arial, sans-serif !important;
            color: {textColor} !important;
        ";

        string hoverEffect = @"
            this.style.setProperty('box-shadow','0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)','important');
            this.style.setProperty('filter','brightness(1.05)','important');
        ";

        string resetEffect = @"
            this.style.setProperty('box-shadow','0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)','important');
            this.style.setProperty('filter','brightness(1)','important');
        ";

        var validTemplates = new HashSet<string>
        {
            "image_no_description_light",
            "image_no_description_dark",
            "image_description_light",
            "image_description_dark",
            "noimage_description_light",
            "noimage_description_dark",
            "noimage_no_description_light",
            "noimage_no_description_dark",
            "image_no_description_withTypes_light",
            "image_no_description_withTypes_dark",
            "image_description_withTypes_light",
            "image_description_withTypes_dark",
            "noimage_description_withTypes_light",
            "noimage_description_withTypes_dark",
            "noimage_no_description_withTypes_light",
            "noimage_no_description_withTypes_dark"
        };

        if (!validTemplates.Contains(templateId))
        {
            throw new ArgumentException($"Invalid template ID: {templateId}", nameof(templateId));
        }

        StringBuilder templateBuilder = new StringBuilder();
        templateBuilder.Append($@"
            <div style='{commonStyles}' onmouseover=""{hoverEffect}"" onmouseout=""{resetEffect}"">
                <a href='{linkUrl}'
                   style='
                        text-decoration: none !important;
                        color: inherit !important;
                        display: block !important;
                        border-bottom: none !important;
                   '
                   id='{credential.Id}'
                   data-credential-id='{credential.Id}'
                   data-template-id='{templateId}'>

                    <div style='
                        display: flex !important;
                        align-items: flex-start !important;
                        gap: 1rem !important;
                    '>
                        {(templateId.StartsWith("image") ? $@"
                        <div style='
                            width: 7rem !important;
                            height: 7rem !important;
                            flex-shrink: 0 !important;
                            background-color: {logoBackgroundColor} !important;
                            border-radius: 0.5rem !important;
                            overflow: hidden !important;
                        '>
                            <img src='{GetImage(credential.Image)}'
                                 alt='{truncatedName}'
                                 style='
                                    width: 100% !important;
                                    height: 100% !important;
                                    object-fit: contain !important;
                                 ' />
                        </div>" : "")}

                        <div style='
                            flex-grow: 1 !important;
                            min-width: 0 !important;
                        '>
                            <div style='
                                font-size: 1.5rem !important;
                                margin-bottom: 0.5rem !important;
                                line-height: 1.5 !important;
                                color: {titleColor} !important;
                                overflow: hidden !important;
                                text-overflow: ellipsis !important;
                                white-space: nowrap !important;
                                font-weight: bold !important;
                            '
                            title='{credential.Name}'>{truncatedName}</div>

                            {(showTypes && credential.Types != null && credential.Types.Count > 0 ? $@"
                            <div style='
                                display: flex !important;
                                align-items: baseline !important;
                                gap: 0.25rem !important;
                                font-size: 0.875rem !important;
                                margin: 0 !important;
                                padding: 0 !important;
                            '>
                                <div style='
                                    color: {validityLabelColor} !important;
                                    font-weight: 400 !important;
                                '>
                                    {(credential.Types.Count == 1 ? "Type" : "Types")}
                                </div>
                                <div style='
                                    color: {validityDateColor} !important;
                                    font-weight: 500 !important;
                                '>
                                    {string.Join(", ", credential.Types)}
                                </div>
                            </div>
                            " : "")}

                            <div style='
                                color: {subtitleColor} !important;
                                font-size: 0.875rem !important;
                                font-weight: 500 !important;
                                margin-bottom: 0.5rem !important;
                                overflow: hidden !important;
                                text-overflow: ellipsis !important;
                                white-space: nowrap !important;
                            '
                            title='{credential.Issuer}'>Issued by: {truncatedIssuer}</div>

                            {(showDescription ? $@"
                            <div style='
                                font-size: 0.875rem !important;
                                margin-bottom: 0.5rem !important;
                                color: {descriptionColor} !important;
                                overflow: hidden !important;
                                text-overflow: ellipsis !important;
                                display: -webkit-box !important;
                                -webkit-line-clamp: 2 !important;
                                -webkit-box-orient: vertical !important;
                            '
                            title='{credential.Description}'>{truncatedDescription}</div>
                            " : "")}

                            {(credential.Claims != null && credential.Claims.Count > 0 ? $@"
                            <div style='
                                display: flex !important;
                                flex-direction: column !important;
                                gap: 0.25rem !important;
                                margin-bottom: 0.5rem !important;
                            '>
                                {string.Join("", credential.Claims.Select(claim => $@"
                                    <div style='
                                        display: flex !important;
                                        align-items: baseline !important;
                                        gap: 0.25rem !important;
                                        font-size: 0.875rem !important;
                                    '>
                                        <div style='
                                            color: {validityLabelColor} !important;
                                            font-weight: 400 !important;
                                        '>
                                            {claim.Key}:
                                        </div>
                                        <div style='
                                            color: {validityDateColor} !important;
                                            font-weight: 500 !important;
                                            word-wrap: break-word !important;
                                        '>
                                            {claim.Value}
                                        </div>
                                    </div>
                                "))}
                            </div>
                            " : "")}

                            <div style='
                                display: flex !important;
                                align-items: center !important;
                                justify-content: space-between !important;
                                margin-top: 0.25rem !important;
                            '>
                                <div style='
                                    display: flex !important;
                                    flex-direction: column !important;
                                    line-height: 1 !important;
                                '>
                                    <div style='
                                        display: flex !important;
                                        align-items: baseline !important;
                                        gap: 0.25rem !important;
                                        font-size: 0.875rem !important;
                                        margin-bottom: 0.25rem !important;
                                    '>
                                        <div style='
                                            color: {validityLabelColor} !important;
                                            font-weight: 400 !important;
                                        '>
                                            Valid from
                                        </div>
                                        <div style='
                                            color: {validityDateColor} !important;
                                            font-weight: 500 !important;
                                        '>
                                            {credential.ValidFrom:dd MMMM, yyyy}
                                        </div>
                                    </div>
                                    {(credential.ValidUntil != default(DateTime) ? $@"
                                    <div style='
                                        display: flex !important;
                                        align-items: baseline !important;
                                        gap: 0.25rem !important;
                                        font-size: 0.875rem !important;
                                    '>
                                        <div style='
                                            color: {validityLabelColor} !important;
                                            font-weight: 400 !important;
                                        '>
                                            Valid until
                                        </div>
                                        <div style='
                                            color: {validityDateColor} !important;
                                            font-weight: 500 !important;
                                        '>
                                            {credential.ValidUntil:dd MMMM, yyyy}
                                        </div>
                                    </div>
                                    " : "")}
                                </div>
                                {GetStatusButton(isDarkTheme, credential.Status)}
                            </div>
                        </div>
                    </div>
                </a>
            </div>
        ");

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
            <button style='
                background-color: {backgroundColor} !important;
                color: {textColor} !important;
                padding: 0.375rem 0.75rem !important;
                border-radius: 0.25rem !important;
                display: flex !important;
                align-items: center !important;
                font-size: 0.875rem !important;
                font-weight: 500 !important;
                border: none !important;
                outline: none !important;
                cursor: pointer !important;
            '>
                {GetStatusIcon(status, iconColor)}
                <div style='margin-left: 0.25rem !important; display: inline-block !important;'>{statusText}</div>
            </button>
        ";
    }

    private (string backgroundColor, string textColor, string iconColor) GetStatusStyles(bool isDarkTheme, EVerificationStatus status)
    {
        return status switch
        {
            EVerificationStatus.Verified => isDarkTheme
                ? ("#718096", "#0F172A", "#0F172A")
                : ("#334155", "#ffffff", "#ffffff"),
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

        return $@"<svg
                    xmlns='http://www.w3.org/2000/svg'
                    width='20'
                    height='20'
                    viewBox='0 0 24 24'
                    fill='none'
                    stroke='{color}'
                    stroke-width='2'
                    stroke-linecap='round'
                    stroke-linejoin='round'>
                      <path d='{iconPath}'></path>
                  </svg>";
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

        if (!image.StartsWith("http") && !image.StartsWith("data:image"))
        {
            return $"data:image;base64,{image}";
        }

        return image;
    }
}
