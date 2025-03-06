using System.Text;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;
using System.Text.RegularExpressions;

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

        // Basic show/hide logic
        bool showDescription = templateId.Contains("description") && !templateId.Contains("no_description");
        bool showTypes = templateId.Contains("_withTypes") && !templateId.Contains("_small_");
        bool noClaims = templateId.Contains("_noclaims");
        bool isSmall = templateId.Contains("_small_");

        // Subject name
        var subjectName = credential.SubjectName;

        // Truncated strings
        credential.Claims.TryGetValue("name", out var name);
        credential.Claims.TryGetValue("description", out var description);
        string truncatedName = TruncateString(name ?? "", 60);
        string truncatedIssuer = TruncateString(credential.Issuer, 50);
        string truncatedDescription = TruncateString(description ?? "", 170);

        // Valid template IDs (including endorsement ones)
        var validTemplates = new HashSet<string>
        {
            "image_no_description_light", "image_no_description_dark",
            "image_description_light", "image_description_dark",
            "noimage_description_light", "noimage_description_dark",
            "noimage_no_description_light", "noimage_no_description_dark",
            "image_no_description_withTypes_light", "image_no_description_withTypes_dark",
            "image_description_withTypes_light", "image_description_withTypes_dark",
            "noimage_description_withTypes_light", "noimage_description_withTypes_dark",
            "noimage_no_description_withTypes_light", "noimage_no_description_withTypes_dark",
            "image_no_description_small_light", "image_no_description_small_dark",
            "image_description_small_light", "image_description_small_dark",
            "noimage_description_small_light", "noimage_description_small_dark",
            "noimage_no_description_small_light", "noimage_no_description_small_dark",
            "image_no_description_noclaims_light", "image_no_description_noclaims_dark",
            "image_description_noclaims_light", "image_description_noclaims_dark",
            "noimage_description_noclaims_light", "noimage_description_noclaims_dark",
            "noimage_no_description_noclaims_light", "noimage_no_description_noclaims_dark",
            "image_no_description_withTypes_noclaims_light", "image_no_description_withTypes_noclaims_dark",
            "image_description_withTypes_noclaims_light", "image_description_withTypes_noclaims_dark",
            "noimage_description_withTypes_noclaims_light", "noimage_description_withTypes_noclaims_dark",
            "noimage_no_description_withTypes_noclaims_light", "noimage_no_description_withTypes_noclaims_dark",
            "image_no_description_small_noclaims_light", "image_no_description_small_noclaims_dark",
            "image_description_small_noclaims_light", "image_description_small_noclaims_dark",
            "noimage_description_small_noclaims_light", "noimage_description_small_noclaims_dark",
            "noimage_no_description_small_noclaims_light", "noimage_no_description_small_noclaims_dark",

            // Endorsement templates
            "endorsement_light",
            "endorsement_dark",
            "endorsement_detailed_light",
            "endorsement_detailed_dark"
        };

        if (!validTemplates.Contains(templateId))
        {
            throw new ArgumentException($"Invalid template ID: {templateId}", nameof(templateId));
        }

        // Common hover effects
        string hoverEffect = @"
            this.style.setProperty('box-shadow','0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)','important');
            this.style.setProperty('filter','brightness(1.05)','important');
        ";

        string resetEffect = @"
            this.style.setProperty('box-shadow','0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)','important');
            this.style.setProperty('filter','brightness(1)','important');
        ";

        // Endorsement handling
        if (templateId.StartsWith("endorsement"))
        {
            bool isDetailed = templateId.Contains("detailed");

            credential.Claims.TryGetValue("endorsementComment", out var endorsementCommentRaw);
            string endorsementComment = endorsementCommentRaw ?? "";
            string truncatedEndorsementComment = TruncateString(endorsementComment, 200);
            bool hasEndorsementComment = !string.IsNullOrWhiteSpace(endorsementComment);

            StringBuilder endorsementBuilder = new StringBuilder();

            // Outer container (same as other templates)
            string containerStyles = $@"
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

            endorsementBuilder.Append($@"
                <div style='{containerStyles}' onmouseover=""{hoverEffect}"" onmouseout=""{resetEffect}"">
                    <a href='{linkUrl}'
                       style='text-decoration: none !important;
                              color: inherit !important;
                              display: block !important;
                              border-bottom: none !important;'
                       id='{credential.Id}'
                       data-credential-id='{credential.Id}'
                       data-template-id='{templateId}'>

                        <div style='display: flex !important;
                                    align-items: flex-start !important;
                                    gap: 1rem !important;'>
            ");

            // If we DO have an endorsement comment, show the big SVG quote
            if (hasEndorsementComment)
            {
                endorsementBuilder.Append($@"
                <div style='
                    width: 3.5rem !important;
                    flex-shrink: 0 !important;
                    display: flex !important;
                    align-items: flex-start !important;
                    justify-content: center !important;
                '>
                    <svg
                        clip-rule='evenodd'
                        fill-rule='evenodd'
                        stroke-linejoin='round'
                        stroke-miterlimit='2'
                        viewBox='0 0 24 24'
                        xmlns='http://www.w3.org/2000/svg'
                        style='
                            width: 3.5rem !important;
                            height: 3.5rem !important;
                            fill: {(isDarkTheme ? "#ffffff" : "#000000")} !important;
                        '>
                        <path d='m21.301 4c.411 0 .699.313.699.663 0 .248-.145.515-.497.702-1.788.948-3.858 4.226-3.858 6.248 3.016-.092 4.326 2.582 4.326 4.258 0 2.007-1.738 4.129-4.308 4.129-3.24 0-4.83-2.547-4.83-5.307 0-5.98 6.834-10.693 8.468-10.693zm-10.833 0c.41 0 .699.313.699.663 0 .248-.145.515-.497.702-1.788.948-3.858 4.226-3.858 6.248 3.016-.092 4.326 2.582 4.326 4.258 0 2.007-1.739 4.129-4.308 4.129-3.241 0-4.83-2.547-4.83-5.307 0-5.98 6.833-10.693 8.468-10.693z'
                              fill-rule='nonzero'/>
                    </svg>
                </div>
    ");
            }

            // Right block (main text area)
            endorsementBuilder.Append($@"
                            <div style='flex-grow: 1 !important; min-width: 0 !important;'>
            ");

            // 1) Big text
            if (hasEndorsementComment)
            {
                // endorsementComment in bigger text
                endorsementBuilder.Append($@"
                                <div style='
                                    font-size: 1.75rem !important;
                                    margin-bottom: 0.5rem !important;
                                    line-height: 1.4 !important;
                                    color: {titleColor} !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                    font-weight: bold !important;
                                '
                                title='{truncatedEndorsementComment}'>{truncatedEndorsementComment}</div>
                ");
            }

            // 2) If detailed AND we have endorsementComment => show name in smaller style
            //    If endorsementComment is missing => name is big text
            if (isDetailed)
            {
                if (hasEndorsementComment)
                {
                    endorsementBuilder.Append($@"
                                <div style='
                                    font-size: 0.875rem !important;
                                    margin-bottom: 0.5rem !important;
                                    color: {descriptionColor} !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                    font-weight: 500 !important;
                                '
                                title='{truncatedName}'>{truncatedName}</div>
                    ");
                }
                else
                {
                    endorsementBuilder.Append($@"
                                <div style='
                                    font-size: 1.75rem !important;
                                    margin-bottom: 0.5rem !important;
                                    line-height: 1.4 !important;
                                    color: {titleColor} !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                    font-weight: bold !important;
                                '
                                title='{truncatedName}'>{truncatedName}</div>
                    ");
                }
            }
            else
            {
                // Non-detailed version
                // If endorsementComment is missing => show name in big text; otherwise skip name
                if (!hasEndorsementComment)
                {
                    endorsementBuilder.Append($@"
                                <div style='
                                    font-size: 1.75rem !important;
                                    margin-bottom: 0.5rem !important;
                                    line-height: 1.4 !important;
                                    color: {titleColor} !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                    font-weight: bold !important;
                                '
                                title='{truncatedName}'>{truncatedName}</div>
                    ");
                }
            }

            // 3) Issued by
            endorsementBuilder.Append($@"
                                <div style='
                                    color: {subtitleColor} !important;
                                    font-size: 0.875rem !important;
                                    font-weight: 500 !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                '
                                title='{credential.Issuer}'>Issued by: {truncatedIssuer}</div>
            ");

            // 4) Issued to
            if (!string.IsNullOrWhiteSpace(subjectName))
            {
                endorsementBuilder.Append($@"
                                <div style='
                                    color: {subtitleColor} !important;
                                    font-size: 0.875rem !important;
                                    font-weight: 500 !important;
                                    margin-bottom: 0.5rem !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                '
                                title='{subjectName}'>Issued to: {subjectName}</div>
                ");
            }

            // 5) If detailed, show truncatedDescription
            if (isDetailed && !string.IsNullOrWhiteSpace(truncatedDescription))
            {
                endorsementBuilder.Append($@"
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
                                title='{truncatedDescription}'>{truncatedDescription}</div>
                ");
            }

            // 6) Skip claims entirely for endorsement

            // 7) Validity + status
            endorsementBuilder.Append($@"
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
            ");

            if (credential.ValidUntil != default(DateTime) && credential.ValidUntil is not null)
            {
                endorsementBuilder.Append($@"
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
                ");
            }

            endorsementBuilder.Append($@"
                                    </div>
                                    {GetStatusButton(isDarkTheme, credential.Status)}
                                </div>
                            </div>
                        </div>
                    </a>
                </div>
            ");

            return endorsementBuilder.ToString();
        }

        // Non-endorsement templates
        StringBuilder templateBuilder = new StringBuilder();

        if (!isSmall)
        {
            // REGULAR TEMPLATE
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

            templateBuilder.Append($@"
                <div style='{commonStyles}' onmouseover=""{hoverEffect}"" onmouseout=""{resetEffect}"">
                    <a href='{linkUrl}'
                       style='text-decoration: none !important;
                              color: inherit !important;
                              display: block !important;
                              border-bottom: none !important;'
                       id='{credential.Id}'
                       data-credential-id='{credential.Id}'
                       data-template-id='{templateId}'>

                        <div style='display: flex !important;
                                    align-items: flex-start !important;
                                    gap: 1rem !important;'>
            ");

            if (templateId.StartsWith("image"))
            {
                templateBuilder.Append($@"
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
                            </div>
                ");
            }

            templateBuilder.Append($@"
                            <div style='flex-grow: 1 !important; min-width: 0 !important;'>
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
                                title='{truncatedName}'>{truncatedName}</div>
            ");

            // Show types
            if (showTypes && credential.Types != null && credential.Types.Count > 0)
            {
                templateBuilder.Append($@"
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
                ");
            }

            templateBuilder.Append($@"
                                <div style='
                                    color: {subtitleColor} !important;
                                    font-size: 0.875rem !important;
                                    font-weight: 500 !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                '
                                title='{credential.Issuer}'>Issued by: {truncatedIssuer}</div>
            ");

            if (!string.IsNullOrWhiteSpace(subjectName))
            {
                templateBuilder.Append($@"
                                <div style='
                                    color: {subtitleColor} !important;
                                    font-size: 0.875rem !important;
                                    font-weight: 500 !important;
                                    margin-bottom: 0.5rem !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                '
                                title='{subjectName}'>Issued to: {subjectName}</div>
                ");
            }

            if (showDescription)
            {
                templateBuilder.Append($@"
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
                                title='{truncatedDescription}'>{truncatedDescription}</div>
                ");
            }

            // Claims
            if (!noClaims && credential.Claims != null && credential.Claims.Count > 0)
            {
                var claimItems = credential.Claims
                    .Where(p => p.Key.ToLowerInvariant() != "name"
                                && p.Key.ToLowerInvariant() != "description"
                                && p.Key.ToLowerInvariant() != "identifier")
                    .Select(claim => $@"
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
                                                 {MakeReadableKey(claim.Key)}:
                                            </div>
                                            <div style='
                                                color: {validityDateColor} !important;
                                                font-weight: 500 !important;
                                                word-wrap: break-word !important;
                                            '>
                                                {claim.Value}
                                            </div>
                                        </div>
                    ");

                templateBuilder.Append($@"
                                <div style='
                                    display: flex !important;
                                    flex-direction: column !important;
                                    gap: 0.25rem !important;
                                    margin-bottom: 0.5rem !important;
                                '>
                                    {string.Join("", claimItems)}
                                </div>
                ");
            }

            templateBuilder.Append($@"
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
            ");

            if (credential.ValidUntil != default(DateTime) && credential.ValidUntil is not null)
            {
                templateBuilder.Append($@"
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
                ");
            }

            templateBuilder.Append($@"
                                    </div>
                                    {GetStatusButton(isDarkTheme, credential.Status)}
                                </div>
                            </div>
                        </div>
                    </a>
                </div>
            ");
        }
        else
        {
            // SMALL TEMPLATE
            string smallStyles = $@"
                display: inline-block !important;
                width: 20rem !important;
                border: 1px solid {borderColor} !important;
                border-radius: 0.5rem !important;
                background: {backgroundColor} !important;
                margin: 0.5rem !important;
                padding: 0.5rem !important;
                box-sizing: border-box !important;
                transition: box-shadow 0.2s ease-in-out, filter 0.2s ease-in-out !important;
                font-family: Calibri, Candara, Segoe, Segoe UI, Optima, Arial, sans-serif !important;
                color: {textColor} !important;
            ";

            templateBuilder.Append($@"
                <div style='{smallStyles}' onmouseover=""{hoverEffect}"" onmouseout=""{resetEffect}"">
                    <a href='{linkUrl}'
                       style='text-decoration: none !important;
                              color: inherit !important;
                              display: block !important;
                              border-bottom: none !important;'
                       id='{credential.Id}'
                       data-credential-id='{credential.Id}'
                       data-template-id='{templateId}'>

                        <div style='display: flex !important;
                                    align-items: flex-start !important;
                                    gap: 0.5rem !important;'>
            ");

            if (templateId.StartsWith("image"))
            {
                templateBuilder.Append($@"
                            <div style='
                                width: 3.5rem !important;
                                height: 3.5rem !important;
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
                            </div>
                ");
            }

            templateBuilder.Append($@"
                            <div style='flex-grow: 1 !important; min-width: 0 !important;'>
                                <div style='
                                    font-size: 1.125rem !important;
                                    margin-bottom: 0.3rem !important;
                                    line-height: 1.3 !important;
                                    color: {titleColor} !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                    font-weight: bold !important;
                                '
                                title='{credential.Name}'>{truncatedName}</div>

                                <div style='
                                    color: {subtitleColor} !important;
                                    font-size: 0.8rem !important;
                                    font-weight: 500 !important;
                                    margin-bottom: 0.3rem !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                '
                                title='{credential.Issuer}'>Issued by: {truncatedIssuer}</div>
            ");

            if (!string.IsNullOrWhiteSpace(subjectName))
            {
                templateBuilder.Append($@"
                                <div style='
                                    color: {subtitleColor} !important;
                                    font-size: 0.8rem !important;
                                    font-weight: 500 !important;
                                    margin-bottom: 0.3rem !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    white-space: nowrap !important;
                                '
                                title='{subjectName}'>Issued to: {subjectName}</div>
                ");
            }

            if (showDescription)
            {
                templateBuilder.Append($@"
                                <div style='
                                    font-size: 0.8rem !important;
                                    margin-bottom: 0.3rem !important;
                                    color: {descriptionColor} !important;
                                    overflow: hidden !important;
                                    text-overflow: ellipsis !important;
                                    display: -webkit-box !important;
                                    -webkit-line-clamp: 2 !important;
                                    -webkit-box-orient: vertical !important;
                                '
                                title='{credential.Description}'>{truncatedDescription}</div>
                ");
            }

            if (!noClaims && credential.Claims != null && credential.Claims.Any())
            {
                var claimItems = credential.Claims
                    .Where(p => p.Key.ToLowerInvariant() != "name"
                                && p.Key.ToLowerInvariant() != "description"
                                && p.Key.ToLowerInvariant() != "identifier")
                    .Select(claim => $@"
                                        <div style='
                                            display: flex !important;
                                            align-items: baseline !important;
                                            gap: 0.25rem !important;
                                            font-size: 0.8rem !important;
                                        '>
                                            <div style='
                                                color: {validityLabelColor} !important;
                                                font-weight: 400 !important;
                                            '>
                                                {MakeReadableKey(claim.Key)}:
                                            </div>
                                            <div style='
                                                color: {validityDateColor} !important;
                                                font-weight: 500 !important;
                                                word-wrap: break-word !important;
                                            '>
                                                {claim.Value}
                                            </div>
                                        </div>
                    ");

                templateBuilder.Append($@"
                                <div style='
                                    display: flex !important;
                                    flex-direction: column !important;
                                    gap: 0.25rem !important;
                                    margin-bottom: 0.3rem !important;
                                '>
                                    {string.Join("", claimItems)}
                                </div>
                ");
            }

            // For small templates, no ValidFrom/ValidUntil lines; just a status button
            templateBuilder.Append($@"
                                <div style='
                                    display: flex !important;
                                    justify-content: flex-end !important;
                                '>
                                    {GetStatusButtonSmall(isDarkTheme, credential.Status)}
                                </div>
                            </div>
                        </div>
                    </a>
                </div>
            ");
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

    private string GetStatusButtonSmall(bool isDarkTheme, EVerificationStatus status)
    {
        var (backgroundColor, textColor, iconColor) = GetStatusStyles(isDarkTheme, status);
        string statusText = GetStatusText(status);

        return $@"
            <button style='
                background-color: {backgroundColor} !important;
                color: {textColor} !important;
                padding: 0.25rem 0.5rem !important;
                border-radius: 0.25rem !important;
                display: flex !important;
                align-items: center !important;
                font-size: 0.75rem !important;
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

        // If it's not a URL (http/https) or a data URI, assume base64
        if (!image.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
            !image.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
        {
            return $"data:image;base64,{image}";
        }

        return image;
    }

    public static string MakeReadableKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return key ?? "";

        // If the key already has spaces, just uppercase the first letter
        if (key.Contains(' '))
        {
            return char.ToUpper(key[0]) + key.Substring(1);
        }
        else
        {
            // Insert a space before each uppercase letter
            string withSpaces = Regex.Replace(key, "([A-Z])", " $1").Trim();
            // Capitalize the first letter, rest lower
            return char.ToUpper(withSpaces[0]) + withSpaces.Substring(1).ToLower();
        }
    }
}
