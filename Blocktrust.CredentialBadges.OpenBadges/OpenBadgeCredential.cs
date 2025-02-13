namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(AchievementCredential), "achievement")]
[JsonDerivedType(typeof(EndorsementCredential), "endorsement")]
public class OpenBadgeCredential
{
    /// <summary>
    /// The value of the @context property MUST be an ordered set where the first item is a URI with the value 
    /// 'https://www.w3.org/ns/credentials/v2', and the second item is a URI with the value 
    /// 'https://purl.imsglobal.org/spec/ob/v3p0/context-3.0.3.json'. [1..*]
    /// </summary>
    [JsonPropertyName("@context")]
    public required List<Uri> Context { get; set; }

    /// <summary>
    /// Unambiguous reference to the credential. [1]
    /// This property is acutally required, but for deserialization from an JWT purposes it is marked as optional 
    /// </summary>
    [JsonPropertyName("id")]
    public Uri Id { get; set; }

    /// <summary>
    /// The short description of the credential for display purposes in wallets. [0..1]
    /// </summary> 
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Timestamp of when the credential was awarded. validFrom is used to determine the most recent version of a Credential 
    /// in conjunction with issuer and id. Consequently, the only way to update a Credential is to update the validFrom, 
    /// losing the date when the Credential was originally awarded. awardedDate is meant to keep this original date. [0..1]
    /// </summary>
    [JsonPropertyName("awardedDate")]
    public DateTime? AwardedDate { get; set; }

    /// <summary>
    /// A description of the individual, entity, or organization that issued the credential. [1]
    /// This property is acutally required, but for deserialization from an JWT purposes it is marked as optional 
    /// </summary>
    [JsonPropertyName("issuer")]
    [JsonConverter(typeof(ProfileRefConverter))]
    public ProfileRef Issuer { get; set; }

    /// <summary>
    /// Timestamp of when the credential becomes valid. [1]
    /// This property is acutally required, but for deserialization from an JWT purposes it is marked as optional
    /// Note, that this property is used in v2 data model, but not in v1
    /// </summary>
    [JsonPropertyName("validFrom")]
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// Timestamp of when the credential becomes valid. [1]
    /// This property is acutally required, but for deserialization from an JWT purposes it is marked as optional 
    /// Note, that this property is used in v1 data model, but not in v2 
    /// </summary>
    [JsonPropertyName("issuanceDate")]
    public DateTime? IssuanceDate { get; set; }

    /// <summary>
    /// If the credential has some notion of validity period, this indicates a timestamp when a credential should no longer 
    /// be considered valid. After this time, the credential should be considered invalid. [0..1]
    /// Note, that this property is used in v2 data model, but not in v1 
    /// </summary>
    [JsonPropertyName("validUntil")]
    public DateTime? ValidUntil { get; set; }

    /// <summary>
    /// If the credential has some notion of validity period, this indicates a timestamp when a credential should no longer 
    /// be considered valid. After this time, the credential should be considered invalid. [0..1]
    /// Note, that this property is used in v1 data model, but not in v2 
    /// </summary>
    [JsonPropertyName("expirationDate")]
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// If present, one or more embedded cryptographic proofs that can be used to detect tampering and verify the authorship 
    /// of the credential. [0..*]
    /// </summary>
    [JsonPropertyName("proof")]
    [JsonConverter(typeof(ProofListJsonConverter))]
    public List<Proof>? Proof { get; set; }

    /// <summary>
    /// The value of the credentialSchema property MUST be one or more data schemas that provide verifiers with enough information 
    /// to determine if the provided data conforms to the provided schema. [0..*]
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
    /// The value of the termsOfUse property tells the verifier what actions it is required to perform (an obligation), not allowed 
    /// to perform (a prohibition), or allowed to perform (a permission) if it is to accept the verifiable credential. [0..*]
    /// </summary>
    [JsonPropertyName("termsOfUse")]
    public List<TermsOfUse>? TermsOfUse { get; set; }

    /// <summary>
    /// Optional property to capture the complete JWT as it was created 
    /// </summary>
    [JsonIgnore]
    public JwtModel? Jwt { get; set; }

    /// <summary>
    /// Flag to determine the type of data model used for the credential (1.0 or 2.0)
    /// </summary>
    [JsonIgnore]
    public EDataModelType DataModelType { get; set; }
    
    /// <summary>
    /// The raw untouched data of the json-form of the credential
    /// </summary>
    [JsonIgnore] 
    public string RawData { get; set; }
}