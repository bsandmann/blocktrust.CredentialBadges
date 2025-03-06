using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blocktrust.CredentialBadges.OpenBadges;

public class ProfileRefConverter : JsonConverter<ProfileRef>
{
    public override ProfileRef? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        // If the JSON is just a string (e.g. "did:prism:..."),
        // then interpret that string as the ProfileRef.Id
        if (reader.TokenType == JsonTokenType.String)
        {
            var issuerString = reader.GetString();
            // If you want to treat "did:..." as a valid URI,
            // you can do something like:
            //     new Uri(issuerString, UriKind.RelativeOrAbsolute)
            // or validate it in your own way.
            return new ProfileRef
            {
                Id = new Uri(issuerString!,
                    UriKind.RelativeOrAbsolute),
                Type = new List<string>() { "Profile" }
            };
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using var doc = JsonDocument.ParseValue(ref reader);

            // Make a clone of the incoming options
            var noProfileRefConverterOptions = new JsonSerializerOptions(options);

            // Remove or skip adding your converter to avoid recursion
            for (int i = noProfileRefConverterOptions.Converters.Count - 1; i >= 0; i--)
            {
                if (noProfileRefConverterOptions.Converters[i] is ProfileRefConverter)
                {
                    noProfileRefConverterOptions.Converters.RemoveAt(i);
                }
            }

            var result = JsonSerializer.Deserialize<ProfileRef>(
                doc.RootElement.GetRawText(),
                noProfileRefConverterOptions);

            return result;
        }

        // Otherwise, throw if JSON has some other unexpected format.
        throw new JsonException($"Expected string or object for ProfileRef, but got {reader.TokenType}.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        ProfileRef value,
        JsonSerializerOptions options)
    {
        // For serialization, decide how you want to write out ProfileRef. 
        // If *all* you care about is the Id, and it’s the only field populated, 
        // you might just want to serialize it as a string. Otherwise, 
        // write the full object.
        
        // Simple example:
        if (value != null 
            && value.Id != null 
            && OnlyHasId(value))
        {
            writer.WriteStringValue(value.Id.ToString());
        }
        else
        {
            JsonSerializer.Serialize(writer, (object?)value, options);
        }
    }

    private bool OnlyHasId(ProfileRef value)
    {
        // Check if everything else is null or default
        return value.Type == null ||
               value.Type.Count == 0 ||
               (value.Type.Count == 1 && value.Type[0] == "Profile") &&
               value.Name == null &&
               value.Url == null &&
               value.Phone == null &&
               value.Description == null &&
               value.Image == null &&
               value.Email == null &&
               value.Address == null &&
               value.OtherIdentifier == null &&
               value.Official == null &&
               value.ParentOrg == null &&
               value.FamilyName == null &&
               value.GivenName == null &&
               value.AdditionalName == null &&
               value.PatronymicName == null &&
               value.HonorificPrefix == null &&
               value.HonorificSuffix == null &&
               value.FamilyNamePrefix == null &&
               value.DateOfBirth == null;
    }
}
