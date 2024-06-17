namespace Blocktrust.CredentialBadges.Core.Common;

using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;
using OpenBadges;
using Result = FluentResults.Result;

public static class CredentialParser
{
    // TODO we currently only handle VCs here. Add support for VPs 
    public static Result<OpenBadgeCredential> Parse(string rawInput)
    {
        if (string.IsNullOrEmpty(rawInput))
        {
            return Result.Fail("Input cannot be empty");
        }
        else if (rawInput.Trim().StartsWith("{") && rawInput.Trim().EndsWith("}"))
        {
            // We naively assume it is a raw JSON object / credential with no JWT-signature
            return DeserializeAsOpenBadgeCredential(rawInput);
        }
        else if (rawInput.Trim().StartsWith("ey"))
        {
            // we naivly assume it is an jwt token and try to decode it
            var parsedJwt = JwtParser.ExtractHeaderAndPayloadFromBase64(rawInput);
            if (parsedJwt.IsFailed)
            {
                return Result.Fail($"Credential was assumed to be an JWT but could not decode:  {parsedJwt.Errors.FirstOrDefault()?.Message}");
            }

            if (string.IsNullOrEmpty(parsedJwt.Value.PayloadAsJson))
            {
                return Result.Fail("Could not extract payload from JWT.");
            }

            var openBadgeCredentialResult = DeserializeAsOpenBadgeCredential(parsedJwt.Value.PayloadAsJson);
            if (openBadgeCredentialResult.IsFailed)
            {
                return openBadgeCredentialResult.ToResult();
            }

            // Add the JWT information to the credential
            openBadgeCredentialResult.Value.Jwt = parsedJwt.Value;

            return Result.Ok(openBadgeCredentialResult.Value);
        }
        else
        {
            // we naivly assume it is an base64 encoded string and try to decode it
            string? decodedString = null;
            try
            {
                var decodedBase64 = Base64Url.Decode(rawInput.Trim());
                decodedString = System.Text.Encoding.UTF8.GetString(decodedBase64);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return Result.Fail($"Credential was assumed to be an Base64 decoded VC but could not decode:  {ex.Message}");
            }

            if (string.IsNullOrEmpty(decodedString))
            {
                return Result.Fail($"Credential was assumed to be an Base64 decoded VC but could not decode: empty string");
            }

            if (!decodedString.StartsWith("ey"))
            {
                return Result.Fail($"Credential was assumed to be an Base64 and to contain a JWT but could not find JWT");
            }
            
            var parsedJwt = JwtParser.ExtractHeaderAndPayloadFromBase64(decodedString);
            if (parsedJwt.IsFailed)
            {
                return Result.Fail($"Credential was assumed to be an JWT but could not decode:  {parsedJwt.Errors.FirstOrDefault()?.Message}");
            }

            if (string.IsNullOrEmpty(parsedJwt.Value.PayloadAsJson))
            {
                return Result.Fail("Could not extract payload from JWT.");
            }

            var openBadgeCredentialResult = DeserializeAsOpenBadgeCredential(parsedJwt.Value.PayloadAsJson);
            if (openBadgeCredentialResult.IsFailed)
            {
                return openBadgeCredentialResult.ToResult();
            }

            // Add the JWT information to the credential
            openBadgeCredentialResult.Value.Jwt = parsedJwt.Value;

            return Result.Ok(openBadgeCredentialResult.Value); 
        }
    }

    private static Result<OpenBadgeCredential> DeserializeAsOpenBadgeCredential(string rawInput)
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        var isEndorsementCredentialResult = IsEndorsementCredential(rawInput);
        if (isEndorsementCredentialResult.IsFailed)
        {
            return isEndorsementCredentialResult.ToResult();
        }

        if (isEndorsementCredentialResult.Value)
        {
            try
            {
                var deserializedObject = JsonSerializer.Deserialize<EndorsementCredential>(rawInput, options);
                if (deserializedObject is null)
                {
                    return Result.Fail<OpenBadgeCredential>("Deserialization returned null.");
                }

                return Result.Ok<OpenBadgeCredential>(deserializedObject);
            }
            catch (JsonException ex)
            {
                return Result.Fail<OpenBadgeCredential>($"Deserialization error: {ex.Message}");
            }
        }
        else
        {
            try
            {
                var deserializedObject = JsonSerializer.Deserialize<AchievementCredential>(rawInput, options);
                if (deserializedObject is null)
                {
                    return Result.Fail<OpenBadgeCredential>("Deserialization returned null.");
                }

                return Result.Ok<OpenBadgeCredential>(deserializedObject);
            }
            catch (JsonException ex)
            {
                return Result.Fail<OpenBadgeCredential>($"Deserialization error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Method to determine if the input is an EndorsementCredential or an OpenBadgeCredential
    /// This method can be improved by parsing it as a JsonDocument and checking the type property
    /// specifically, but for now we just check if the input contains the relevant strings
    /// </summary>
    /// <param name="rawInput"></param>
    /// <returns></returns>
    private static Result<bool> IsEndorsementCredential(string rawInput)
    {
        if (rawInput.Contains("OpenBadgeCredential") && !rawInput.Contains("EndorsementCredential"))
        {
            return Result.Ok(false);
        }
        else if (rawInput.Contains("EndorsementCredential") && !rawInput.Contains("OpenBadgeCredential"))
        {
            return Result.Ok(true);
        }
        else if (rawInput.Contains("EndorsementCredential") && rawInput.Contains("OpenBadgeCredential"))
        {
            // Simple assumption that we are dealing with a credential with embbded endorsements
            // like the "CompleteOpenBadgeCredential.json" in the tests
            return Result.Ok(false);
        }
        else
        {
            return Result.Fail("Could not determine the type of the credential to either be a OpenBadgeCredential or an EndorsementCredential");
        }
    }
}