using System.Text;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Crypto;
using FluentResults;
using SimpleBase;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;

namespace Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification
{
    public class DIDKeySignatureVerification
    {
        private readonly EcServiceBouncyCastle _ecService;

        public DIDKeySignatureVerification(EcServiceBouncyCastle ecService)
        {
            _ecService = ecService;
        }

        public Result<ECheckSignatureResponse> VerifySignature(string didKey, string proofValue, string signedData)
        {
            try
            {
                var publicKeyMultibase = ExtractPublicKeyMultibase(didKey);
                var publicKey = ConvertMultibaseToPublicKey(publicKeyMultibase);
                
                bool isValid = VerifySignatureInternal(signedData, proofValue, publicKey);

                return isValid 
                    ? Result.Ok(ECheckSignatureResponse.Valid) 
                    : Result.Ok(ECheckSignatureResponse.Invalid);
            }
            catch (Exception ex)
            {
                return Result.Fail<ECheckSignatureResponse>($"Error during DID Key signature verification: {ex.Message}");
            }
        }

        public string ExtractPublicKeyMultibase(string didKey)
        {
            // The public key is the part after "did:key:"
            return didKey.Substring(8);
        }

        public byte[] ConvertMultibaseToPublicKey(string multibaseKey)
        {
            // Remove the multibase prefix (first character)
            string encodedKey = multibaseKey.Substring(1);

            // Decode the Base58 encoding using SimpleBase
            byte[] decodedKey = Base58.Bitcoin.Decode(encodedKey).ToArray();

            // Remove the multicodec prefix (first two bytes)
            return decodedKey.Skip(2).ToArray();
        }

        private bool VerifySignatureInternal(string signedData, string proofValue, byte[] publicKey)
        {
            byte[] signature = Convert.FromBase64String(proofValue);
            byte[] message = Encoding.UTF8.GetBytes(signedData);

            try
            {
                return VerifyEd25519Signature(message, signature, publicKey);
            }
            catch (Exception)
            {
                // If verification throws an exception, we consider it invalid
                return false;
            }
        }

        public bool VerifyEd25519Signature(byte[] message, byte[] signature, byte[] publicKey)
        {
            try
            {
                var verifier = new Ed25519Signer();
                verifier.Init(false, new Ed25519PublicKeyParameters(publicKey, 0));
                verifier.BlockUpdate(message, 0, message.Length);
                return verifier.VerifySignature(signature);
            }
            catch (Exception)
            {
                // If verification throws an exception, we consider it invalid
                return false;
            }
        }
    }
}