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
public class AchievementCredential
{
    /// <summary>
    /// The value of the @context property MUST be an ordered set where the first item is a URI with 
    /// the value 'https://www.w3.org/ns/credentials/v2', and the second item is a URI with the value 
    /// 'https://purl.imsglobal.org/spec/ob/v3p0/context-3.0.3.json'. [1..*]
    /// </summary>
    [JsonPropertyName("@context")]
    public required List<Uri> Context { get; set; }

    /// <summary>
    /// Unambiguous reference to the credential. [1]
    /// </summary>
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the items MUST be the URI 'VerifiableCredential', 
    /// and one of the items MUST be the URI 'AchievementCredential' or the URI 'OpenBadgeCredential'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "VerifiableCredential", "AchievementCredential" };

    /// <summary>
    /// The name of the credential for display purposes in wallets. For example, in a list of credentials and in detail views. [0..1]
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The short description of the credential for display purposes in wallets. [0..1]
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The image representing the credential for display purposes in wallets. [0..1]
    /// </summary>
    [JsonPropertyName("image")]
    public Image? Image { get; set; }

    /// <summary>
    /// Timestamp of when the credential was awarded. [0..1]
    /// </summary>
    [JsonPropertyName("awardedDate")]
    public DateTime? AwardedDate { get; set; }

    /// <summary>
    /// The recipient of the achievement. [1]
    /// </summary>
    [JsonPropertyName("credentialSubject")]
    public required AchievementSubject CredentialSubject { get; set; }

    // Currently Not supported by us
    // /// <summary>
    // /// Allows endorsers to make specific claims about the credential, and the achievement and profiles in the credential. 
    // /// These endorsements are signed with a Data Integrity proof format. [0..*]
    // /// </summary>
    // [JsonPropertyName("endorsement")]
    // public List<EndorsementCredential>? Endorsement { get; set; }

    // Currently Not supported by us
    // /// <summary>
    // /// Allows endorsers to make specific claims about the credential, and the achievement and profiles in the credential. 
    // /// These endorsements are signed with the VC-JWT proof format. [0..*]
    // /// </summary>
    // [JsonPropertyName("endorsementJwt")]
    // public List<string>? EndorsementJwt { get; set; }

    /// <summary>
    /// A description of the work that the recipient did to earn the achievement. 
    /// This can be a page that links out to other pages if linking directly to the work is infeasible. [0..*]
    /// </summary>
    [JsonPropertyName("evidence")]
    public List<Evidence>? Evidence { get; set; }

    /// <summary>
    /// A description of the individual, entity, or organization that issued the credential. [1]
    /// </summary>
    [JsonPropertyName("issuer")]
    public required ProfileRef Issuer { get; set; }

    /// <summary>
    /// Timestamp of when the credential becomes valid. [1]
    /// </summary>
    [JsonPropertyName("validFrom")]
    public required DateTime ValidFrom { get; set; }

    /// <summary>
    /// If the credential has some notion of validity period, this indicates a timestamp 
    /// when a credential should no longer be considered valid. After this time, the credential should be considered invalid. [0..1]
    /// </summary>
    [JsonPropertyName("validUntil")]
    public DateTime? ValidUntil { get; set; }

    /// <summary>
    /// If present, one or more embedded cryptographic proofs that can be used to detect tampering 
    /// and verify the authorship of the credential. [0..*]
    /// </summary>
    [JsonPropertyName("proof")]
    public List<Proof>? Proof { get; set; }

    /// <summary>
    /// The value of the credentialSchema property MUST be one or more data schemas that provide verifiers 
    /// with enough information to determine if the provided data conforms to the provided schema. [0..*]
    /// </summary>
    [JsonPropertyName("credentialSchema")]
    public List<CredentialSchema>? CredentialSchema { get; set; }

    /// <summary>
    /// The information in CredentialStatus is used to discover information about the current status of a verifiable credential, 
    /// such as whether it is suspended or revoked. [0..1]
    /// </summary>
    [JsonPropertyName("credentialStatus")]
    public CredentialStatus? CredentialStatus { get; set; }

    /// <summary>
    /// The information in RefreshService is used to refresh the verifiable credential. [0..1]
    /// </summary>
    [JsonPropertyName("refreshService")]
    public RefreshService? RefreshService { get; set; }

    /// <summary>
    /// The value of the termsOfUse property tells the verifier what actions it is required to perform (an obligation), 
    /// not allowed to perform (a prohibition), or allowed to perform (a permission) if it is to accept the verifiable credential. [0..*]
    /// </summary>
    [JsonPropertyName("termsOfUse")]
    public List<TermsOfUse>? TermsOfUse { get; set; }
}
