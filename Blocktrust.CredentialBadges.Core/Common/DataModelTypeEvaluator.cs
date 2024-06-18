namespace Blocktrust.CredentialBadges.Core.Common;

using OpenBadges;

public static class DataModelTypeEvaluator
{
    public static EDataModelType Evaluate(OpenBadgeCredential credential)
    {
        var contexts = credential.Context.Select(p => p.ToString()).ToList();
        if (contexts.Contains("https://www.w3.org/2018/credentials/v1"))
        {
            return EDataModelType.DataModel11;
        }
        else if (contexts.Contains("https://www.w3.org/ns/credentials/v2"))
        {
            return EDataModelType.DataModel2;
        }

        return EDataModelType.Undefined;
    }
}