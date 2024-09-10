using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blocktrust.CredentialBadges.OpenBadges
{
    public class ProofListJsonConverter : JsonConverter<List<Proof>>
    {
        public override List<Proof> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Single proof object
                var proof = JsonSerializer.Deserialize<Proof>(ref reader, options);
                return new List<Proof> { proof };
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Array of proof objects
                return JsonSerializer.Deserialize<List<Proof>>(ref reader, options);
            }

            throw new JsonException("Invalid proof format");
        }

        public override void Write(Utf8JsonWriter writer, List<Proof> value, JsonSerializerOptions options)
        {
            if (value.Count == 1)
            {
                // Single proof object
                JsonSerializer.Serialize(writer, value[0], options);
            }
            else
            {
                // Array of proof objects
                JsonSerializer.Serialize(writer, value, options);
            }
        }
    }
}