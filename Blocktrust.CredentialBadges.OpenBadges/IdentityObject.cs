namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// A collection of information about the recipient of an achievement.
/// </summary>
public class IdentityObject
{
    /// <summary>
    /// MUST be the IRI 'IdentityObject'. [1]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "IdentityObject" };

    /// <summary>
    /// Whether or not the identityHash value is hashed. [1]
    /// </summary>
    [JsonPropertyName("hashed")]
    public required bool Hashed { get; set; }

    /// <summary>
    /// Either the IdentityHash of the identity or the plaintext value. If it's possible that the plaintext
    /// transmission and storage of the identity value would leak personally identifiable information where
    /// there is an expectation of privacy, it is strongly recommended that an IdentityHash be used. [1]
    /// </summary>
    [JsonPropertyName("identityHash")]
    public required string IdentityHash { get; set; }

    /// <summary>
    /// The identity type. [1]
    /// </summary>
    [JsonPropertyName("identityType")]
    public required EIdentifierType IdentityType { get; set; }

    /// <summary>
    /// If the identityHash is hashed, this should contain the string used to salt the hash. If this value is
    /// not provided, it should be assumed that the hash was not salted. [0..1]
    /// </summary>
    [JsonPropertyName("salt")]
    public string? Salt { get; set; }
}