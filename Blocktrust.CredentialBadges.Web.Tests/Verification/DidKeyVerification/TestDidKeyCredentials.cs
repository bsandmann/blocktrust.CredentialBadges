namespace Blocktrust.CredentialBadges.Web.Tests.Verification.DidKeyVerification;

public static class TestDidKeyCredentials
{
  public const string ValidCredential = """
                                        {
                                          "@context": [
                                            "https://www.w3.org/2018/credentials/v1",
                                            "https://purl.imsglobal.org/spec/ob/v3p0/context-3.0.1.json",
                                            "https://w3id.org/security/suites/ed25519-2020/v1"
                                          ],
                                          "id": "urn:uuid:cebd27cf-f753-471d-bc2b-b728e51595f3",
                                          "type": [
                                            "VerifiableCredential",
                                            "OpenBadgeCredential"
                                          ],
                                          "name": "DCC Test Credential",
                                          "issuer": {
                                            "type": [
                                              "Profile"
                                            ],
                                            "id": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                            "name": "Digital Credentials Consortium Test Issuer",
                                            "url": "https://dcconsortium.org",
                                            "image": "https://user-images.githubusercontent.com/947005/133544904-29d6139d-2e7b-4fe2-b6e9-7d1022bb6a45.png"
                                          },
                                          "issuanceDate": "2023-08-02T21:19:28.154Z",
                                          "credentialSubject": {
                                            "type": [
                                              "AchievementSubject"
                                            ],
                                            "achievement": {
                                              "id": "urn:uuid:bd6d9316-f7ae-4073-a1e5-2f7f5bd22922",
                                              "type": [
                                                "Achievement"
                                              ],
                                              "achievementType": "Diploma",
                                              "name": "Badge",
                                              "description": "This is a sample credential issued by the Digital Credentials Consortium to demonstrate the functionality of Verifiable Credentials for wallets and verifiers.",
                                              "criteria": {
                                                "type": "Criteria",
                                                "narrative": "This credential was issued to a student that demonstrated proficiency in the Python programming language through activities performed in the course titled *Introduction to Python* offered by [Example Institute of Technology](https://exit.example.edu) from **February 17, 2023** to **June 12, 2023**. This is a credential with the following criteria:\n1. completed all homework assignments\n2. passed all exams\n3. completed final group project"
                                              },
                                              "image": {
                                                "id": "https://user-images.githubusercontent.com/752326/214947713-15826a3a-b5ac-4fba-8d4a-884b60cb7157.png",
                                                "type": "Image"
                                              }
                                            },
                                            "name": "Jane Doe"
                                          },
                                          "expirationDate": "2025-07-26T00:00:00.000Z",
                                          "proof": {
                                            "type": "Ed25519Signature2020",
                                            "created": "2023-08-02T21:19:28Z",
                                            "verificationMethod": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC#z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                            "proofPurpose": "assertionMethod",
                                            "proofValue": "z6sVGrHrDE1K8TLSV8qK87GZEpNHH1S3TTi9KhKyKiXCDPtxT2Y2Hs5xX5ZK3McwhHGoGUdoG9tu9vJsLxMDazVC"
                                          }
                                        }
                                        """;


  public const string ValidCredentialNotRevoked = """
                                        {
                                        "@context": [
                                           "https://www.w3.org/2018/credentials/v1",
                                           "https://w3id.org/security/suites/ed25519-2020/v1",
                                           "https://w3id.org/dcc/v1",
                                           "https://w3id.org/vc/status-list/2021/v1"
                                         ],
                                         "type": [
                                           "VerifiableCredential",
                                           "Assertion"
                                         ],
                                         "issuer": {
                                           "id": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                           "name": "Example University",
                                           "url": "https://cs.example.edu",
                                           "image": "https://user-images.githubusercontent.com/947005/133544904-29d6139d-2e7b-4fe2-b6e9-7d1022bb6a45.png"
                                         },
                                         "issuanceDate": "2020-08-16T12:00:00.000+00:00",
                                         "credentialSubject": {
                                           "id": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                           "name": "Kayode Ezike",
                                           "hasCredential": {
                                             "type": [
                                               "EducationalOccupationalCredential"
                                             ],
                                             "name": "GT Guide",
                                             "description": "The holder of this credential is qualified to lead new student orientations."
                                           }
                                         },
                                         "expirationDate": "2025-08-16T12:00:00.000+00:00",
                                         "credentialStatus": {
                                           "id": "https://digitalcredentials.github.io/credential-status-playground/JWZM3H8WKU#2",
                                           "type": "StatusList2021Entry",
                                           "statusPurpose": "revocation",
                                           "statusListIndex": 2,
                                           "statusListCredential": "https://digitalcredentials.github.io/credential-status-playground/JWZM3H8WKU"
                                         },
                                         "proof": {
                                           "type": "Ed25519Signature2020",
                                           "created": "2022-08-19T06:55:17Z",
                                           "verificationMethod": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC#z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                           "proofPurpose": "assertionMethod",
                                           "proofValue": "z4EiTbmC79r4dRaqLQZr2yxQASoMKneHVNHVaWh1xcDoPG2eTwYjKoYaku1Canb7a6Xp5fSogKJyEhkZCaqQ6Y5nw"
                                         }
                                        }


                                        """;


