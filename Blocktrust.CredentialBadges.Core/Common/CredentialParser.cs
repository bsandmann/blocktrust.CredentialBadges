namespace Blocktrust.CredentialBadges.Core.Common;

using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;
using OpenBadges;
using Result = FluentResults.Result;

public static class CredentialParser
{
    /// <summary>
    /// Parses a raw input string and returns an OpenBadgeCredential
    /// Takes a raw credential as json, a JWT or a Base64 encoded string (which contians a JWT)
    /// </summary>
    /// <param name="rawInput"></param>
    /// <returns></returns>
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
            var openBadgeCredentialResult = ParseJwt(rawInput);
            if (openBadgeCredentialResult.IsFailed)
            {
                return openBadgeCredentialResult.ToResult();
            }

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

            var openBadgeCredentialResult = ParseJwt(decodedString);
            if (openBadgeCredentialResult.IsFailed)
            {
                return openBadgeCredentialResult.ToResult();
            }

            return Result.Ok(openBadgeCredentialResult.Value);
        }
    }

    /// <summary>
    /// Deconstructs the JWT into its parts and then parses the payload into a OpenBadgeCredential
    /// </summary>
    /// <param name="rawInput"></param>
    /// <returns></returns>
    private static Result<OpenBadgeCredential> ParseJwt(string rawInput)
    {
        var jwt = ExtractHeaderAndPayloadFromJwt(rawInput);
        if (jwt.IsFailed)
        {
            return jwt.ToResult();
        }

        var parsedJwt = Parse(jwt.Value);
        if (parsedJwt.IsFailed)
        {
            return Result.Fail($"Credential was assumed to be an JWT but could not decode:  {parsedJwt.Errors.FirstOrDefault()?.Message}");
        }

        if (parsedJwt.Value.OpenBadgeCredentials.Count == 0)
        {
            return Result.Fail("Could not extract payload from JWT: No OpenBadgeCredentials found.");
        }
        else if (parsedJwt.Value.OpenBadgeCredentials.Count > 1)
        {
            return Result.Fail("Could not extract payload from JWT: Multiple OpenBadgeCredentials found. Only one is supported.");
        }

        return parsedJwt.Value.OpenBadgeCredentials.Single();
    }

    /// <summary>
    /// Deserializes the Credential-Content as an OpenBadgeCredential or an EndorsementCredential
    /// </summary>
    /// <param name="rawInput"></param>
    /// <returns></returns>
    private static Result<OpenBadgeCredential> DeserializeAsOpenBadgeCredential(string rawInput)
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new ImageJsonConverter(), new ProofListJsonConverter(), new ProfileRefConverter() }
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

                deserializedObject.RawData = rawInput;
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

                deserializedObject.RawData = rawInput;
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
        // TODO The implementation of this part of the code cannot be done properly atm
        // as the OpenBadgeCredential created by Identus can't be 100% conform to the spec
        // will use a simple heuristic for now

        if (rawInput.Contains("EndorsementSubject") && !rawInput.Contains("AchievementSubject"))
        {
            return Result.Ok(true);
        }
        else
        {
            return Result.Ok(false);
        }
    }


    private static readonly List<string> AllowedJwtAlgorithms = new List<string>()
    {
        "HS256",
        "HS384",
        "HS512",
        "RS256",
        "RS384",
        "RS512",
        "ES256",
        "ES384",
        "ES512",
        "ES256K",
        "PS256",
        "PS384",
        "PS512",
        "EDDSA",
        "NONE"
    };


    private static readonly List<string> SupportedJwtAlgorithms = new List<string>()
    {
        "ES256K",
        "EDDSA",
        "NONE"
    };

    /// <summary>
    /// Deconstructs the JWT-string into its parts
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private static Result<JwtModel> ExtractHeaderAndPayloadFromJwt(string content)
    {
        var parts = content.Trim().Split('.');
        if (parts.Length == 2 || parts.Length == 3)
        {
            var headersAsBytes = Base64Url.Decode(parts[0]);
            var headersAsString = System.Text.Encoding.UTF8.GetString(headersAsBytes);
            var headers = JsonSerializer.Deserialize<Dictionary<string, object>>(headersAsString);
            if (headers is null || headers.Count == 0)
            {
                return Result.Fail("Invalid JWT. Could not extract headers.");
            }

            var payloadAsBytes = Base64Url.Decode(parts[1]);
            var payloadAsString = System.Text.Encoding.UTF8.GetString(payloadAsBytes);
            var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadAsString);
            if (payload is null || payload.Count == 0)
            {
                return Result.Fail("Invalid JWT. Could not extract payload.");
            }

            if (parts.Length == 3)
            {
                // JWT with signature
                if (string.IsNullOrEmpty(parts[2]))
                {
                    return Result.Fail("Invalid JWT. Could not extract signature.");
                }

                return Result.Ok(new JwtModel(headers.ToDictionary(), payload, parts[2], headersAsString, payloadAsString));
            }
            else if (parts.Length == 2)
            {
                // JWT without signature    
                return Result.Ok(new JwtModel(headers.ToDictionary(), payload, null, headersAsString, payloadAsString));
            }
        }


        return Result.Fail("Invalid JWT in Base64 format.");
    }

    /// <summary>
    /// Deconstructs the JWT into its parts and validates the headers and payload
    /// Properties like iss and sub are then added to the OpenBadgeCredential
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private static Result<ParsedJwt> Parse(JwtModel jwt)
    {
        if (jwt.HeadersAsJson is null)
        {
            return Result.Fail("This parser requires the JWT headers to be in JSON format.");
        }

        var headers = JsonDocument.Parse(jwt.HeadersAsJson);
        var typ = headers.RootElement.TryGetProperty("typ", out var typElement) ? typElement.GetString() : null;
        if (typ is null || !typ.Equals("JWT", StringComparison.InvariantCultureIgnoreCase))
        {
            //TODO turned off. Do reaseach on this
            // return Result.Fail("Invalid JWT. The typ header must be 'JWT'.");
        }

        var alg = headers.RootElement.TryGetProperty("alg", out var algElement) ? algElement.GetString() : null;
        if (string.IsNullOrEmpty(alg))
        {
            return Result.Fail("Invalid JWT. The alg header must be present.");
        }

        if (!AllowedJwtAlgorithms.Contains(alg.ToUpperInvariant()))
        {
            return Result.Fail($"Invalid JWT. The alg header must be one of the following: {string.Join(", ", AllowedJwtAlgorithms)}");
        }

        //Execute this code only in a production environment
#if RELEASE
        if (!SupportedJwtAlgorithms.Contains(alg.ToUpperInvariant()))
        {
            return Result.Fail($"Invalid JWT. Only these alg-headers are currently supported: {string.Join(", ", SupportedJwtAlgorithms)}");
        }
#endif
        var kid = headers.RootElement.TryGetProperty("kid", out var kidElement) ? kidElement.GetString() : null;

        var exp = jwt.Payload.TryGetValue("exp", out var expElement) ? expElement.ToString() : null;
        var iss = jwt.Payload.TryGetValue("iss", out var issElement) ? issElement.ToString() : null;
        var nbf = jwt.Payload.TryGetValue("nbf", out var nbfElement) ? nbfElement.ToString() : null;
        var sub = jwt.Payload.TryGetValue("sub", out var subElement) ? subElement.ToString() : null;
        var jti = jwt.Payload.TryGetValue("jti", out var jtiElement) ? jtiElement.ToString() : null;
        var aud = jwt.Payload.TryGetValue("aud", out var audElement) ? audElement.ToString() : null;

        var hasVc = jwt.Payload.TryGetValue("vc", out var vcElement);
        var hasVp = jwt.Payload.TryGetValue("vp", out var vpElement);
        var vcList = new List<OpenBadgeCredential>();
        if (hasVc && vcElement is JsonElement vcJsonElement)
        {
            if (vcJsonElement.ValueKind == JsonValueKind.Object)
            {
                try
                {
                    var vc = CredentialParser.DeserializeAsOpenBadgeCredential(vcJsonElement.GetRawText());
                    if (vc.IsFailed)
                    {
                        return Result.Fail("Invalid JWT. Could not extract Verifiable Credential.");
                    }

                    vc.Value.DataModelType = DataModelTypeEvaluator.Evaluate(vc.Value);

                    vcList.Add(vc.Value);
                }
                catch (Exception e)
                {
                    return Result.Fail("Invalid JWT. Could not deserialize OpenBadge Credential.");
                }
            }
            else if (vcJsonElement.ValueKind == JsonValueKind.Array)
            {
                var multipleVc = JsonSerializer.Deserialize<List<OpenBadgeCredential>>(vcJsonElement.GetRawText());
                if (multipleVc is null || multipleVc.Count > 1)
                {
                    return Result.Fail("Multiple Verifiable Credentials found in JWT. Only one is supported in this version.");
                }
            }
            else
            {
                return Result.Fail("Invalid JWT. Could not extract Verifiable Credential.");
            }
        }
        else if (hasVp && vpElement is JsonElement vpJsonElement)
        {
            return Result.Fail("Invalid JWT. Could not extract Verifiable Presentation - VC is expected");
        }

        // Post-validation if the JWT claims match the Verifiable Credential
        if (exp is not null)
        {
            // If exp is present, the UNIX timestamp MUST be converted to an [XMLSCHEMA11-2] date-time, and MUST be used
            // to set the value of the expirationDate property of credentialSubject of the new JSON object.
            for (var i = 0; i < vcList.Count; i++)
            {
                var expirationDateOfCredential = vcList[i].ValidUntil;
                if (expirationDateOfCredential is null && vcList[i].DataModelType == EDataModelType.DataModel11)
                {
                    // // The expirationDate property was likely removed from the Credential and just placed in the header as "exp" claim
                    vcList[i].ValidUntil = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).DateTime;
                }
                else if (expirationDateOfCredential is null && vcList[i].DataModelType == EDataModelType.DataModel2)
                {
                    vcList[i].ValidUntil = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).DateTime;
                }
                else if (expirationDateOfCredential is not null && vcList[i].DataModelType == EDataModelType.DataModel11)
                {
                    if (expirationDateOfCredential.Value != DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).DateTime)
                    {
                        vcList[i].ValidUntil = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).DateTime;
                    }
                }
                else if (expirationDateOfCredential is not null && vcList[i].DataModelType == EDataModelType.DataModel2)
                {
                    throw new NotImplementedException();
                }
            }
        }

        if (nbf is not null)
        {
            // If nbf is present, the UNIX timestamp MUST be converted to an [XMLSCHEMA11-2] date-time, and MUST be 
            // used to set the value of the issuanceDate property of the new JSON object.
            for (var i = 0; i < vcList.Count; i++)
            {
                var issuanceDateOfCredential = vcList[i].ValidFrom;
                if (vcList[i].DataModelType == EDataModelType.DataModel11)
                {
                    // // The issuanceDate property was likely removed from the Credential and just placed in the header as "nbf" claim
                    vcList[i].ValidFrom = DateTimeOffset.FromUnixTimeSeconds(long.Parse(nbf)).DateTime;
                }
                else if (vcList[i].DataModelType == EDataModelType.DataModel2)
                {
                    vcList[i].ValidFrom = DateTimeOffset.FromUnixTimeSeconds(long.Parse(nbf)).DateTime;
                }
                else if (vcList[i].DataModelType == EDataModelType.DataModel11)
                {
                    if (vcList[i].ValidFrom != DateTimeOffset.FromUnixTimeSeconds(long.Parse(nbf)).DateTime)
                    {
                        vcList[i].ValidFrom = DateTimeOffset.FromUnixTimeSeconds(long.Parse(nbf)).DateTime;
                    }
                }
                else if (vcList[i].DataModelType == EDataModelType.DataModel2)
                {
                    throw new NotImplementedException();
                }
            }
        }

        if (iss is not null)
        {
            // If iss is present, the value MUST be used to set the issuer property of the new credential JSON object
            // or the holder property of the new presentation JSON object.
            for (var i = 0; i < vcList.Count; i++)
            {
                var issuerOfCredential = vcList[i].Issuer;
                if (issuerOfCredential is null)
                {
                    // usually the issuer of a credemtial is a required property, but in this case we can add it from the JWT
                    // The issuer property was likely removed from the Credential and just placed in the header as "iss" claim
                    var profileRef = new ProfileRef
                    {
                        Id = new Uri(iss),
                        Type = new List<string>() { "Profile" }
                    };
                    vcList[i].Issuer = profileRef;
                }
                else
                {
                    var issUri = new Uri(iss);
                    issuerOfCredential.Id = issUri;
                }
            }
        }

        if (sub is not null)
        {
            // If sub is present, the value MUST be used to set the value of the id property of credentialSubject of the new credential JSON object. 
            for (int i = 0; i < vcList.Count; i++)
            {
                var subUri = new Uri(sub);

                if (vcList[i] is AchievementCredential)
                {
                    // note that the Id is optional here
                    (vcList[i] as AchievementCredential).CredentialSubject.Id = subUri;
                }
                else if (vcList[i] is EndorsementCredential)
                {
                    // but required here
                    (vcList[i] as EndorsementCredential).CredentialSubject.Id = subUri;
                }
                else
                {
                    return Result.Fail("Invalid JWT. Could not extract Verifiable Credential: Could not determine the type of OpenBadge.");
                }
            }
        }

        if (jti is not null)
        {
            // if jti is present, the value MUST be used to set the value of the id property of the new JSON object.
            if (vcList.Count == 1)
            {
                // TODO there are some cases this application currently doesn't handle
                throw new NotImplementedException();
                if (vcList[0].Id is null)
                {
                    // The id property was likely removed from the Credential and just placed in the header as "jti" claim
                    // var jtiIsUri = Uri.TryCreate(jti, UriKind.RelativeOrAbsolute, out Uri? jtiUri);
                    // if (jtiIsUri)
                    // {
                    //     vcList[0] = vcList[0] with { Id = jtiUri };
                    //     // we take not of this modification so that we can potentially rebuild the idential JWT 
                    //     vcList[0] = vcList[0] with { JwtParsingArtefact = vcList[0].JwtParsingArtefact! with { RemoveIdFromCredentialOrPresentationAndReplaceWithClaim = true } };
                    // }
                }
                else if (vcList[0].Id is not null && !vcList[0].Id!.OriginalString.Equals(jti, StringComparison.InvariantCultureIgnoreCase) && !vcList[0].Id!.AbsoluteUri.Equals(jti, StringComparison.InvariantCultureIgnoreCase))
                {
                    // vcList[0].JwtParsingArtefact!.JwtParsingWarnings!.Add("The jti-claims (JWT ID) does not match the id property of the verifiable credential. The id property of the verifiable credential will be overritten with the 'jti' value of the JWT.");
                    // var jtiIsUri = Uri.TryCreate(jti, UriKind.RelativeOrAbsolute, out Uri? jtiUri);
                    // if (jtiIsUri)
                    // {
                    //     vcList[0] = vcList[0] with { Id = jtiUri };
                    // }
                }
            }
            else if (vcList.Count > 1)
            {
                var idWasChanged = false;
                for (int i = 0; i < vcList.Count; i++)
                {
                    // TODO there are some cases this application currently doesn't handle
                    throw new NotImplementedException();

                    if (vcList[i].Id is null)
                    {
                        // if (idWasChanged)
                        // {
                        //     // The id property was likely removed from the Credential and just placed in the header as "jti" claim
                        //     vcList[i].JwtParsingArtefact!.JwtParsingWarnings!.Add("Multiple Id properties of Verifiable Credentials are set to the same id based on the jti-claims (JWT ID). This is not recommended!");
                        //     // we take not of this modification so that we can potentially rebuild the idential JWT 
                        //     vcList[i] = vcList[i] with { JwtParsingArtefact = vcList[i].JwtParsingArtefact! with { RemoveIdFromCredentialOrPresentationAndReplaceWithClaim = true } };
                        // }
                        //
                        // var jtiIsUri = Uri.TryCreate(jti, UriKind.RelativeOrAbsolute, out Uri? jtiUri);
                        // if (jtiIsUri)
                        // {
                        //     vcList[i] = vcList[i] with { Id = jtiUri };
                        //     idWasChanged = true;
                        // }
                    }
                    else if (vcList[i].Id is not null && !vcList[i].Id!.OriginalString.Equals(jti, StringComparison.InvariantCultureIgnoreCase) && !vcList[i].Id!.AbsoluteUri.Equals(jti, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // if (idWasChanged)
                        // {
                        //     vcList[i].JwtParsingArtefact!.JwtParsingWarnings!.Add("Multiple Id properties of Verifiable Credentials are set to the same id based on the jti-claims (JWT ID). This is not recommended!");
                        // }
                        //
                        // vcList[i].JwtParsingArtefact!.JwtParsingWarnings!.Add("The jti-claims (JWT ID) does not match the id property of the verifiable credential. The id property of the verifiable credential will be overritten with the 'jti' value of the JWT.");
                        // var jtiIsUri = Uri.TryCreate(jti, UriKind.RelativeOrAbsolute, out Uri? jtiUri);
                        // if (jtiIsUri)
                        // {
                        //     vcList[i] = vcList[i] with { Id = jtiUri };
                        //     idWasChanged = true;
                        // }
                    }
                }
            }
        }

        var reservedClaimsList = new List<string>() { "sub", "iss", "aud", "exp", "nbf", "jti", "vc", "vp" };
        var additionalPayloadData = jwt.Payload.Where(p => !reservedClaimsList.Contains(p.Key, StringComparer.InvariantCultureIgnoreCase)).ToDictionary();

        if (vcList.Any())
        {
            if (additionalPayloadData.Any())
            {
                // TODO there are some cases this application currently doesn't handle
                throw new NotImplementedException();
            }

            vcList.ForEach(p => p.Jwt = jwt);
            return Result.Ok(new ParsedJwt() { OpenBadgeCredentials = vcList });
        }
        else
        {
            return Result.Fail("Invalid JWT. Could not extract Verifiable Credential or Verifiable Presentation.");
        }
    }
}