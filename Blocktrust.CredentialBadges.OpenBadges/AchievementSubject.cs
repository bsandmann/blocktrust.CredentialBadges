namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// A collection of information about the recipient of an achievement.
/// Maps to Credential Subject in [VC-DATA-MODEL-2.0].
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#achievementsubject"/>
/// </summary>
public class AchievementSubject
{
    /// <summary>
    /// An identifier for the Credential Subject. Either id or at least one
    /// identifier MUST be supplied. [0..1]
    /// </summary>
    [JsonPropertyName("id")]
    public Uri? Id { get; set; }
    
    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the items
    /// MUST be the IRI 'AchievementSubject'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; }

    /// <summary>
    /// The datetime the activity ended. [0..1]
    /// </summary>
    [JsonPropertyName("activityEndDate")]
    public DateTime? ActivityEndDate { get; set; }
    
    /// <summary>
    /// The datetime the activity started. [0..1]
    /// </summary>
    [JsonPropertyName("activityStartDate")]
    public DateTime? ActivityStartDate { get; set; }
    
    /// <summary>
    /// The number of credits earned, generally in semester or quarter credit hours.
    /// This field correlates with the Achievement creditsAvailable field. [0..1]
    /// </summary>
    [JsonPropertyName("creditsEarned")]
    public decimal? CreditsEarned { get; set; }
    
    /// <summary>
    /// The achievement being awarded. [1]
    /// </summary>
    [JsonPropertyName("achievement")]
    public required Achievement Achievement { get; set; }
    
    /// <summary>
    /// Other identifiers for the recipient of the achievement. Either id or at
    /// least one identifier MUST be supplied. [0..*]
    /// </summary>
    [JsonPropertyName("identifier")]
    public List<IdentityObject>? Identifier { get; set; }
    
    /// <summary>
    /// An image representing this user's achievement. If present, this must be a
    /// PNG or SVG image, and should be prepared via the 'baking' instructions.
    /// An 'unbaked' image for the achievement is defined in the Achievement class
    /// and should not be duplicated here. [0..1]
    /// </summary>
    [JsonPropertyName("image")]
    public Image? Image { get; set; }
    
    /// <summary>
    /// The license number that was issued with this credential. [0..1]
    /// </summary>
    [JsonPropertyName("licenseNumber")]
    public string? LicenseNumber { get; set; } 
    
    /// <summary>
    /// A narrative that connects multiple pieces of evidence. Likely only present
    /// at this location if evidence is a multi-value array. [0..1]
    /// </summary>
    [JsonPropertyName("narrative")]
    public string? Narrative { get; set; } 
    
    /// <summary>
    /// The set of results being asserted. [0..*]
    /// </summary>
    [JsonPropertyName("result")]
    public List<Result>? Result { get; set; } 
    
    /// <summary>
    /// Role, position, or title of the learner when demonstrating or performing
    /// the achievement or evidence of learning being asserted. Examples include
    /// 'Student President', 'Intern', 'Captain', etc. [0..1]
    /// </summary>
    [JsonPropertyName("role")]
    public string? Role { get; set; } 
    
    /// <summary>
    /// The person, organization, or system that assessed the achievement on behalf
    /// of the issuer. For example, a school may assess the achievement, while the
    /// school district issues the credential. [0..1]
    /// </summary>
    [JsonPropertyName("source")]
    public Profile? Source { get; set; } 
    
    /// <summary>
    /// The academic term in which this assertion was achieved. [0..1]
    /// </summary>
    [JsonPropertyName("term")]
    public string? Term { get; set; } 
}