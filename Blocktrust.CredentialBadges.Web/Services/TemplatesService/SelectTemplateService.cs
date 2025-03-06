using Blocktrust.CredentialBadges.Web.Domain;
using System.Collections.Generic;

namespace Blocktrust.CredentialBadges.Web.Services.TemplatesService;

using Core.Common;
using OpenBadges;

public class SelectTemplateService
{
    public List<string> GetApplicableTemplateIds(VerifiedCredential credential)
    {
        // Determine if the credential has an image or a description
        bool hasImage = !string.IsNullOrEmpty(credential.Image);
        bool hasDescription = !string.IsNullOrEmpty(credential.Description);
        if (!hasDescription)
        {
            hasDescription = credential.Claims.TryGetValue("description", out var description) && !string.IsNullOrEmpty(description);
        }

        // Determine if the credential has non-empty "filtered" types
        var filteredTypes = GetFilteredTypes(credential);
        bool hasTypes = filteredTypes.Any();

        var applicableTemplateIds = new List<string>();

        // 1) Templates that require both image and description
        if (hasImage && hasDescription)
        {
            // Regular
            applicableTemplateIds.Add("image_description_light");
            applicableTemplateIds.Add("image_description_dark");
            applicableTemplateIds.Add("image_description_small_light");
            applicableTemplateIds.Add("image_description_small_dark");
            // _noclaims
            applicableTemplateIds.Add("image_description_noclaims_light");
            applicableTemplateIds.Add("image_description_noclaims_dark");
            applicableTemplateIds.Add("image_description_small_noclaims_light");
            applicableTemplateIds.Add("image_description_small_noclaims_dark");

            // Also add the _withTypes variation if we have types
            if (hasTypes)
            {
                applicableTemplateIds.Add("image_description_withTypes_light");
                applicableTemplateIds.Add("image_description_withTypes_dark");
                // ...and the _noclaims version
                applicableTemplateIds.Add("image_description_withTypes_noclaims_light");
                applicableTemplateIds.Add("image_description_withTypes_noclaims_dark");
            }
        }

        // 2) Templates that require image but no description
        if (hasImage)
        {
            // Regular
            applicableTemplateIds.Add("image_no_description_light");
            applicableTemplateIds.Add("image_no_description_dark");
            applicableTemplateIds.Add("image_no_description_small_light");
            applicableTemplateIds.Add("image_no_description_small_dark");
            // _noclaims
            applicableTemplateIds.Add("image_no_description_noclaims_light");
            applicableTemplateIds.Add("image_no_description_noclaims_dark");
            applicableTemplateIds.Add("image_no_description_small_noclaims_light");
            applicableTemplateIds.Add("image_no_description_small_noclaims_dark");

            // Also add the _withTypes variation if we have types
            if (hasTypes)
            {
                applicableTemplateIds.Add("image_no_description_withTypes_light");
                applicableTemplateIds.Add("image_no_description_withTypes_dark");
                // ...and the _noclaims version
                applicableTemplateIds.Add("image_no_description_withTypes_noclaims_light");
                applicableTemplateIds.Add("image_no_description_withTypes_noclaims_dark");
            }
        }

        // 3) Templates that require description but no image
        if (hasDescription)
        {
            // Regular
            applicableTemplateIds.Add("noimage_description_light");
            applicableTemplateIds.Add("noimage_description_dark");
            applicableTemplateIds.Add("noimage_description_small_light");
            applicableTemplateIds.Add("noimage_description_small_dark");
            // _noclaims
            applicableTemplateIds.Add("noimage_description_noclaims_light");
            applicableTemplateIds.Add("noimage_description_noclaims_dark");
            applicableTemplateIds.Add("noimage_description_small_noclaims_light");
            applicableTemplateIds.Add("noimage_description_small_noclaims_dark");

            // Also add the _withTypes variation if we have types
            if (hasTypes)
            {
                applicableTemplateIds.Add("noimage_description_withTypes_light");
                applicableTemplateIds.Add("noimage_description_withTypes_dark");
                // ...and the _noclaims version
                applicableTemplateIds.Add("noimage_description_withTypes_noclaims_light");
                applicableTemplateIds.Add("noimage_description_withTypes_noclaims_dark");
            }
        }

        // 4) Templates that require neither image nor description
        // Always add these
        applicableTemplateIds.Add("noimage_no_description_light");
        applicableTemplateIds.Add("noimage_no_description_dark");
        applicableTemplateIds.Add("noimage_no_description_small_light");
        applicableTemplateIds.Add("noimage_no_description_small_dark");
        // ...and the _noclaims version
        applicableTemplateIds.Add("noimage_no_description_noclaims_light");
        applicableTemplateIds.Add("noimage_no_description_noclaims_dark");
        applicableTemplateIds.Add("noimage_no_description_small_noclaims_light");
        applicableTemplateIds.Add("noimage_no_description_small_noclaims_dark");

        // Also add _withTypes if we have types
        if (hasTypes)
        {
            applicableTemplateIds.Add("noimage_no_description_withTypes_light");
            applicableTemplateIds.Add("noimage_no_description_withTypes_dark");
            // ...and the _noclaims version
            applicableTemplateIds.Add("noimage_no_description_withTypes_noclaims_light");
            applicableTemplateIds.Add("noimage_no_description_withTypes_noclaims_dark");
        }

        return applicableTemplateIds;
    }

    public List<string> GetFilteredTypes(VerifiedCredential credential)
    {
        var result = CredentialParser.Parse(credential.Credential);

        if (result.Value is AchievementCredential achievementCredential)
        {
            var achievementCredentialTypes = achievementCredential.Type;
            var subjectType = achievementCredential.CredentialSubject.Type ?? new List<string>();
            var achievementType = achievementCredential.CredentialSubject?.Achievement?.Type ?? new List<string>();
            var achievementType2 = achievementCredential.CredentialSubject?.Achievement?.AchievementType?.ToString() ?? "";

            var combinedType = achievementCredentialTypes
                .Concat(subjectType)
                .Concat(achievementType)
                .Concat(new List<string>() { achievementType2 })
                .ToList();

            var filteredTypes = combinedType
                .Where(x => !string.IsNullOrEmpty(x)
                            && !x.Equals("VerifiableCredential", StringComparison.InvariantCultureIgnoreCase)
                            && !x.Equals("AchievementSubject", StringComparison.InvariantCultureIgnoreCase)
                            && !x.Equals("Achievement", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (!filteredTypes.Any())
            {
                return new List<string>() { "Achievement" };
            }

            return filteredTypes;
        }
        else if (result.Value is EndorsementCredential endorsementCredential)
        {
            return new List<string>() { "Endorsement" };
        }

        if (credential.Types != null && credential.Types.Count > 0)
        {
            return credential.Types.Where(x => !string.IsNullOrEmpty(x)).ToList();
        }

        return new List<string>();
    }
}