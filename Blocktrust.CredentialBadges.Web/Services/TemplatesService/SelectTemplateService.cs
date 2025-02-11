using Blocktrust.CredentialBadges.Web.Domain;
using System.Collections.Generic;

namespace Blocktrust.CredentialBadges.Web.Services.TemplatesService;

using OpenBadges;

public class SelectTemplateService
{
    public List<string> GetApplicableTemplateIds(VerifiedCredential credential)
    {
        // Determine if the credential has an image or a description
        bool hasImage = !string.IsNullOrEmpty(credential.Image);
        bool hasDescription = !string.IsNullOrEmpty(credential.Description);

        // Determine if the credential has non-empty "filtered" types
        // (Example logic similar to what you showed; adapt as needed)
        var filteredTypes = GetFilteredTypes(credential);
        bool hasTypes = filteredTypes.Any();

        var applicableTemplateIds = new List<string>();

        // Templates that require both image and description
        if (hasImage && hasDescription)
        {
            applicableTemplateIds.Add("image_description_light");
            applicableTemplateIds.Add("image_description_dark");

            // Also add the _withTypes variation if we have types
            if (hasTypes)
            {
                applicableTemplateIds.Add("image_description_light_withTypes");
                applicableTemplateIds.Add("image_description_dark_withTypes");
            }
        }

        // Templates that require image but no description
        if (hasImage)
        {
            applicableTemplateIds.Add("image_no_description_light");
            applicableTemplateIds.Add("image_no_description_dark");

            // Also add the _withTypes variation if we have types
            if (hasTypes)
            {
                applicableTemplateIds.Add("image_no_description_light_withTypes");
                applicableTemplateIds.Add("image_no_description_dark_withTypes");
            }
        }

        // Templates that require description but no image
        if (hasDescription)
        {
            applicableTemplateIds.Add("noimage_description_light");
            applicableTemplateIds.Add("noimage_description_dark");

            // Also add the _withTypes variation if we have types
            if (hasTypes)
            {
                applicableTemplateIds.Add("noimage_description_light_withTypes");
                applicableTemplateIds.Add("noimage_description_dark_withTypes");
            }
        }

        // Templates that require neither image nor description
        // Always add these
        applicableTemplateIds.Add("noimage_no_description_light");
        applicableTemplateIds.Add("noimage_no_description_dark");

        // Also add _withTypes if we have types
        if (hasTypes)
        {
            applicableTemplateIds.Add("noimage_no_description_light_withTypes");
            applicableTemplateIds.Add("noimage_no_description_dark_withTypes");
        }

        return applicableTemplateIds;
    }

    private List<string> GetFilteredTypes(VerifiedCredential credential)
    {
        //TODO WIP

        // // If credential is actually an AchievementCredential, adapt to your scenario:
        // if (credential is AchievementCredential achievementCredential)
        // {
        //     var achievementCredentialTypes = achievementCredential.Type;
        //     var subjectType = achievementCredential.CredentialSubject.Type;
        //     var achievementType = achievementCredential.CredentialSubject.Achievement.Type;
        //
        //     var combinedType = achievementCredentialTypes
        //         .Concat(subjectType)
        //         .Concat(achievementType)
        //         .ToList();
        //
        //     var filteredTypes = combinedType
        //         .Where(x => !string.IsNullOrEmpty(x)
        //                     && !x.Equals("VerifiableCredential", StringComparison.InvariantCultureIgnoreCase)
        //                     && !x.Equals("AchievementSubject", StringComparison.InvariantCultureIgnoreCase))
        //         .ToList();
        //
        //     return filteredTypes;
        // }
        //
        // if (credential.Types != null && credential.Types.Count > 0)
        // {
        //     return credential.Types.Where(x => !string.IsNullOrEmpty(x)).ToList();
        // }

        // No types
        return new List<string>();
    }
}