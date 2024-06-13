namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

public class Achievement
{
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "Achievement" };

    [JsonPropertyName("alignment")]
    public List<Alignment>? Alignment { get; set; }

    [JsonPropertyName("achievementType")]
    public AchievementType? AchievementType { get; set; }

    [JsonPropertyName("creator")]
    public Profile? Creator { get; set; }

    [JsonPropertyName("creditsAvailable")]
    public float? CreditsAvailable { get; set; }

    [JsonPropertyName("criteria")]
    public required Criteria Criteria { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    // [JsonPropertyName("endorsement")]
    // public List<EndorsementCredential>? Endorsement { get; set; }

    // [JsonPropertyName("endorsementJwt")]
    // public List<CompactJws>? EndorsementJwt { get; set; }

    [JsonPropertyName("fieldOfStudy")]
    public string? FieldOfStudy { get; set; }

    [JsonPropertyName("humanCode")]
    public string? HumanCode { get; set; }

    [JsonPropertyName("image")]
    public Image? Image { get; set; }

    [JsonPropertyName("inLanguage")]
    public string? InLanguage { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    // [JsonPropertyName("otherIdentifier")]
    // public List<IdentifierEntry>? OtherIdentifier { get; set; }

    [JsonPropertyName("related")]
    public List<Related>? Related { get; set; }

    [JsonPropertyName("resultDescription")]
    public List<ResultDescription>? ResultDescription { get; set; }

    [JsonPropertyName("specialization")]
    public string? Specialization { get; set; }

    [JsonPropertyName("tag")]
    public List<string>? Tag { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
}

// Assuming the enums and classes for Alignment, AchievementType, Profile, Criteria, EndorsementCredential, CompactJws, Image, LanguageCode, IdentifierEntry, Related, and ResultDescription are defined elsewhere in your codebase.
