using Blocktrust.CredentialBadges.Web.Domain;

namespace Blocktrust.CredentialBadges.Web.Services.TemplatesService;

public class SelectTemplateService
{
    private readonly TemplatesService _templatesService;

    public SelectTemplateService(TemplatesService templatesService)
    {
        _templatesService = templatesService;
    }

    public string GetApplicableTemplates(VerifiedCredential credential)
    {
        var applicableTemplates = new List<string>();
        bool hasImage = !string.IsNullOrEmpty(credential.Image);
        bool hasDescription = !string.IsNullOrEmpty(credential.Description);

        if (hasImage && hasDescription)
        {
            applicableTemplates.Add("image_description_light");
            applicableTemplates.Add("image_description_dark");
        }
        
        if (hasImage && !hasDescription)
        {
            applicableTemplates.Add("image_no_description_light");
            applicableTemplates.Add("image_no_description_dark");
        }
        
        if (!hasImage && hasDescription)
        {
            applicableTemplates.Add("noimage_description_light");
            applicableTemplates.Add("noimage_description_dark");
        }
        
        if (!hasImage && !hasDescription)
        {
            applicableTemplates.Add("noimage_no_description_light");
            applicableTemplates.Add("noimage_no_description_dark");
        }

        return CombineTemplates(applicableTemplates, credential);
    }

    private string CombineTemplates(List<string> templateIds, VerifiedCredential credential)
    {
        var combinedTemplates = new List<string>();

        foreach (var templateId in templateIds)
        {
            string theme = templateId.EndsWith("dark") ? "dark" : "light";
            string populatedTemplate = _templatesService.GetPopulatedTemplate(templateId, theme, credential);
            combinedTemplates.Add(populatedTemplate);
        }

        return string.Join(Environment.NewLine, combinedTemplates);
    }
}