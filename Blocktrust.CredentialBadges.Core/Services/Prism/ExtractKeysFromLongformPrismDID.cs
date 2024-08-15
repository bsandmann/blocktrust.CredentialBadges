using FluentResults;
using Blocktrust.CredentialBadges.Core.Prism;
using OpenPrismNode;

namespace Blocktrust.CredentialBadges.Core.Services.Prism;

public class ExtractKeysFromLongformPrismDID
{
    public Result<byte[]> ExtractPublicKey(string longformDid)
    {
        try
        {
            // Split the DID into its components
            var parts = longformDid.Split(':');
            if (parts.Length != 4 || parts[0] != "did" || parts[1] != "prism")
            {
                return Result.Fail<byte[]>("Invalid long-form Prism DID format");
            }

            // Extract the base64url encoded part
            var encodedState = parts[3];

            // Decode the base64url string to bytes
            byte[] decodedState = PrismEncoding.Base64ToByteArray(encodedState);

            // Parse the AtalaOperation
            var atalaOperation = AtalaOperation.Parser.ParseFrom(decodedState);

            if (atalaOperation.CreateDid == null)
            {
                return Result.Fail<byte[]>("The operation is not a CreateDIDOperation");
            }

            var createDIDOperation = atalaOperation.CreateDid;

            // Find the first ISSUING_KEY (or MASTER_KEY if ISSUING_KEY is not present)
            var publicKey = createDIDOperation.DidData.PublicKeys
                                .FirstOrDefault(key => key.Usage == KeyUsage.IssuingKey) 
                            ?? createDIDOperation.DidData.PublicKeys
                                .FirstOrDefault(key => key.Usage == KeyUsage.MasterKey);

            if (publicKey == null)
            {
                return Result.Fail<byte[]>("No suitable public key found in the DID document");
            }

            byte[] fullPublicKey;

            if (publicKey.EcKeyData != null)
            {
                fullPublicKey = CombinePublicKeyCoordinates(publicKey.EcKeyData.X.ToByteArray(), publicKey.EcKeyData.Y.ToByteArray());
            }
            else if (publicKey.CompressedEcKeyData != null)
            {
                var decompressResult = PrismPublicKey.Decompress(
                    publicKey.CompressedEcKeyData.Data.ToByteArray(),
                    publicKey.CompressedEcKeyData.Curve
                );
                    
                if (decompressResult.IsFailed)
                {
                    return Result.Fail<byte[]>($"Failed to decompress key: {decompressResult.Errors.First().Message}");
                }

                var (x, y) = decompressResult.Value;
                fullPublicKey = CombinePublicKeyCoordinates(x, y);
            }
            else
            {
                return Result.Fail<byte[]>("Invalid key data in the DID document");
            }

            return Result.Ok(fullPublicKey);
        }
        catch (Exception ex)
        {
            return Result.Fail<byte[]>($"Failed to extract public key: {ex.Message}");
        }
    }

    private byte[] CombinePublicKeyCoordinates(byte[] x, byte[] y)
    {
        byte[] fullPublicKey = new byte[65];
        fullPublicKey[0] = 0x04; // Uncompressed public key prefix
        Array.Copy(x, 0, fullPublicKey, 1, 32);
        Array.Copy(y, 0, fullPublicKey, 33, 32);
        return fullPublicKey;
    }
}