  public const string ValidCredentialRevoked = """
                                           
                                               {
                                                     "@context": [
                                                       "https://www.w3.org/2018/credentials/v1",
                                                       "https://w3id.org/security/suites/ed25519-2020/v1",
                                                       "https://w3id.org/dcc/v1",
                                                       "https://w3id.org/vc/status-list/2021/v1"
                                                     ],
                                                     "type": [
                                                       "VerifiableCredential",
                                                       "Assertion"
                                                     ],
                                                     "issuer": {
                                                       "id": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                                       "name": "Example University",
                                                       "url": "https://cs.example.edu",
                                                       "image": "https://user-images.githubusercontent.com/947005/133544904-29d6139d-2e7b-4fe2-b6e9-7d1022bb6a45.png"
                                                     },
                                                     "issuanceDate": "2020-08-16T12:00:00.000+00:00",
                                                     "credentialSubject": {
                                                       "id": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                                       "name": "Kayode Ezike",
                                                       "hasCredential": {
                                                         "type": [
                                                           "EducationalOccupationalCredential"
                                                         ],
                                                         "name": "GT Guide",
                                                         "description": "The holder of this credential is qualified to lead new student orientations."
                                                       }
                                                     },
                                                     "expirationDate": "2025-08-16T12:00:00.000+00:00",
                                                     "credentialStatus": {
                                                       "id": "https://digitalcredentials.github.io/credential-status-playground/JWZM3H8WKU#3",
                                                       "type": "StatusList2021Entry",
                                                       "statusPurpose": "revocation",
                                                       "statusListIndex": 3,
                                                       "statusListCredential": "https://digitalcredentials.github.io/credential-status-playground/JWZM3H8WKU"
                                                     },
                                                     "proof": {
                                                       "type": "Ed25519Signature2020",
                                                       "created": "2022-08-19T06:58:29Z",
                                                       "verificationMethod": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC#z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                                       "proofPurpose": "assertionMethod",
                                                       "proofValue": "z33Wy3kvx8UEoPHdQWYHVCXAjW19AZpA88NnikwfJqcH9oNmHyqSkt6wiVS31ewytAX7m2vneVEm8Awo4xzqKHYUp"
                                                     }
                                               }
                                               
                                               

                                               """;
                                        
