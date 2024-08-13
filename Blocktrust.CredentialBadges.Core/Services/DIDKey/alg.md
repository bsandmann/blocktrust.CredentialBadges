# STEPS FOR RESOLVING DID KEY

## 1. Validate the input DID key

- This step ensures that the input is valid before processing.
- Check if the input string is not null or empty to avoid processing invalid data.
- Verify that the string starts with "did:key:" to ensure it follows the DID key format.
- This validation helps prevent errors in subsequent steps and provides early feedback on invalid inputs.

## 2. Extract the key part

- After confirming the "did:key:" prefix, remove it to isolate the actual key data.
- Verify that the remaining part starts with "z6Mk", which is the prefix for Ed25519 keys in the multicodec format.
- This step ensures we're dealing with the correct key type (Ed25519) before proceeding.

## 3. Decode the Base58-encoded key

- The key part is encoded in Base58, a binary-to-text encoding scheme.
- Use a Base58 decoding algorithm to convert the string back into its original byte representation.
- After decoding, verify that the resulting byte array is exactly 36 bytes long. This length is specific to the encoding scheme used for Ed25519 keys in the did:key method.

## 4. Extract the public key

- The decoded 36-byte array contains more than just the public key.
- Remove the first 2 bytes, which represent the multicodec prefix (identifying the key type).
- Remove the last 2 bytes, which are typically used for error checking.
- The remaining 32 bytes in the middle constitute the actual Ed25519 public key.

## 5. Create the Verification Method

- Construct an object that represents the verification method.
- Include properties:
    - id: typically the DID plus a fragment (e.g., "#keys-1")
    - type: set to "Ed25519VerificationKey2020"
    - controller: set to the full DID
    - publicKeyMultibase: the original Base58-encoded key part

## 6. Convert Ed25519 public key to X25519

- Ed25519 is used for signatures, while X25519 is used for key agreement (encryption).
- Apply the conversion algorithm described in RFC 7748 to derive the X25519 public key from the Ed25519 public key.
- This conversion allows the same key pair to be used for both signing and encryption.

## 7. Create the Key Agreement

- Construct another object, similar to the Verification Method, but for key agreement.
- Use properties similar to the Verification Method, but:
    - type is set to "X25519KeyAgreementKey2020"
    - publicKeyMultibase uses the Base58-encoded X25519 public key

## 8. Create the DID Document

- Construct a larger object that represents the entire DID document.
- Include standard properties like @context and id.
- Add the Verification Method and Key Agreement objects created in steps 5 and 7.
- Include authentication, assertionMethod, capabilityDelegation, and capabilityInvocation properties, typically referencing the Verification Method.
- Add the keyAgreement property, referencing the Key Agreement object.

## 9. Serialize the DID Document to JSON

- Convert the DID Document object into a JSON string.
- Use proper naming policies (e.g., camelCase for property names).
- Apply formatting for readability (e.g., indentation).

## 10. Return the serialized DID Document

- The final step is to return the JSON string representation of the DID Document.
- This document can now be used in various decentralized identity operations.