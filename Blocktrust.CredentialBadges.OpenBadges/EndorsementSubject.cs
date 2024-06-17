namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// A collection of information about the subject of the endorsement.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#endorsementsubject"/>
/// </summary>
public class EndorsementSubject
{
    /// <summary>
    /// The identifier of the individual, entity, organization, assertion, or achievement that is endorsed. [1]
    /// </summary>
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the items MUST be the URI 'EndorsementSubject'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "EndorsementSubject" };

    /// <summary>
    /// Allows endorsers to make a simple claim in writing about the entity. [0..1]
    /// </summary>
    [JsonPropertyName("endorsementComment")]
    public string? EndorsementComment { get; set; }
}