                                          public const string ValidCredentialWithoutProof = """
                                                    {
                                                      "@context": [
                                                        "https://www.w3.org/2018/credentials/v1",
                                                        "https://purl.imsglobal.org/spec/ob/v3p0/context-3.0.1.json",
                                                        "https://w3id.org/security/suites/ed25519-2020/v1"
                                                      ],
                                                      "id": "urn:uuid:cebd27cf-f753-471d-bc2b-b728e51595f3",
                                                      "type": [
                                                        "VerifiableCredential",
                                                        "OpenBadgeCredential"
                                                      ],
                                                      "name": "DCC Test Credential",
                                                      "issuer": {
                                                        "type": [
                                                          "Profile"
                                                        ],
                                                        "id": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                                        "name": "Digital Credentials Consortium Test Issuer",
                                                        "url": "https://dcconsortium.org",
                                                        "image": "https://user-images.githubusercontent.com/947005/133544904-29d6139d-2e7b-4fe2-b6e9-7d1022bb6a45.png"
                                                      },
                                                      "issuanceDate": "2023-08-02T21:19:28.154Z",
                                                      "credentialSubject": {
                                                        "type": [
                                                          "AchievementSubject"
                                                        ],
                                                        "achievement": {
                                                          "id": "urn:uuid:bd6d9316-f7ae-4073-a1e5-2f7f5bd22922",
                                                          "type": [
                                                            "Achievement"
                                                          ],
                                                          "achievementType": "Diploma",
                                                          "name": "Badge",
                                                          "description": "This is a sample credential issued by the Digital Credentials Consortium to demonstrate the functionality of Verifiable Credentials for wallets and verifiers.",
                                                          "criteria": {
                                                            "type": "Criteria",
                                                            "narrative": "This credential was issued to a student that demonstrated proficiency in the Python programming language through activities performed in the course titled *Introduction to Python* offered by [Example Institute of Technology](https://exit.example.edu) from **February 17, 2023** to **June 12, 2023**. This is a credential with the following criteria:\n1. completed all homework assignments\n2. passed all exams\n3. completed final group project"
                                                          },
                                                          "image": {
                                                            "id": "https://user-images.githubusercontent.com/752326/214947713-15826a3a-b5ac-4fba-8d4a-884b60cb7157.png",
                                                            "type": "Image"
                                                          }
                                                        },
                                                        "name": "Jane Doe"
                                                      },
                                                      "expirationDate": "2025-07-26T00:00:00.000Z"
                                                    }
                                                    """;

  public const string ValidCredential2 = """
                                         {
                                             "@context": [
                                                 "https://www.w3.org/ns/credentials/v2",
                                                 "https://purl.imsglobal.org/spec/ob/v3p0/context-3.0.2.json",
                                                 "https://w3id.org/security/suites/ed25519-2020/v1"
                                             ],
                                             "id": "urn:uuid:2fe53dc9-b2ec-4939-9b2c-0d00f6663b6c",
                                             "type": [
                                                 "VerifiableCredential",
                                                 "OpenBadgeCredential"
                                             ],
                                             "name": "DCC Test Credential",
                                             "issuer": {
                                                 "type": [
                                                     "Profile"
                                                 ],
                                                 "id": "did:key:z6MknNQD1WHLGGraFi6zcbGevuAgkVfdyCdtZnQTGWVVvR5Q",
                                                 "name": "Digital Credentials Consortium Test Issuer",
                                                 "url": "https://dcconsortium.org",
                                                 "image": "https://user-images.githubusercontent.com/752326/230469660-8f80d264-eccf-4edd-8e50-ea634d407778.png"
                                             },
                                             "validFrom": "2023-08-02T17:43:32.903Z",
                                             "credentialSubject": {
                                                 "type": [
                                                     "AchievementSubject"
                                                 ],
                                                 "achievement": {
                                                     "id": "urn:uuid:bd6d9316-f7ae-4073-a1e5-2f7f5bd22922",
                                                     "type": [
                                                         "Achievement"
                                                     ],
                                                     "achievementType": "Diploma",
                                                     "name": "Badge",
                                                     "description": "This is a sample credential issued by the Digital Credentials Consortium to demonstrate the functionality of Verifiable Credentials for wallets and verifiers.",
                                                     "criteria": {
                                                         "type": "Criteria",
                                                         "narrative": "This credential was issued to a student that demonstrated proficiency in the Python programming language that occurred from **February 17, 2023** to **June 12, 2023**."
                                                     },
                                                     "image": {
                                                         "id": "https://user-images.githubusercontent.com/752326/214947713-15826a3a-b5ac-4fba-8d4a-884b60cb7157.png",
                                                         "type": "Image"
                                                     }
                                                 },
                                                 "name": "Jane Doe"
                                             },
                                             "proof": {
                                                 "type": "Ed25519Signature2020",
                                                 "created": "2023-10-05T11:17:41Z",
                                                 "verificationMethod": "did:key:z6MknNQD1WHLGGraFi6zcbGevuAgkVfdyCdtZnQTGWVVvR5Q#z6MknNQD1WHLGGraFi6zcbGevuAgkVfdyCdtZnQTGWVVvR5Q",
                                                 "proofPurpose": "assertionMethod",
                                                 "proofValue": "z5fk6gq9upyZvcFvJdRdeL5KmvHr69jxEkyDEd2HyQdyhk9VnDEonNSmrfLAcLEDT9j4gGdCG24WHhojVHPbRsNER"
                                             }
                                         }
                                         """;

