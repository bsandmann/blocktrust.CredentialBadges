// using System.Text;
// using System.Text.Json;
// using System.Text.Json.Nodes;
// using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
// using Blocktrust.CredentialBadges.Core.Crypto;
// using FluentResults;
// using SimpleBase;
// using Org.BouncyCastle.Crypto.Signers;
// using Org.BouncyCastle.Crypto.Parameters;
//
// namespace Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;
//
// public class DIDKeySignatureVerification
// {
//     private readonly EcServiceBouncyCastle _ecService;
//
//     public DIDKeySignatureVerification(EcServiceBouncyCastle ecService)
//     {
//         _ecService = ecService;
//     }
//
//     public Result<ECheckSignatureResponse> VerifySignature(string credentialJson)
//     {
//         try
//         {
//             var credentialObject = JsonNode.Parse(credentialJson).AsObject();
//     
//             // Extract proof and issuer information
//             var proof = credentialObject["proof"];
//             var issuerDid = credentialObject["issuer"]["id"].GetValue<string>();
//             var proofValue = proof["proofValue"].GetValue<string>();
//
//             // Extract canonicalized document
//             var (canonicalizedDocument, extractionError) = ExtractCanonicalizedDocument(credentialObject);
//             if (extractionError != null)
//             {
//                 return Result.Fail<ECheckSignatureResponse>(extractionError);
//             }
//
//
//             // Convert the canonicalized document to bytes
//             var message = Encoding.UTF8.GetBytes(canonicalizedDocument);
//
//             var publicKeyMultibase = ExtractPublicKeyMultibase(issuerDid);
//             var publicKey = ConvertMultibaseToPublicKey(publicKeyMultibase);
//
//             bool isValid = VerifySignatureInternal(message, proofValue, publicKey);
//
//             return isValid
//                 ? Result.Ok(ECheckSignatureResponse.Valid)
//                 : Result.Ok(ECheckSignatureResponse.Invalid);
//         }
//         catch (Exception ex)
//         {
//             return Result.Fail<ECheckSignatureResponse>($"Error during DID Key signature verification: {ex.Message}");
//         }
//     }
//     
// private (string document, string error) ExtractCanonicalizedDocument(JsonObject credentialObject)
// {
//     try
//     {
//         // Create a new object to hold the document without the "proof" property
//         var documentWithoutProof = new JsonObject();
//
//         // Recursively deep clone and add each property except "proof"
//         foreach (var property in credentialObject)
//         {
//             if (property.Key != "proof")
//             {
//                 documentWithoutProof.Add(property.Key, DeepClone(property.Value));
//             }
//         }
//
//         // Convert the JSON object to a sorted, compact JSON string
//         var canonicalizedDocument = SerializeSorted(documentWithoutProof);
//
//         return (canonicalizedDocument, null);
//     }
//     catch (Exception ex)
//     {
//         return (null, $"Error during document canonicalization: {ex.Message}");
//     }
// }
//
// // Helper method to recursively deep clone a JsonNode
// private JsonNode DeepClone(JsonNode node)
// {
//     switch (node)
//     {
//         case JsonObject jsonObject:
//             var clonedObject = new JsonObject();
//             foreach (var property in jsonObject)
//             {
//                 clonedObject.Add(property.Key, DeepClone(property.Value));
//             }
//             return clonedObject;
//
//         case JsonArray jsonArray:
//             var clonedArray = new JsonArray();
//             foreach (var item in jsonArray)
//             {
//                 clonedArray.Add(DeepClone(item));
//             }
//             return clonedArray;
//
//         case JsonValue jsonValue:
//             return JsonNode.Parse(jsonValue.ToJsonString()); // Creates a new instance of the primitive value
//
//         default:
//             return node;
//     }
// }
//
// // Helper method to serialize a JSON object with sorted keys
// private string SerializeSorted(JsonObject jsonObject)
// {
//     var sortedObject = new SortedDictionary<string, JsonNode?>();
//
//     foreach (var property in jsonObject)
//     {
//         sortedObject.Add(property.Key, property.Value);
//     }
//
//     return JsonSerializer.Serialize(sortedObject, new JsonSerializerOptions
//     {
//         WriteIndented = false,
//         Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
//         PropertyNamingPolicy = null, // Keep original case
//         DictionaryKeyPolicy = null,  // Keep original case
//         DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
//     });
// }
//
//
//
//
//     public string ExtractPublicKeyMultibase(string didKey)
//     {
//         return didKey.Substring(8);
//     }
//
//     public byte[] ConvertMultibaseToPublicKey(string multibaseKey)
//     {
//         if (string.IsNullOrEmpty(multibaseKey) || multibaseKey[0] != 'z')
//         {
//             throw new ArgumentException("Invalid multibase key. Must start with 'z'.", nameof(multibaseKey));
//         }
//         string encodedKey = multibaseKey.Substring(1);
//         byte[] decodedBytes = Base58.Bitcoin.Decode(encodedKey).ToArray();
//         return decodedBytes.Skip(2).ToArray();
//     }
//
//     public bool VerifySignatureInternal(byte[] message, string proofValue, byte[] publicKey)
//     {
//         try
//         {
//             byte[] signature = Base58.Bitcoin.Decode(proofValue.StartsWith("z") ? proofValue.Substring(1) : proofValue).ToArray();
//             return VerifyEd25519Signature(message, signature, publicKey);
//         }
//         catch (Exception)
//         {
//             return false;
//         }
//     }
//
//     public bool VerifyEd25519Signature(byte[] message, byte[] signature, byte[] publicKey)
//     {
//         try
//         {
//             var verifier = new Ed25519Signer();
//             verifier.Init(false, new Ed25519PublicKeyParameters(publicKey));
//             verifier.BlockUpdate(message, 0, message.Length);
//             return verifier.VerifySignature(signature);
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error in VerifyEd25519Signature: {ex.Message}");
//             return false;
//         }
//     
//     }
// }