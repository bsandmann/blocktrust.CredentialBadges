namespace Blocktrust.CredentialBadges.OpenBadges;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// AchievementCredentials are representations of an awarded achievement, used to share information 
/// about an achievement belonging to one earner. Maps to a Verifiable Credential as defined in the 
/// [VC-DATA-MODEL-2.0]. As described in § 8. Proofs (Signatures), at least one proof mechanism, 
/// and the details necessary to evaluate that proof, MUST be expressed for a credential to be a 
/// verifiable credential. In the case of an embedded proof, the credential MUST append the proof in the proof property.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#achievementcredential"/>
/// </summary>
public class AchievementCredential : OpenBadgeCredential
{
    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the items MUST be the URI 'VerifiableCredential', 
    /// and one of the items MUST be the URI 'AchievementCredential' or the URI 'OpenBadgeCredential'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; }

    /// <summary>
    /// The name of the credential for display purposes in wallets. For example, in a list of credentials and in detail views. [0..1]
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The image representing the credential for display purposes in wallets. [0..1]
    /// </summary>
    [JsonPropertyName("image")]
    public Image? Image { get; set; }

    /// <summary>
    /// The recipient of the achievement. [1]
    /// </summary>
    [JsonPropertyName("credentialSubject")]
    public required AchievementSubject CredentialSubject { get; set; }

    /// <summary>
    /// Allows endorsers to make specific claims about the credential, and the achievement and profiles in the credential. 
    /// These endorsements are signed with a Data Integrity proof format. [0..*]
    /// </summary>
    [JsonPropertyName("endorsement")]
    public List<EndorsementCredential>? Endorsement { get; set; }

    /// <summary>
    /// Allows endorsers to make specific claims about the credential, and the achievement and profiles in the credential. 
    /// These endorsements are signed with the VC-JWT proof format. [0..*]
    /// </summary>
    [JsonPropertyName("endorsementJwt")]
    public List<string>? EndorsementJwt { get; set; }

    /// <summary>
    /// A description of the work that the recipient did to earn the achievement. 
    /// This can be a page that links out to other pages if linking directly to the work is infeasible. [0..*]
    /// </summary>
    [JsonPropertyName("evidence")]
    public List<Evidence>? Evidence { get; set; }
}