  public const string ValidCredentialNoProof = """
                                               {
                                                 "@context": [
                                                   "https://www.w3.org/2018/credentials/v1",
                                                   "https://purl.imsglobal.org/spec/ob/v3p0/context-3.0.1.json",
                                                   "https://w3id.org/security/suites/ed25519-2020/v1"
                                                 ],
                                                 "id": "urn:uuid:cebd27cf-f753-471d-bc2b-b728e51595f3",
                                                 "type": [
                                                   "VerifiableCredential",
                                                   "OpenBadgeCredential"
                                                 ],
                                                 "name": "DCC Test Credential",
                                                 "issuer": {
                                                   "type": [
                                                     "Profile"
                                                   ],
                                                   "id": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                                   "name": "Digital Credentials Consortium Test Issuer",
                                                   "url": "https://dcconsortium.org",
                                                   "image": "https://user-images.githubusercontent.com/947005/133544904-29d6139d-2e7b-4fe2-b6e9-7d1022bb6a45.png"
                                                 },
                                                 "issuanceDate": "2023-08-02T21:19:28.154Z",
                                                 "credentialSubject": {
                                                   "type": [
                                                     "AchievementSubject"
                                                   ],
                                                   "achievement": {
                                                     "id": "urn:uuid:bd6d9316-f7ae-4073-a1e5-2f7f5bd22922",
                                                     "type": [
                                                       "Achievement"
                                                     ],
                                                     "achievementType": "Diploma",
                                                     "name": "Badge",
                                                     "description": "This is a sample credential issued by the Digital Credentials Consortium to demonstrate the functionality of Verifiable Credentials for wallets and verifiers.",
                                                     "criteria": {
                                                       "type": "Criteria",
                                                       "narrative": "This credential was issued to a student that demonstrated proficiency in the Python programming language through activities performed in the course titled *Introduction to Python* offered by [Example Institute of Technology](https://exit.example.edu) from **February 17, 2023** to **June 12, 2023**. This is a credential with the following criteria:\n1. completed all homework assignments\n2. passed all exams\n3. completed final group project"
                                                     },
                                                     "image": {
                                                       "id": "https://user-images.githubusercontent.com/752326/214947713-15826a3a-b5ac-4fba-8d4a-884b60cb7157.png",
                                                       "type": "Image"
                                                     }
                                                   },
                                                   "name": "Jane Doe"
                                                 },
                                                 "expirationDate": "2025-07-26T00:00:00.000Z",
                                               }
                                               """;

  public const string InvalidSignatureCredential = """
                                                   {
                                                     "@context": [
                                                       "https://www.w3.org/2018/credentials/v1",
                                                       "https://purl.imsglobal.org/spec/ob/v3p0/context-3.0.1.json",
                                                       "https://w3id.org/security/suites/ed25519-2020/v1"
                                                     ],
                                                     "id": "urn:uuid:cebd27cf-f753-471d-bc2b-b728e51595f3",
                                                     "type": [
                                                       "VerifiableCredential",
                                                       "OpenBadgeCredential"
                                                     ],
                                                     "name": "DCC Test Credential",
                                                     "issuer": {
                                                       "type": [
                                                         "Profile"
                                                       ],
                                                       "id": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                                       "name": "Digital Credentials Consortium Test Issuer",
                                                       "url": "https://dcconsortium.org",
                                                       "image": "https://user-images.githubusercontent.com/947005/133544904-29d6139d-2e7b-4fe2-b6e9-7d1022bb6a45.png"
                                                     },
                                                     "issuanceDate": "2023-08-02T21:19:28.154Z",
                                                     "credentialSubject": {
                                                       "type": [
                                                         "AchievementSubject"
                                                       ],
                                                       "achievement": {
                                                         "id": "urn:uuid:bd6d9316-f7ae-4073-a1e5-2f7f5bd22922",
                                                         "type": [
                                                           "Achievement"
                                                         ],
                                                         "achievementType": "Diploma",
                                                         "name": "Badge",
                                                         "description": "This is a sample credential issued by the Digital Credentials Consortium to demonstrate the functionality of Verifiable Credentials for wallets and verifiers.",
                                                         "criteria": {
                                                           "type": "Criteria",
                                                           "narrative": "This credential was issued to a student that demonstrated proficiency in the Python programming language through activities performed in the course titled *Introduction to Python* offered by [Example Institute of Technology](https://exit.example.edu) from **February 17, 2023** to **June 12, 2023**. This is a credential with the following criteria:\n1. completed all homework assignments\n2. passed all exams\n3. completed final group project"
                                                         },
                                                         "image": {
                                                           "id": "https://user-images.githubusercontent.com/752326/214947713-15826a3a-b5ac-4fba-8d4a-884b60cb7157.png",
                                                           "type": "Image"
                                                         }
                                                       },
                                                       "name": "Jane Doe"
                                                     },
                                                     "expirationDate": "2025-07-26T00:00:00.000Z",
                                                     "proof": {
                                                       "type": "Ed25519Signature2020",
                                                       "created": "2023-08-02T21:19:28Z",
                                                       "verificationMethod": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC#z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                                       "proofPurpose": "assertionMethod",
                                                       "proofValue": "VGrHrDE1K8TLSV8qK87GZEpNHH1S3TTi9KhKyKiXCDPtxT2Y2Hs5xX5nvalid"
                                                     }
                                                   }
                                                   """;

