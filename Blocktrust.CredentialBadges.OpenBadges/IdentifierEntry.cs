namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#identifierentry"/>
/// </summary>
public class IdentifierEntry
{
    /// <summary>
    /// The value of the type property MUST be an unordered set.
    /// One of the items MUST be the IRI 'IdentifierEntry'. [1]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "IdentifierEntry" };

    /// <summary>
    /// An identifier. [1]
    /// </summary>
    [JsonPropertyName("identifier")]
    public required string Identifier { get; set; }

    /// <summary>
    /// The type of identifier. [0..1]
    /// </summary>
    [JsonPropertyName("identifierType")]
    public EIdentifierType? IdentifierType { get; set; }
}