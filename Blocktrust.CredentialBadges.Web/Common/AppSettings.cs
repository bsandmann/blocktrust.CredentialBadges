namespace Blocktrust.CredentialBadges.Web.Common;

public class AppSettings
{
    public PrismDidSettings PrismDid { get; set; }
}

public class PrismDidSettings
{
    public string BaseUrl { get; set; }
    public string DefaultLedger { get; set; }
}