  public const string InvalidDIDKeyCredential = """
                                                {
                                                  "@context": [
                                                    "https://www.w3.org/2018/credentials/v1",
                                                    "https://purl.imsglobal.org/spec/ob/v3p0/context-3.0.1.json",
                                                    "https://w3id.org/security/suites/ed25519-2020/v1"
                                                  ],
                                                  "id": "urn:uuid:cebd27cf-f753-471d-bc2b-b728e51595f3",
                                                  "type": [
                                                    "VerifiableCredential",
                                                    "OpenBadgeCredential"
                                                  ],
                                                  "name": "DCC Test Credential",
                                                  "issuer": {
                                                    "type": [
                                                      "Profile"
                                                    ],
                                                    "id": "did:key:invalidDIDab",
                                                    "name": "Digital Credentials Consortium Test Issuer",
                                                    "url": "https://dcconsortium.org",
                                                    "image": "https://user-images.githubusercontent.com/947005/133544904-29d6139d-2e7b-4fe2-b6e9-7d1022bb6a45.png"
                                                  },
                                                  "issuanceDate": "2023-08-02T21:19:28.154Z",
                                                  "credentialSubject": {
                                                    "type": [
                                                      "AchievementSubject"
                                                    ],
                                                    "achievement": {
                                                      "id": "urn:uuid:bd6d9316-f7ae-4073-a1e5-2f7f5bd22922",
                                                      "type": [
                                                        "Achievement"
                                                      ],
                                                      "achievementType": "Diploma",
                                                      "name": "Badge",
                                                      "description": "This is a sample credential issued by the Digital Credentials Consortium to demonstrate the functionality of Verifiable Credentials for wallets and verifiers.",
                                                      "criteria": {
                                                        "type": "Criteria",
                                                        "narrative": "This credential was issued to a student that demonstrated proficiency in the Python programming language through activities performed in the course titled *Introduction to Python* offered by [Example Institute of Technology](https://exit.example.edu) from **February 17, 2023** to **June 12, 2023**. This is a credential with the following criteria:\n1. completed all homework assignments\n2. passed all exams\n3. completed final group project"
                                                      },
                                                      "image": {
                                                        "id": "https://user-images.githubusercontent.com/752326/214947713-15826a3a-b5ac-4fba-8d4a-884b60cb7157.png",
                                                        "type": "Image"
                                                      }
                                                    },
                                                    "name": "Jane Doe"
                                                  },
                                                  "expirationDate": "2025-07-26T00:00:00.000Z",
                                                  "proof": {
                                                    "type": "Ed25519Signature2020",
                                                    "created": "2023-08-02T21:19:28Z",
                                                    "verificationMethod": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC#z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                                    "proofPurpose": "assertionMethod",
                                                    "proofValue": "z6sVGrHrDE1K8TLSV8qK87GZEpNHH1S3TTi9KhKyKiXCDPtxT2Y2Hs5xX5ZK3McwhHGoGUdoG9tu9vJsLxMDazVC"
                                                  }
                                                }
                                                """;
}