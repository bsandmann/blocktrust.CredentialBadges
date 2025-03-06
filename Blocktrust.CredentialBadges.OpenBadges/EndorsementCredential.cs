namespace Blocktrust.CredentialBadges.OpenBadges;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// A verifiable credential that asserts a claim about an entity. As described in § 8. Proofs (Signatures), 
/// at least one proof mechanism, and the details necessary to evaluate that proof, MUST be expressed for a 
/// credential to be a verifiable credential. In the case of an embedded proof, the credential MUST append the 
/// proof in the proof property.
/// </summary>
public class EndorsementCredential : OpenBadgeCredential
{
    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the items MUST be the URI 'VerifiableCredential', 
    /// and one of the items MUST be the URI 'EndorsementCredential'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; }

    /// <summary>
    /// The name of the credential for display purposes in wallets. For example, in a list of credentials and in detail views. [1]
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The individual, entity, organization, assertion, or achievement that is endorsed and the endorsement comment. [1]
    /// </summary>
    [JsonPropertyName("credentialSubject")]
    public required EndorsementSubject CredentialSubject { get; set; }
}
