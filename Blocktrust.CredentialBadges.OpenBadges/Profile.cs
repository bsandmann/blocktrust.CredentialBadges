namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

public class Profile
{
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "Profile" };

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public Uri? Url { get; set; }

    // [JsonPropertyName("phone")]
    // public PhoneNumber? Phone { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    // [JsonPropertyName("endorsement")]
    // public List<EndorsementCredential>? Endorsement { get; set; }

    // [JsonPropertyName("endorsementJwt")]
    // public List<CompactJws>? EndorsementJwt { get; set; }

    [JsonPropertyName("image")]
    public Image? Image { get; set; }

    // [JsonPropertyName("email")]
    // public EmailAddress? Email { get; set; }

    // [JsonPropertyName("address")]
    // public Address? Address { get; set; }

    // [JsonPropertyName("otherIdentifier")]
    // public List<IdentifierEntry>? OtherIdentifier { get; set; }

    [JsonPropertyName("official")]
    public string? Official { get; set; }

    [JsonPropertyName("parentOrg")]
    public Profile? ParentOrg { get; set; }

    [JsonPropertyName("familyName")]
    public string? FamilyName { get; set; }

    [JsonPropertyName("givenName")]
    public string? GivenName { get; set; }

    [JsonPropertyName("additionalName")]
    public string? AdditionalName { get; set; }

    [JsonPropertyName("patronymicName")]
    public string? PatronymicName { get; set; }

    [JsonPropertyName("honorificPrefix")]
    public string? HonorificPrefix { get; set; }

    [JsonPropertyName("honorificSuffix")]
    public string? HonorificSuffix { get; set; }

    [JsonPropertyName("familyNamePrefix")]
    public string? FamilyNamePrefix { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public DateTime? DateOfBirth { get; set; }
}

