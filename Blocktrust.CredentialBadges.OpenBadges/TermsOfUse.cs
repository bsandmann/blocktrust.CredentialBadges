namespace Blocktrust.CredentialBadges.OpenBadges;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// Terms of use can be utilized by an issuer or a holder to communicate the terms 
/// under which a verifiable credential or verifiable presentation was issued.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#termsofuse"/>
/// </summary>
public class TermsOfUse
{
    /// <summary>
    /// The value MUST be a URI identifying the term of use. [0..1]
    /// </summary>
    [JsonPropertyName("id")]
    public Uri? Id { get; set; }

    /// <summary>
    /// The value MUST identify the type of the terms of use. [1]
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }
}