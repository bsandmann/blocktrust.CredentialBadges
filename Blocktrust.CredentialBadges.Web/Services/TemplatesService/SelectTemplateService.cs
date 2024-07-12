using Blocktrust.CredentialBadges.Web.Domain;
using System.Collections.Generic;

namespace Blocktrust.CredentialBadges.Web.Services.TemplatesService;

public class SelectTemplateService
{
    public List<string> GetApplicableTemplateIds(VerifiedCredential credential)
    {
        var applicableTemplateIds = new List<string>();
        bool hasImage = !string.IsNullOrEmpty(credential.Image);
        bool hasDescription = !string.IsNullOrEmpty(credential.Description);

        // Templates that require both image and description
        if (hasImage && hasDescription)
        {
            applicableTemplateIds.Add("image_description_light");
            applicableTemplateIds.Add("image_description_dark");
        }

        // Templates that require image but no description
        if (hasImage)
        {
            applicableTemplateIds.Add("image_no_description_light");
            applicableTemplateIds.Add("image_no_description_dark");
        }

        // Templates that require description but no image
        if (hasDescription)
        {
            applicableTemplateIds.Add("noimage_description_light");
            applicableTemplateIds.Add("noimage_description_dark");
        }

        // Templates that require neither image nor description
        applicableTemplateIds.Add("noimage_no_description_light");
        applicableTemplateIds.Add("noimage_no_description_dark");

        return applicableTemplateIds;
    }
}