using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blocktrust.CredentialBadges.OpenBadges
{
    public class ImageJsonConverter : JsonConverter<Image>
    {
        public override Image Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string imageUrl = reader.GetString();
                return new Image
                {
                    Id = new Uri(imageUrl),
                    Type = "Image"
                };
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                {
                    JsonElement root = doc.RootElement;

                    Image image = new Image
                    {
                        Id = !string.IsNullOrWhiteSpace(root.GetProperty("id").GetString()) ? new Uri(root.GetProperty("id").GetString()) : null,
                        Type = root.GetProperty("type").GetString()
                    };

                    if (root.TryGetProperty("caption", out JsonElement captionElement))
                    {
                        image.Caption = captionElement.GetString();
                    }

                    return image;
                }
            }

            throw new JsonException("Invalid image format");
        }

        public override void Write(Utf8JsonWriter writer, Image value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("id", value.Id.ToString());
            writer.WriteString("type", value.Type);
            if (!string.IsNullOrEmpty(value.Caption))
            {
                writer.WriteString("caption", value.Caption);
            }

            writer.WriteEndObject();
        }
    }
}