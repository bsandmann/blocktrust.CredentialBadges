using Blocktrust.CredentialBadges.Web.Entities;
using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Domain;

public class VerifiedCredential
{
    private const string KeySeparator = "<|key|>";
    private const string PairSeparator = "<|value|>";
    private const int MaxClaimsLength = 5000;

    public Guid Id { get; set; }
    public EVerificationStatus Status { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public List<string> Types { get; set; }
    public string? Image { get; set; }
    public string Credential { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string Issuer { get; set; }
    public string TemplateId { get; set; }
    public string? Domain { get; set; }
    public Dictionary<string, string>? Claims { get; set; }
    public string? SubjectId { get; set; }
    public string? SubjectName { get; set; }

    public VerifiedCredentialEntity ToEntity()
    {
        return new VerifiedCredentialEntity
        {
            StoredCredentialId = Id,
            Status = Status,
            Image = Image,
            Credential = Credential,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
            Issuer = Issuer,
            TemplateId = TemplateId,
            Domain = Domain,
            Claims = SerializeClaims(Claims),
            SubjectId = SubjectId,
            SubjectName = SubjectName
        };
    }

    public static VerifiedCredential FromEntity(VerifiedCredentialEntity entity)
    {
        return new VerifiedCredential
        {
            Id = entity.StoredCredentialId,
            Status = entity.Status,
            Image = entity.Image,
            Credential = entity.Credential,
            ValidFrom = entity.ValidFrom,
            ValidUntil = entity.ValidUntil,
            Issuer = entity.Issuer,
            TemplateId = entity.TemplateId,
            Domain = entity.Domain,
            Claims = DeserializeClaims(entity.Claims),
            SubjectId = entity.SubjectId,
            SubjectName = entity.SubjectName
        };
    }

    /// <summary>
    /// Convert a Dictionary to a single string with special delimiters.
    /// Example format for each pair: "key<|key|>value"
    /// Pairs are separated by "<|value|>".
    /// </summary>
    private static string? SerializeClaims(Dictionary<string, string>? claims)
    {
        if (claims == null || claims.Count == 0)
        {
            return null;
        }

        var pairs = claims.Select(kvp => $"{kvp.Key}{KeySeparator}{kvp.Value}");
        var joined = string.Join(PairSeparator, pairs);

        // Truncate if longer than 5000 chars (extreme edge case)
        if (joined.Length > MaxClaimsLength)
        {
            joined = joined.Substring(0, MaxClaimsLength);
        }

        return joined;
    }

    /// <summary>
    /// Convert the single string with special delimiters back into a Dictionary.
    /// </summary>
    private static Dictionary<string, string>? DeserializeClaims(string? claimsString)
    {
        if (string.IsNullOrWhiteSpace(claimsString))
        {
            return null;
        }

        var dictionary = new Dictionary<string, string>();

        // Split by the pair separator
        var pairs = claimsString.Split(new[] { PairSeparator }, StringSplitOptions.None);

        foreach (var pair in pairs)
        {
            // Split each pair by the key separator
            var parts = pair.Split(new[] { KeySeparator }, 2, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                dictionary[parts[0]] = parts[1];
            }
            // If parts.Length != 2, we ignore that pair
        }

        return dictionary;
    }
}
