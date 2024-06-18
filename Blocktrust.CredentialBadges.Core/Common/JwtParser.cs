namespace Blocktrust.CredentialBadges.Core.Common;

using System.Text.Json;
using FluentResults;
using IdentusClientApi;
using OpenBadges;
using Result = FluentResults.Result;

public static class JwtParser
{
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

    public static Result<ParsedJwt> Parse(string base64String)
    {
        var jwt = ExtractHeaderAndPayloadFromBase64(base64String);
        if (jwt.IsFailed)
        {
            return jwt.ToResult();
        }

        return Parse(jwt.Value);
    }

    public static Result<JwtModel> ExtractHeaderAndPayloadFromBase64(string content)
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
                    var ff = vcJsonElement.GetRawText();
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