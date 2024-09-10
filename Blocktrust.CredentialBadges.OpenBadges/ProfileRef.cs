namespace Blocktrust.CredentialBadges.OpenBadges;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// A description of the individual, entity, or organization that issued the credential. 
/// Either a URI with the Unique URI for the Issuer/Profile file, or a Profile object MUST be supplied.
/// </summary>
public class ProfileRef
{
    /// <summary>
    /// A URI representing the Unique URI for the Issuer/Profile file. [0..1]
    /// </summary>
    [JsonPropertyName("id")]
    public Uri? Id { get; set; }

    // /// <summary>
    // /// A Profile is a collection of information that describes the entity or organization using Open Badges. 
    // /// Issuers must be represented as Profiles, and endorsers, or other entities may also be represented using this vocabulary. 
    // /// Each Profile that represents an Issuer may be referenced in many BadgeClasses that it has defined. 
    // /// Anyone can create and host an Issuer file to start issuing Open Badges. Issuers may also serve as recipients of Open Badges, 
    // /// often identified within an Assertion by specific properties, like their url or contact email address. [0..1]
    // /// </summary>
    // [JsonPropertyName("profile")]
    // public Profile? Profile { get; set; }
    

    // TODO Copied over from the Profile class for easier serialization
    // Maybe custom serializer, or just use the Profile class directly without
    // an required Id ?
    
    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the
    /// items MUST be the IRI 'Profile'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "Profile" };

    /// <summary>
    /// The name of the entity or organization. [0..1]
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The homepage or social media profile of the entity, whether individual
    /// or institutional. Should be a URL/URI Accessible via HTTP. [0..1]
    /// </summary>
    [JsonPropertyName("url")]
    public Uri? Url { get; set; }

    /// <summary>
    /// A phone number. [0..1]
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// A short description of the issuer entity or organization. [0..1]
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; } 
    
      /// <summary>
    /// An image representing the issuer. This must be a PNG or SVG image. [0..1]
    /// </summary>
    [JsonPropertyName("image")]
    [JsonConverter(typeof(ImageJsonConverter))]
    public Image? Image { get; set; }

    /// <summary>
    /// An email address. [0..1]
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// An address for the individual or organization. [0..1]
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    /// <summary>
    /// A list of identifiers for the described entity. [0..*]
    /// </summary>
    [JsonPropertyName("otherIdentifier")]
    public List<IdentifierEntry>? OtherIdentifier { get; set; }

    /// <summary>
    /// If the entity is an organization, official is the name of an authorized
    /// official of the organization. [0..1]
    /// </summary>
    [JsonPropertyName("official")]
    public string? Official { get; set; }

    /// <summary>
    /// The parent organization of the entity. [0..1]
    /// </summary>
    [JsonPropertyName("parentOrg")]
    public Profile? ParentOrg { get; set; }

    /// <summary>
    /// Family name. In the western world, often referred to as the
    /// 'last name' of a person. [0..1]
    /// </summary>
    [JsonPropertyName("familyName")]
    public string? FamilyName { get; set; }

    /// <summary>
    /// Given name. In the western world, often referred to as the
    /// 'first name' of a person. [0..1]
    /// </summary>
    [JsonPropertyName("givenName")]
    public string? GivenName { get; set; }

    /// <summary>
    /// Additional name. Includes what is often referred to as
    /// 'middle name' in the western world. [0..1]
    /// </summary>
    [JsonPropertyName("additionalName")]
    public string? AdditionalName { get; set; }

    /// <summary>
    /// Patronymic name. [0..1]
    /// </summary>
    [JsonPropertyName("patronymicName")]
    public string? PatronymicName { get; set; }

    /// <summary>
    /// Honorific prefix(es) preceding a person's name
    /// (e.g. 'Dr', 'Mrs' or 'Mr'). [0..1]
    /// </summary>
    [JsonPropertyName("honorificPrefix")]
    public string? HonorificPrefix { get; set; }

    /// <summary>
    /// Honorific suffix(es) following a person's name
    /// (e.g. 'M.D, PhD'). [0..1]
    /// </summary>
    [JsonPropertyName("honorificSuffix")]
    public string? HonorificSuffix { get; set; }

    /// <summary>
    /// Family name prefix. As used in some locales, this is the leading part
    /// of a family name (e.g. 'de' in the name 'de Boer'). [0..1]
    /// </summary>
    [JsonPropertyName("familyNamePrefix")]
    public string? FamilyNamePrefix { get; set; }

    /// <summary>
    /// Birthdate of the person. [0..1]
    /// </summary>
    [JsonPropertyName("dateOfBirth")]
    public DateTime? DateOfBirth { get; set; }
}