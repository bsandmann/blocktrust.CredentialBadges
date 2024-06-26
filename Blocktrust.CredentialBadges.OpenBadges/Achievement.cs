namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;
using Enums;

/// <summary>
/// A collection of information about the accomplishment recognized by the Assertion.
/// Many assertions may be created corresponding to one Achievement.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#achievement"/>
/// </summary>
public class Achievement
{
    /// <summary>
    /// Unique URI for the Achievement. [1]
    /// </summary>
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    /// <summary>
    /// Type of the Achievement. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "Achievement" };

    /// <summary>
    /// An object describing which objectives or educational standards this
    /// achievement aligns to, if any. [0..*]
    /// </summary>
    [JsonPropertyName("alignment")]
    public List<Alignment>? Alignment { get; set; }

    /// <summary>
    /// The type of achievement. This is an extensible vocabulary. [0..1]
    /// </summary>
    [JsonPropertyName("achievementType")]
    public EAchievementType? AchievementType { get; set; }

    /// <summary>
    /// The person or organization that created the achievement definition. [0..1]
    /// </summary>
    [JsonPropertyName("creator")]
    public Profile? Creator { get; set; }
    
    /// <summary>
    /// Credit hours associated with this entity, or credit hours possible.
    /// For example 3.0. [0..1] 
    /// </summary>
    [JsonPropertyName("creditsAvailable")]
    public float? CreditsAvailable { get; set; }

    /// <summary>
    /// Criteria describing how to earn the achievement. [1] 
    /// </summary>
    [JsonPropertyName("criteria")]
    public required Criteria Criteria { get; set; }

    /// <summary>
    /// A short description of the achievement. [1] 
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    /// <summary>
    /// Allows endorsers to make specific claims about the Achievement.
    /// These endorsements are signed with a Data Integrity proof format. [0..*] 
    /// </summary>
    [JsonPropertyName("endorsement")]
    public List<EndorsementCredential>? Endorsement { get; set; }
    
    /// <summary>
    /// Allows endorsers to make specific claims about the Achievement.
    /// These endorsements are signed with the VC-JWT proof format. [0..*] 
    /// </summary>
    [JsonPropertyName("endorsementJwt")]
    public List<string>? EndorsementJwt { get; set; }

    /// <summary>
    /// Category, subject, area of study, discipline, or general branch of
    /// knowledge. Examples include Business, Education, Psychology, and Technology. [0..1] 
    /// </summary>
    [JsonPropertyName("fieldOfStudy")]
    public string? FieldOfStudy { get; set; }

    /// <summary>
    /// The code, generally human readable, associated with an achievement. [0..1] 
    /// </summary>
    [JsonPropertyName("humanCode")]
    public string? HumanCode { get; set; }

    /// <summary>
    /// An image representing the achievement. [0..1] 
    /// </summary>
    [JsonPropertyName("image")]
    public Image? Image { get; set; }

    /// <summary>
    /// The language of the achievement. [0..1] 
    /// </summary>
    [JsonPropertyName("inLanguage")]
    public string? InLanguage { get; set; }

    /// <summary>
    /// The name of the achievement. [1] 
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// A list of identifiers for the described entity. [0..*] 
    /// </summary>
     [JsonPropertyName("otherIdentifier")]
     public List<IdentifierEntry>? OtherIdentifier { get; set; }

    /// <summary>
    /// The related property identifies another Achievement that should be
    /// considered the same for most purposes. It is primarily intended to
    /// identify alternate language editions or previous versions of Achievements. [0..*] 
    /// </summary>
    [JsonPropertyName("related")]
    public List<Related>? Related { get; set; }

    /// <summary>
    /// The set of result descriptions that may be asserted as results with this achievement. [0..*] 
    /// </summary>
    [JsonPropertyName("resultDescription")]
    public List<ResultDescription>? ResultDescription { get; set; }

    /// <summary>
    /// Name given to the focus, concentration, or specific area of study defined
    /// in the achievement. Examples include 'Entrepreneurship',
    /// 'Technical Communication', and 'Finance'. [0..1] 
    /// </summary>
    [JsonPropertyName("specialization")]
    public string? Specialization { get; set; }

    /// <summary>
    /// One or more short, human-friendly, searchable, keywords that describe
    /// the type of achievement. [0..*] 
    /// </summary>
    [JsonPropertyName("tag")]
    public List<string>? Tag { get; set; }

    /// <summary>
    /// The version property allows issuers to set a version string for an Achievement.
    /// This is particularly useful when replacing a previous version with an update. [0..1] 
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
